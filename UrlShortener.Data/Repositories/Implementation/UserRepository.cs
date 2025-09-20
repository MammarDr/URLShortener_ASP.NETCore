using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UrlShortener.Data.Entities.Plans;
using UrlShortener.Data.Entities.Users;
using UrlShortener.Data.Repositories.Extensions;
using UrlShortener.Data.Repositories.Interfaces;
using UrlShortener.Models.DTOs.User;

namespace UrlShortener.Data.Repositories.Implementation
{

    public class UserRepository(AppDbContext _context) : RepositoryBase<User>(_context), IUserRepository
    {

        public async Task<IEnumerable<User>> GetAllUsersAsync(UserParameters userParam, CancellationToken ct, bool withPlan = false, bool asNoTracking = false)
            => await FindAll(asNoTracking)
                .PlanFilter(userParam.PlanId)
                .MaxDateFilter(userParam.MaxDate)
                .MinDateFilter(userParam.MinDate)
                .Search(userParam.SearchTerm!)
                .Sort(userParam.OrderBy!)
                .Skip((userParam.PageNumber - 1) * userParam.PageSize)
                .Take(userParam.PageSize)
                .IncludePlan(withPlan)
                .ToListAsync();

        public async Task<User?> GetByIdAsync(int id, bool withPlan = false, bool asNoTracking = false)
            => await FindByConditon(user => user.ID == id, asNoTracking).IncludePlan(withPlan).FirstOrDefaultAsync();
        public async Task<User?> GetByEmailAsync(string email, bool withPlan = false, bool asNoTracking = false)
            => await FindByConditon(user => user.Email == email, asNoTracking).IncludePlan(withPlan).FirstOrDefaultAsync();

        public async Task<IEnumerable<User?>> GetByIDsAsync(IEnumerable<int> IDs, CancellationToken ct, bool withPlan = false, bool asNoTracking = false)
            => await FindByConditon(user => IDs.Contains(user.ID), asNoTracking).IncludePlan(withPlan).ToListAsync();

        public void AddOrUpdate(IEnumerable<User> users, CancellationToken ct)
        {
            foreach (var user in users)
            {
                if (user.ID == 0)
                    Add(user);
                else
                    Update(user);
            }
        }

        public async Task ExecuteDeleteAsync(IEnumerable<int> users, CancellationToken ct)
            => await FindByConditon(p => users.Contains(p.ID)).ExecuteDeleteAsync(ct);

        public async Task<bool> ExistsAnyAsync(Expression<Func<User, bool>> expression)
            => await FindByConditon(expression).Select(u => u.ID).AnyAsync();

        public async Task<int> CountAsync(Expression<Func<User, bool>> expression)
            => await FindByConditon(expression).CountAsync();

    }


}
