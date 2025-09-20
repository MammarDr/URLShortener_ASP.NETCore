using System.Linq.Expressions;
using UrlShortener.Data.Entities.Plans;
using UrlShortener.Data.Repositories.Implementation;
using UrlShortener.Models.DTOs.Plan;

namespace UrlShortener.Data.Repositories.Interfaces
{
    public interface IPlanRepository : IRepositoryBase<Plan>
    {
        void AddOrUpdate(IEnumerable<Plan> plans, CancellationToken ct);
        Task ExecuteDeleteAsync(IEnumerable<int> plans, CancellationToken ct);
        Task<IEnumerable<Plan>> GetAllPlansAsync(PlanParameters planParam, CancellationToken ct, bool asNoTracking = false);
        Task<Plan?> GetByIdAsync(int id, bool asNoTracking = false);
        Task<IEnumerable<Plan?>> GetByIDsAsync(IEnumerable<int> IDs, CancellationToken ct, bool asNoTracking = false);
        Task<bool> ExistsAnyAsync(Expression<Func<Plan, bool>> expression);
        Task<int> CountAsync(Expression<Func<Plan, bool>> expression);
        
    }
}
