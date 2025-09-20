using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using UrlShortener.Core.Domain.Adapters;
using UrlShortener.Core.Domain.Errors;
using UrlShortener.Core.Domain.Results;
using UrlShortener.Core.Services.Interfaces;
using UrlShortener.Core.Validators;
using UrlShortener.Data.Entities.Plans;
using UrlShortener.Data.Repositories.Implementation;
using UrlShortener.Models.DTOs.Paging;
using UrlShortener.Models.DTOs.Plan;
using static UrlShortener.Core.Domain.Errors.Error;

namespace UrlShortener.Core.Services.Implementations
{  
    public class PlanService : IPlanService
    {
        private readonly IRepositoryManager _repo;
        private readonly IServiceManager _sm; 
        private readonly ILoggerManager _logger;

        public PlanService(IRepositoryManager repository, IServiceManager serviceManager, ILoggerManager logger)
        {
            _repo = repository;
            _sm = serviceManager;
            _logger = logger;   
        }

#region HelperMethods    
        private Result<int> EnsureValidId(Result<int> result, string paramName = "planId")
            => result.Ensure(i => i > 0, _ => BadRequest($"Id must be greater than zero (Parameter '{paramName}')"));

        private Result<IEnumerable<int>> EnsureValidIDs(IEnumerable<int> urlIDs)
        {
            if (urlIDs.Count() == 0) return Error.BadRequest($"Require one valid ID or more.");

            var errors = new Dictionary<string, List<string>> { { "IDs", [] } };
            var set = new HashSet<int>();

            foreach (int id in urlIDs)
            {
                if (id < 1)
                {
                    errors["IDs"].Add(id.ToString());
                }

                set.Add(id);
            }

            if (errors.Count() - 1 > 0)
                return new ValidationError(errors, "Validation Failed", "One or more url IDs are invalid. Id must be greater than zero.", enErrorType.BadRequest);
            return set;
        }

        private async Task<bool> hasNoConflict(Plan plan)
            => !await _repo.Plan.ExistsAnyAsync(p => p.Name == plan.Name);
#endregion

#region CreateMethods
        public async Task<Result<FullPlanDTO>> CreatePlanAsync(CreatePlanDTO planDTO)
            => await planDTO.Validate()
                    .Map(_ => planDTO.ToPlan())
                    .EnsureAsync(hasNoConflict, _ => AlreadyExists("Plan:Name"))
                    .Tap(_repo.Plan.Add)
                    .TapAsync(_ => _repo.SaveAsync())
                    .TryCatch(ex => { _logger.LogWarn("Creating plan has failed by {userID}", ex, args: [_sm.User.Id]); return ex.ToDbError(); })
                    .Map(plan => plan.ToFullDTO());

        public async Task<Result<IEnumerable<FullPlanDTO>>> CreatePlansAsync(IEnumerable<CreatePlanDTO> planDTOs, CancellationToken ct)
            => await Result<int>.Create(0)
                    .Ensure(_ => planDTOs.All(p => p.Validate().IsSuccess), Err => Err)
                    .Map(_ => planDTOs.Select(p => p.ToPlan()).ToList())
                    .Tap(plans => _repo.Plan.AddOrUpdate(plans, ct))
                    .TapAsync(_ => _repo.SaveAsync(ct))
                    .TryCatch(ex => { _logger.LogWarn("Creating plans has failed by {userID}", ex, args: [_sm.User.Id]); return ex.ToDbError(); })
                    .Map(plan => plan.Select(p => p.ToFullDTO()));
#endregion

#region ReadMethods

        // ====== Retrieve URL Entity ====== //
        public async Task<Result<IEnumerable<Plan>>> RetrieveAllPlansAsync(Result<PlanParameters> PlanParam, bool asNoTracking, CancellationToken ct = default)
            => await PlanParam
                    .MapAsync(param => _repo.Plan.GetAllPlansAsync(param, ct, asNoTracking))
                    .TryCatch(ex => { _logger.LogError("Failed to fetch Plans by {userID}", ex, args: [_sm.User.Id]); return ex.ToDbError(); });

