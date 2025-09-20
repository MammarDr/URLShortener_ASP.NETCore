using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Data.Entities.Plans;

namespace UrlShortener.Data.Repositories.Extensions
{
    public static class IQueryExtension<T>
    {
        public static IQueryable<T> Sort(IQueryable<T> query, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return query;

            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            IOrderedQueryable<T> orderedQueryable = null!;
            var parameter = Expression.Parameter(typeof(T), "t");

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
                var lamdba = Expression.Lambda<Func<T, object>>(converter, parameter);

                if (param.EndsWith(" desc"))
                    orderedQueryable = orderedQueryable is null ? query.OrderByDescending(lamdba) : orderedQueryable.ThenByDescending(lamdba);
                else
                    orderedQueryable = orderedQueryable is null ? query.OrderBy(lamdba) : orderedQueryable.ThenBy(lamdba);

            }


            if (orderedQueryable is null)
                return query;

            return orderedQueryable;
        }
    }
}
