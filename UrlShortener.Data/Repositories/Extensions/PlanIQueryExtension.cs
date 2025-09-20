using Microsoft.EntityFrameworkCore;
using UrlShortener.Data.Entities.Plans;
using UrlShortener.Models.Enums;


namespace UrlShortener.Data.Repositories.Extensions
{
    public static class PlanIQueryExtension
    {
        public static IQueryable<Plan> PriceFilter(this IQueryable<Plan> plan, uint minAge, uint maxAge)
            => plan.Where(p => p.Price > minAge && p.Price <= maxAge);

        public static IQueryable<Plan> CustomSlugFilter(this IQueryable<Plan> plan, bool? HasCustomSlugs)
            => HasCustomSlugs.HasValue ? plan.Where(p => p.HasCustomSlugs == HasCustomSlugs) : plan;

        public static IQueryable<Plan> SupportLevelFilter(this IQueryable<Plan> plan, IEnumerable<enSupportLevel> supportLevels)
            => plan.Where(p => supportLevels!.Contains(p.SupportLevel));

        public static IQueryable<Plan> Search(this IQueryable<Plan> plan, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return plan;
            var lowerCaseTerm = searchTerm.Trim().ToLower();
            return plan.Where(p => p.Name.ToLower().Contains(lowerCaseTerm));
        }

        public static IQueryable<Plan> Sort(this IQueryable<Plan> plan, string orderByQueryString)
            => IQueryExtension<Plan>.Sort(plan, orderByQueryString);

        /*public static IQueryable<Plan> Sort(this IQueryable<Plan> plan, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return plan.OrderBy(e => e.Name);

            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(Plan).GetProperties(BindingFlags.Public |  BindingFlags.Instance);
  
            IOrderedQueryable<Plan> orderedQueryable = null!;
            var parameter = Expression.Parameter(typeof(Plan), "p");

            foreach (var param in orderParams)
            {
                if (string.IsNullOrWhiteSpace(param))
                    continue;

                var propertyFromQueryName = param.Split(" ")[0];

                // StringComparison.InvariantCultureIgnoreCase : Ignore (A-a), accept(I-İ)
                var objectProperty = propertyInfos.FirstOrDefault(pi => pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));

                if (objectProperty == null)
                    continue;

                var memberExpr = Expression.PropertyOrField(parameter, objectProperty.Name);
                var converter = Expression.Convert(memberExpr, typeof(object));
                var lamdba = Expression.Lambda<Func<Plan, object>>(converter, parameter);

                if (param.EndsWith(" desc"))
                    orderedQueryable = orderedQueryable is null ? plan.OrderByDescending(lamdba) : orderedQueryable.ThenByDescending(lamdba);
                else
                    orderedQueryable = orderedQueryable is null ? plan.OrderBy(lamdba) : orderedQueryable.ThenBy(lamdba);

            }


            if (orderedQueryable is null)
                return plan.OrderBy(p => p.Name);

            return orderedQueryable;
        }*/


    }
}
