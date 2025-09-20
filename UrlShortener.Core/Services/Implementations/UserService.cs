using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using NLog.Targets;
using UrlShortener.Core.Domain.Adapters;
using UrlShortener.Core.Domain.Constants;
using UrlShortener.Core.Domain.Errors;
using UrlShortener.Core.Domain.Results;
using UrlShortener.Core.Factories;
using UrlShortener.Core.Services.Interfaces;
using UrlShortener.Core.Validators;
using UrlShortener.Data.Entities.Users;
using UrlShortener.Data.Repositories.Implementation;
using UrlShortener.Models.DTOs;
using UrlShortener.Models.DTOs.Paging;
using UrlShortener.Models.DTOs.Plan;
using UrlShortener.Models.DTOs.User;
using UrlShortener.Models.Enums;
using static UrlShortener.Core.Domain.Errors.Error;





namespace UrlShortener.Core.Services.Implementations
{
    
    public class UserService : IUserService
    {


        private readonly IRepositoryManager _repo;
        private readonly IServiceManager _sm;
        private readonly ILoggerManager _logger;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserService(IRepositoryManager repo, IServiceManager sm, ILoggerManager logger)
        {
            _repo = repo;
            _sm = sm;
            _logger = logger;
            _passwordHasher = new PasswordHasher<User>();
        }

        #region HelperMethods

        private Result<int>                 EnsureValidId(Result<int> result, string paramName = "userId")
            => result.Ensure(i => i > 0, _ => BadRequest($"Id must be greater than zero (Parameter '{paramName}')"));

        private Result<IEnumerable<int>>    EnsureValidIDs(IEnumerable<int> urlIDs)
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
                return new ValidationError(errors, "Validation Failed", "One or more User IDs are invalid. Id must be greater than zero.", enErrorType.BadRequest);
            return set;
        }

        private async Task<Result<IEnumerable<User>>> ToUserListIfValidDto(IEnumerable<CreateUserDTO> dtos, CancellationToken ct)
        {
            var users = new List<User>();

            var plansResult = await _sm.PlanService.RetrieveAllPlansAsync(new PlanParameters { PageSize = 999 }, asNoTracking: true, ct);
            if (plansResult.IsFailure)
                return plansResult.Error;

            var plans = plansResult.Value.ToDictionary(p => p.ID);


            foreach (var dto in dtos)
            {
                if (!plans.ContainsKey(dto.PlanId))
                    return BadRequest("One or more users have invalid Plans");

                var validate = dto.Validate();
                if (validate.IsFailure)
                    return validate.Error;

                if (await _repo.User.ExistsAnyAsync(u => u.Email == dto.Email))
                    return UserError.EmailExists;

                if (!EnumValidator<enRole>.IsDefined(dto.RoleId))
                    return BadRequest("One or more users have invalid Roles");


                users.Add(dto.ToUser(plans[dto.PlanId]));

            }

            return users;
        }

        private async Task<bool> hasNoConflict(string email)
            => !await _repo.User.ExistsAnyAsync(u => u.Email == email);

        #endregion
        #region Create

        private async Task<Result<AccessTokenDTO>> GenerateToken(User user)
        {
            var permissions = await _sm.PermissionService.GetPermissionsAsync(user.ID);
            return _sm.TokenService.CreateToken(user, permissions.Value);

        }

        private async Task<Result<User>> CreateUserInternal(Result<User> user, enRole role) // Doesn't return the new id of created user
           => await  user
                    .Tap(u => u.Password = _passwordHasher.HashPassword(u, u.Password))
                    .Tap(u => u.RoleMappings.Add(new UserRoleMapping { RoleId = new RoleFactory().GetRoleID(role) }))
                    .WithTransaction(unitOfWork: _repo, action: async u => { _repo.User.Add(u); await _repo.SaveAsync(); })
                    .TryCatch(ex => { _logger.LogError("Transaction process to create a user has failed, {managerId}", ex, args: [_sm.User.Id]); return ex.ToDbError(); });