        public async Task<Result<IEnumerable<Plan?>>> RetrievePlansByIdAsync(IEnumerable<int> ids, bool asNoTracking, CancellationToken ct = default)
            => await EnsureValidIDs(ids)
                    .MapAsync(set => _repo.Plan.GetByIDsAsync(set, ct, true))
                    .TryCatch(ex => { _logger.LogError("Failed to fetch Plans by {userID}", ex, args: [_sm.User.Id]); return ex.ToDbError(); })
                    .Ensure(plan => plan != null && plan.Count() != 0, _ => PlanError.ListNotFound)
                    .Tap(plans => _logger.LogTrace("{count} Plan has been fetched by {userID}", args: [plans.Count()]));

        public async Task<Result<Plan?>> RetrievePlanByIdAsync(int id)
            => await EnsureValidId(id)
                    .MapAsync(_ => _repo.Plan.GetByIdAsync(id))
                    .TryCatch(ex => { _logger.LogError("Failed to fetch Plan:{planID} by {userID}", ex, args: [id, _sm.User.Id]); return ex.ToDbError(); });

        public async Task<Result<Plan>> RetrievePlanAndValidateExistenceAsync(int id)
            => (await RetrievePlanByIdAsync(id)
              .Ensure(plan => plan != null, err => PlanError.NotFound(id)))!;


        // ====== Mapping Entity To DTO ====== //
        public async Task<Result<PagedList<FullPlanDTO>>> RetrieveAllPlansWithPagingAsync(PlanParameters PlanParam, bool asNoTracking = true, CancellationToken ct = default)
            => await RetrieveAllPlansAsync(PlanParam, asNoTracking, ct)
                    .Map(plans => PagedList<FullPlanDTO>.ToPagedList(plans.Select(p => p.ToFullDTO()).ToList(), PlanParam.PageNumber, PlanParam.PageSize));
        public async Task<Result<IEnumerable<FullPlanDTO>>> RetrievePlansWithDTOAsync(IEnumerable<int> ids, bool asNoTracking = true, CancellationToken ct = default)
            => await RetrievePlansByIdAsync(ids, asNoTracking, ct)
                    .Map(plans => plans.Select(p => p.ToFullDTO()));

        public async Task<Result<FullPlanDTO>> RetrievePlanWithDTOAsync(int id)
            => await RetrievePlanAndValidateExistenceAsync(id)
                    .Map(plan => plan.ToFullDTO());

        #endregion

        #region UpdateMethods

        public async Task<Result<FullPlanDTO>> UpdatePlanAsync(int id, JsonPatchDocument<UpdatePlanDTO> pathDto)
            => await RetrievePlanAndValidateExistenceAsync(id)
                    .Tap(plan => plan!.Patch(pathDto))
                    .TapAsync(_ => _repo.SaveAsync())
                    .TryCatch(ex => { _logger.LogError("Deleting plan:{planId} has failed by {userID}", ex, args: [id, _sm.User.Id]); return ex.ToDbError(); })
                    .Map(plan => plan!.ToFullDTO());
#endregion

#region RemoveMethods
        public async Task<Result> DeletePlanAsync(int id)
            => await RetrievePlanAndValidateExistenceAsync(id)
                    .Tap(_repo.Plan.Delete)
                    .TapAsync(_ => _repo.SaveAsync())
                    .TryCatch(ex => { _logger.LogError("Deleting plan:{planId} has failed by {userID}", ex, args: [id, _sm.User.Id]); return ex.ToDbError(); });
        public async Task<Result> DeleteByIDsAsync(IEnumerable<int> ids, CancellationToken ct)
            => await EnsureValidIDs(ids)
                    .EnsureAsync(async set => await _repo.Plan.CountAsync(p => set.Contains(p.ID)) == set.Count(), _ => PlanError.ListNotFound)
                    .TapAsync(_ => _repo.Plan.ExecuteDeleteAsync(ids, ct))
                    .TryCatch(ex => {_logger.LogError("Deleting plans has failed by {userID}", ex, args: [_sm.User.Id]); return ex.ToDbError();});
        #endregion




    }

}

