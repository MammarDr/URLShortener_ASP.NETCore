
using UrlShortener.Data.Entities.Urls;


namespace UrlShortener.Data.Repositories.Extensions
{
    public static class UrlIQuaryableExtensions
    {
        public static IQueryable<URL> MaxVisitFilter(this IQueryable<URL> url, int? MaxVisit)
            => MaxVisit.HasValue ? url.Where(u => u.VisitCount <= MaxVisit.Value) : url;

        public static IQueryable<URL> MinVisitFilter(this IQueryable<URL> url, int? MinVisit)
            => MinVisit.HasValue ? url.Where(u => u.VisitCount >= MinVisit.Value) : url;

        public static IQueryable<URL> MaxDateFilter(this IQueryable<URL> url, DateTime? MaxDate)
            => MaxDate.HasValue ? url.Where(u => u.CreatedAt <= MaxDate.Value) : url;

        public static IQueryable<URL> MinDateFilter(this IQueryable<URL> url, DateTime? MinDate)
            => MinDate.HasValue ? url.Where(u => u.CreatedAt >= MinDate.Value) : url;

        public static IQueryable<URL> Search(this IQueryable<URL> url, string search)
        {
            if (string.IsNullOrEmpty(search))
                return url;
            return url.Where(u => u.ShortCode == search.Trim().ToLower());
        }

        public static IQueryable<URL> Sort(this IQueryable<URL> url, string orderBy)
            => IQueryExtension<URL>.Sort(url, orderBy);

        public static IQueryable<URL> OwnedBy(this IQueryable<URL> url, int id)
            => url.Where(u => u.CreatedBy == id);
    }

}
