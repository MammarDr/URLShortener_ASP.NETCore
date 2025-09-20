using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using UrlShortener.Data.Entities.Urls;
using UrlShortener.Data.Repositories.Extensions;
using UrlShortener.Data.Repositories.Interfaces;
using UrlShortener.Models.DTOs.Url;

namespace UrlShortener.Data.Repositories.Implementation
{
    // Page 55
    public class UrlRepository(AppDbContext _context) : RepositoryBase<URL>(_context), IUrlRepository
    {
  
        public async Task<URL> GetByShortCode(string shortCode, bool AsNoTrackable = false)
           => (await (AsNoTrackable ? _context.Urls.AsNoTracking() : _context.Urls).FirstOrDefaultAsync(url => url.ShortCode == shortCode))!;

        private IQueryable<URL> FetchWithParameter(UrlParameters urlParam, CancellationToken ct, bool asNoTracking = false)
             =>  FindAll(asNoTracking)
                     .MaxVisitFilter(urlParam.MaxVisitCount)
                     .MinVisitFilter(urlParam.MinVisitCount)
                     .MaxDateFilter(urlParam.MaxDate)
                     .MinDateFilter(urlParam.MinDate)
                     .Search(urlParam.SearchTerm!)
                     .Sort(urlParam.OrderBy!)
                     .Skip((urlParam.PageNumber - 1) * urlParam.PageSize)
                     .Take(urlParam.PageSize);

        public async Task<IEnumerable<URL>> FetchAllUrlsAsync(UrlParameters urlParam, CancellationToken ct, bool asNoTracking = false)
              => await FetchWithParameter(urlParam, ct, asNoTracking)
                      .ToListAsync();

        public async Task<IEnumerable<URL>> FetchAllOwnedUrlsAsync(UrlParameters urlParam, int userId, CancellationToken ct, bool asNoTracking = false)
              => await FetchWithParameter(urlParam, ct, asNoTracking)
                      .OwnedBy(userId)
                      .ToListAsync();

        public async Task<URL?> FetchByIdAsync(int id, bool asNoTracking = false)
            => await FindByConditon(url => url.ID == id).FirstOrDefaultAsync();
        public async Task<URL?> FetchByOwnedIdAsync(int urlId, int userId, bool asNoTracking = false)
            => await FindByConditon(url => url.CreatedBy == userId && url.ID == urlId).FirstOrDefaultAsync();

        public async Task<IEnumerable<URL?>> FetchByIdsAsync(IEnumerable<int> IDs, CancellationToken ct, bool asNoTracking = false)
            => await FindAll(asNoTracking).Where(url => IDs.Contains(url.ID)).ToListAsync();
        public async Task<IEnumerable<URL?>> FetchByOwnedIdsAsync(IEnumerable<int> IDs, int userId, CancellationToken ct, bool asNoTracking = false)
           => await FindAll(asNoTracking).Where(url => url.CreatedBy == userId && IDs.Contains(url.ID)).ToListAsync();

        public void AddOrUpdate(IEnumerable<URL> urls, CancellationToken ct)
        {
            foreach (var url in urls)
            {
                if (url.ID == 0)
                    Add(url);
                else
                    Update(url);
            }
        }

        public async Task ExecuteDeleteAsync(IEnumerable<int> urls, CancellationToken ct)
            => await _context.Urls.Where(p => urls.Contains(p.ID)).ExecuteDeleteAsync(ct);
        

        public async Task<bool> ExistsAnyAsync(Expression<Func<URL, bool>> expression)
            => await FindByConditon(expression).Select(p => p.ID).AnyAsync();

        public async Task<int> CountAsync(Expression<Func<URL, bool>> expression)
            => await FindByConditon(expression).CountAsync();


    }
}
