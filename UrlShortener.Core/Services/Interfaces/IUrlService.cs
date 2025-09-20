using Microsoft.AspNetCore.JsonPatch;
using UrlShortener.Core.Domain.Results;
using UrlShortener.Data.Entities.Urls;
using UrlShortener.Models.DTOs;
using UrlShortener.Models.DTOs.Paging;
using UrlShortener.Models.DTOs.Url;
using UrlShortener.Models.DTOs.Url;

namespace UrlShortener.Core.Services.Interfaces
{
    public interface IUrlService
    {
        Task<Result<IEnumerable<URL>>>          RetrieveAllUrlsAsync(Result<UrlParameters> UrlParam, CancellationToken ct = default); // ==> Call Data-Layer
        Task<Result<PagedList<FullUrlDTO>>>     RetrieveAllUrlsWithPagingAsync(UrlParameters UrlParam, CancellationToken ct = default);

        Task<Result<IEnumerable<URL>>>          RetrieveAllOwnedUrlsAsync(Result<UrlParameters> UrlParam, int userId, CancellationToken ct = default); // ==> Call Data-Layer
        Task<Result<PagedList<FullUrlDTO>>>     RetrieveAllOwnedUrlsWithPagingAsync(UrlParameters UrlParam, CancellationToken ct = default); 

        Task<Result<IEnumerable<URL?>>>         RetrieveOwnedUrlsByIdAsync(IEnumerable<int> ids, CancellationToken ct = default); // ==> Call Data-Layer
        Task<Result<IEnumerable<FullUrlDTO>>>   RetrieveOwnedUrlsWithDTOAsync(IEnumerable<int> ids, CancellationToken ct = default);

        Task<Result<IEnumerable<URL?>>>         RetrieveUrlsByIdAsync(IEnumerable<int> ids, CancellationToken ct = default); // ==> Call Data-Layer
        Task<Result<IEnumerable<FullUrlDTO>>>   RetrieveUrlsWithDTOAsync(IEnumerable<int> ids, CancellationToken ct = default);

        Task<Result<URL>>                       RetrieveOwnedUrlByIdAsync(int id); // ==> Call Data-Layer
        Task<Result<FullUrlDTO>>                RetrieveOwnedUrlWithDTOAsync(int id);

        Task<Result<URL?>>                      RetrieveUrlByIdAsync(int id); // ==> Call Data-Layer
        Task<Result<URL>>                       RetrieveUrlAndValidateExistenceAsync(int id);
        Task<Result<FullUrlDTO>>                RetrieveUrlWithDTOAsync(int id);

        Task<Result<URL?>>                      RetrieveUrlByShortCode(Result<string> shortCode);
        Task<Result<FullUrlDTO>>                RetrieveUrlByShortCodeWithDTO(string shortCode);

        Task<Result<FullUrlDTO>>                CreateUrlAsync(CreateUrlDTO UrlDTO);
        Task<Result<IEnumerable<FullUrlDTO>>>   CreateUrlsAsync(IEnumerable<CreateUrlDTO> UrlDTOs, CancellationToken ct = default);

        Task<Result>                            RemoveByIDsAsync(IEnumerable<int> ids, CancellationToken ct = default);
        Task<Result>                            RemoveUrlAsync(int id);

        Task<Result<FullUrlDTO>>                UpdateUrlAsync(int id, JsonPatchDocument<UpdateUrlDTO> dto);
        Task                                    RegisterView(Result<URL> url, string userAgent, string visitorIP);
    }
}
