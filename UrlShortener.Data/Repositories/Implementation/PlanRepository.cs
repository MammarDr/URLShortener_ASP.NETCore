using Azure;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using UrlShortener.Data.Entities.Plans;
using UrlShortener.Data.Repositories.Extensions;
using UrlShortener.Data.Repositories.Implementation;
using UrlShortener.Data.Repositories.Interfaces;
using UrlShortener.Models.DTOs.Plan;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace UrlShortener.Data.Repositories
{

    public class PlanRepository(AppDbContext _context) : RepositoryBase<Plan>(_context), IPlanRepository
    {

        public async Task<IEnumerable<Plan>> GetAllPlansAsync(PlanParameters planParam, CancellationToken ct, bool asNoTracking = false)
            => await FindAll(asNoTracking)
                .PriceFilter(planParam.MinPrice, planParam.MaxPrice)
                .CustomSlugFilter(planParam.HasCustomSlugs)
                .SupportLevelFilter(planParam.SupportLevels!)
                .Search(planParam.SearchTerm!)
                .Sort(planParam.OrderBy!)
                .Skip((planParam.PageNumber - 1) * planParam.PageSize)
                .Take(planParam.PageSize)      
                .ToListAsync();

        public async Task<Plan?> GetByIdAsync(int id, bool asNoTracking = false)
            => await FindByConditon(plan => plan.ID == id).FirstOrDefaultAsync();

        public async Task<IEnumerable<Plan?>> GetByIDsAsync(IEnumerable<int> IDs, CancellationToken ct,  bool asNoTracking = false)
            => await FindAll(asNoTracking).Where(plan => IDs.Contains(plan.ID)).ToListAsync();

        public void AddOrUpdate(IEnumerable<Plan> plans, CancellationToken ct)
        {
            foreach (var plan in plans)
            {
                if(plan.ID == 0)
                    Add(plan);
                else
                    Update(plan);
            }
        }

        public async Task ExecuteDeleteAsync(IEnumerable<int> plans, CancellationToken ct)
        {
            await _context.Plans.Where(p => plans.Contains(p.ID)).ExecuteDeleteAsync(ct);
        }

        public async Task<bool> ExistsAnyAsync(Expression<Func<Plan, bool>> expression)
            => await FindByConditon(expression).Select(p => p.ID).AnyAsync();

        public async Task<int> CountAsync(Expression<Func<Plan, bool>> expression)
            => await FindByConditon(expression).CountAsync();
    }
}
