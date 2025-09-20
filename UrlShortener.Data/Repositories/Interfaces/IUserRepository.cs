
using System.Linq.Expressions;
using UrlShortener.Data.Entities.Users;
using UrlShortener.Data.Repositories.Implementation;
using UrlShortener.Models.DTOs.User;

namespace UrlShortener.Data.Repositories.Interfaces
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        void AddOrUpdate(IEnumerable<User> users,  CancellationToken ct);
        Task<int> CountAsync(Expression<Func<User, bool>> expression);
        Task ExecuteDeleteAsync(IEnumerable<int> users, CancellationToken ct);
        Task<bool> ExistsAnyAsync(Expression<Func<User, bool>> expression);
        Task<IEnumerable<User>> GetAllUsersAsync(UserParameters userParam, CancellationToken ct, bool withPlan = false, bool asNoTracking = false);
        Task<User?> GetByEmailAsync(string email, bool withPlan = false, bool asNoTracking = false);
        Task<User?> GetByIdAsync(int id, bool withPlan = false, bool asNoTracking = false);
        Task<IEnumerable<User?>> GetByIDsAsync(IEnumerable<int> IDs, CancellationToken ct, bool withPlan = false, bool asNoTracking = false);
    }
}
