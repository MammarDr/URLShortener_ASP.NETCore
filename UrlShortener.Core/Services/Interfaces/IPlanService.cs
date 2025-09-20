using Microsoft.AspNetCore.JsonPatch;
using UrlShortener.Core.Domain.Results;
using UrlShortener.Data.Entities.Plans;
using UrlShortener.Models.DTOs.Paging;
using UrlShortener.Models.DTOs.Plan;

namespace UrlShortener.Core.Services.Interfaces
{
    public interface IPlanService
    {
        
        Task<Result<IEnumerable<Plan>>>          RetrieveAllPlansAsync(Result<PlanParameters> PlanParam, bool asNoTracking, CancellationToken ct = default); // ==> Call Data-Layer
        Task<Result<PagedList<FullPlanDTO>>>     RetrieveAllPlansWithPagingAsync(PlanParameters PlanParambool, bool asNoTracking = true, CancellationToken ct = default);

        Task<Result<IEnumerable<Plan?>>>         RetrievePlansByIdAsync(IEnumerable<int> ids, bool asNoTracking, CancellationToken ct = default); // ==> Call Data-Layer
        Task<Result<IEnumerable<FullPlanDTO>>>   RetrievePlansWithDTOAsync(IEnumerable<int> ids, bool asNoTracking = true, CancellationToken ct = default);

        Task<Result<Plan?>>                      RetrievePlanByIdAsync(int id); // ==> Call Data-Layer
        Task<Result<Plan>>                       RetrievePlanAndValidateExistenceAsync(int id);
        Task<Result<FullPlanDTO>>                RetrievePlanWithDTOAsync(int id);

    

        Task<Result<FullPlanDTO>> CreatePlanAsync(CreatePlanDTO planDTO);
        Task<Result<IEnumerable<FullPlanDTO>>> CreatePlansAsync(IEnumerable<CreatePlanDTO> planDTOs, CancellationToken ct = default);

        Task<Result> DeleteByIDsAsync(IEnumerable<int> ids, CancellationToken ct = default);
        Task<Result> DeletePlanAsync(int id);

        Task<Result<FullPlanDTO>> UpdatePlanAsync(int id, JsonPatchDocument<UpdatePlanDTO> dto);
    }
}