        public async Task<Result<IEnumerable<FullUserDTO>>> CreateUsersAsync(IEnumerable<CreateUserDTO> dto, CancellationToken ct = default)
            => await ToUserListIfValidDto(dto, ct)
                    .Tap(u => _repo.User.AddOrUpdate(u, ct))
                    .TapAsync(_ => _repo.SaveAsync())
                    .Map(users => users.Select(u => u.ToFullDTO()));

        public async Task<Result<FullUserDTO>> CreateUserAsync(CreateUserDTO dto)
           => await  dto.Validate()
                    .EnsureAsync(async _ => !await _repo.User.ExistsAnyAsync(u => u.Email == dto.Email), _ => UserError.EmailExists)
                    .TryCatch(ex => { _logger.LogCritical("Email Conflict Validation throw Exception [{email}, {userId}]", ex, args: [dto.Email, _sm.User.Id]); return ex.ToDbError(); })
                    .BindAsync(_ => _sm.PlanService.RetrievePlanAndValidateExistenceAsync(dto.PlanId))
                    .BindAsync(async p => await CreateUserInternal(dto.ToUser(p), (enRole)dto.RoleId))
                    .Map(u => u.ToFullDTO());
        public async Task<Result<AccessTokenDTO>> CreateAsync(CreateInternalUserDTO dto)
           => await  dto.Validate()
                    .EnsureAsync(async _ => !await _repo.User.ExistsAnyAsync(u => u.Email == dto.Email), _ => UserError.EmailExists)
                    .BindAsync(_ => _sm.PlanService.RetrievePlanAndValidateExistenceAsync((int)BusinessDefaults.DefaultPlan))
                    .BindAsync(p => CreateUserInternal(dto.ToUser(p), BusinessDefaults.DefaultRole))
                    .BindAsync(GenerateToken);
#endregion
        #region Read

        // = Retrieve User Entity ====== //

        public async Task<Result<IEnumerable<User>>> RetrieveAllUsersAsync(Result<UserParameters> UserParam, bool withPlan, bool asNoTracking, CancellationToken ct = default)
         => await UserParam
                 .MapAsync(u => _repo.User.GetAllUsersAsync(u, ct, withPlan, asNoTracking))
                 .TryCatch(ex => { _logger.LogError("Retreiving Users has failed, {managerId}", ex); return ex.ToDbError(); });


        public async Task<Result<IEnumerable<User?>>> RetrieveUsersByIdAsync(IEnumerable<int> ids, bool withPlan, bool asNoTracking, CancellationToken ct = default)
            => await EnsureValidIDs(ids)
                    .MapAsync(set => _repo.User.GetByIDsAsync(set, ct, withPlan, asNoTracking))
                    .TryCatch(ex => { _logger.LogError("Retreiving Users has failed, {managerId}", ex); return ex.ToDbError(); });

        async Task<Result<User?>> RetrieveUserByEmailAsync(AuthenticationRequest dto)
            =>  await dto.Validate()
                     .MapAsync(_ => _repo.User.GetByEmailAsync(dto.Email))
                     .TryCatch(ex => { _logger.LogError("Retreiving User:{userEmail} has failed, {managerId}", ex, args: [dto.Email, _sm.User?.Id ?? 0]); return ex.ToDbError(); });

        public async Task<Result<User?>> RetrieveUserByIdAsync(int id, bool withPlan)
            => await EnsureValidId(id)
                    .MapAsync(_ => _repo.User.GetByIdAsync(id, withPlan))
                    .TryCatch(ex => { _logger.LogError("Retreiving User:{userId} has failed, {managerId}", ex, args: [id, _sm.User?.Id ?? 0]); return ex.ToDbError(); });

        public async Task<Result<User>> RetrieveUserAndValidateExistenceAsync(int id, bool withPlan)
            => (await RetrieveUserByIdAsync(id, withPlan)
                    .Ensure(user => user != null, err => UserError.NotFound(id)))!;

