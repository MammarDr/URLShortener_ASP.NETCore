using Microsoft.EntityFrameworkCore;
using UrlShortener.Data.Entities.Users;

namespace UrlShortener.Data.Repositories.Extensions
{
    public static class UserIQuaryableExtensions
    {
        public static IQueryable<User> PlanFilter(this IQueryable<User> user, IEnumerable<int> PlanIDs)
            => user.Where(u => PlanIDs.Contains(u.PlanId));
        public static IQueryable<User> IncludePlan(this IQueryable<User> user, bool withPlan)
            => withPlan ? user.Include(u => u.Plan) : user;

        public static IQueryable<User> MaxDateFilter(this IQueryable<User> user, DateTime? MaxDate)
            => MaxDate.HasValue ? user.Where(u => u.CreatedAt <= MaxDate.Value) : user;

        public static IQueryable<User> MinDateFilter(this IQueryable<User> user, DateTime? MinDate)
            => MinDate.HasValue ? user.Where(u => u.CreatedAt >= MinDate.Value) : user;

        public static IQueryable<User> Search(this IQueryable<User> user, string search)
        {
            if(string.IsNullOrEmpty(search))
                return user;
            return user.Where(u => u.Email == search.Trim().ToLower());
        }

        public static IQueryable<User> Sort(this IQueryable<User> user, string orderBy)
            => IQueryExtension<User>.Sort(user, orderBy);

        
    }

}
