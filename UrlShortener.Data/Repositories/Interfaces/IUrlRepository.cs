using System.Linq.Expressions;
using UrlShortener.Data.Entities.Urls;
using UrlShortener.Data.Repositories.Implementation;
using UrlShortener.Models.DTOs.Url;

namespace UrlShortener.Data.Repositories.Interfaces
{

  
    public interface IUrlRepository : IRepositoryBase<URL>
    {
        void AddOrUpdate(IEnumerable<URL> Urls, CancellationToken ct);
        Task ExecuteDeleteAsync(IEnumerable<int> Urls, CancellationToken ct);
        Task<bool> ExistsAnyAsync(Expression<Func<URL, bool>> expression);
        Task<int> CountAsync(Expression<Func<URL, bool>> expression);

        Task<IEnumerable<URL>> FetchAllUrlsAsync(UrlParameters UrlParam, CancellationToken ct, bool asNoTracking = false);
        Task<IEnumerable<URL>> FetchAllOwnedUrlsAsync(UrlParameters urlParam, int id, CancellationToken ct, bool asNoTracking = false);

        Task<URL?> FetchByIdAsync(int id, bool asNoTracking = false);
        Task<URL?> FetchByOwnedIdAsync(int urlId, int userId, bool asNoTracking = false);
       
        Task<IEnumerable<URL?>> FetchByIdsAsync(IEnumerable<int> IDs, CancellationToken ct, bool asNoTracking = false);
        Task<IEnumerable<URL?>> FetchByOwnedIdsAsync(IEnumerable<int> IDs, int userId, CancellationToken ct, bool asNoTracking = false);
        Task<URL> GetByShortCode(string shortCode, bool AsNoTrackable = false);
    }
}