        public async Task<Result<User>> RetrieveOwnedUserAsync(bool withPlan)
            => await RetrieveUserAndValidateExistenceAsync(_sm.User.Id, withPlan);



        //  Retrive Entity To DTO ====== //
        public async Task<Result<AccessTokenDTO>> AuthenticateUserAsync(AuthenticationRequest dto)
            => await RetrieveUserByEmailAsync(dto).BindAsync(u => _sm.AuthService.AuthenticateAsync(u, dto));


        public async Task<Result<PagedList<FullUserDTO>>> RetrieveAllUsersWithPagingAsync(UserParameters UserParam, bool withPlan, bool asNoTracking, CancellationToken ct = default)
            => await RetrieveAllUsersAsync(UserParam, withPlan, asNoTracking, ct)
                    .Map(users => PagedList<FullUserDTO>.ToPagedList(users.Select(u => u.ToFullDTO()).ToList(), UserParam.PageNumber, UserParam.PageSize));

        public async Task<Result<IEnumerable<FullUserDTO>>> RetrieveUsersWithDTOAsync(IEnumerable<int> ids, bool withPlan, bool asNoTracking, CancellationToken ct = default)
            => await RetrieveUsersByIdAsync(ids, withPlan, asNoTracking, ct)
                    .Map(users => users.Select(u => u.ToFullDTO()));

        public async Task<Result<FullUserDTO>> RetrieveOwnedUserWithDTOAsync(bool withPlan = true)
            => await RetrieveOwnedUserAsync(withPlan)
                    .Map(user => user.ToFullDTO());

        public async Task<Result<FullUserDTO>> RetrieveUserWithDTOAsync(int id, bool withPlan = true)
            => await RetrieveUserAndValidateExistenceAsync(id, withPlan)
                    .Map(user => user.ToFullDTO());


        #endregion
        #region Update

        public async Task<Result> UpdateUserAsync(int id, JsonPatchDocument<UpdateUserDTO> patchDto)
           => await  RetrieveUserAndValidateExistenceAsync(id, withPlan: false)
                    .Tap(u => u.Patch(patchDto))
                    .EnsureAsync(async u => (await hasNoConflict(u.Email)), _ => UserError.EmailExists)
                    .Tap(u => u.Password = _passwordHasher.HashPassword(u, u.Password))
                    .TapAsync(_ => _repo.SaveAsync())
                    .TryCatch(ex => { _logger.LogError("Updating User:{userId} has failed, {managerId}", ex, args: [id, _sm.User.Id]); return ex.ToDbError(); });

        public async Task<Result> UpdateAsync(JsonPatchDocument<UpdateUserDTO> patchDto)
           => await UpdateUserAsync(_sm.User.Id, patchDto);

        #endregion
        #region Delete

        public async Task<Result> RemoveUsersAsync(IEnumerable<int> IDs, CancellationToken ct = default)
           => await  EnsureValidIDs(IDs)
                    .WithTransaction(_repo, set => _repo.User.ExecuteDeleteAsync(set, ct));

        private async Task<Result> RemoveUserInternal(User user)
           => await Result.Success()
                    .Tap(() => _repo.User.Delete(user))
                    .TapAsync(() => _repo.SaveAsync())
                    .TryCatch(ex => { _logger.LogError("Removing User:{userId} has failed, {managerId}", ex, args: [user.ID, _sm.User.Id]); return ex.ToDbError(); });

        public async Task<Result> RemoveUserAsync(int target)
           => await EnsureValidId(target)
                    .Ensure(_ => target != _sm.User.Id, _ => UserError.SelfDelete)
                    .BindAsync(_ => RetrieveUserByIdAsync(target, withPlan: false))
                    .Ensure(u => u != null, _ => UserError.NotFound(target))
                    .BindAsync(RemoveUserInternal!);

        public async Task<Result> RemoveAsync()
           => await RemoveUserAsync(_sm.User.Id);
        #endregion

    }
}
