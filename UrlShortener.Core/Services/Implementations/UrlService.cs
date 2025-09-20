using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using NLog.Targets;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Resources;
using UrlShortener.Core.Domain.Errors;
using UrlShortener.Core.Domain.Results;
using UrlShortener.Core.Services.Interfaces;
using UrlShortener.Core.Validators;
using UrlShortener.Data.Entities.Plans;
using UrlShortener.Data.Entities.Urls;
using UrlShortener.Data.Entities.Users;
using UrlShortener.Data.Repositories.Implementation;
using UrlShortener.Models.DTOs.Paging;
using UrlShortener.Models.DTOs.Url;
using static UrlShortener.Core.Domain.Errors.Error.UrlError;

namespace UrlShortener.Core.Services.Implementations
{

    public static class UrlDomainExternal
    {

        

        public static FullUrlDTO ToFullDTO(this URL url)
        {
            return new FullUrlDTO
            (
                ID: url.ID,
                Url: "https://www.url.com/" + url.ShortCode,
                ShortCode: url.ShortCode,
                Source: url.Source,
                Title: url.Title ?? "Shortened Link",
                VisitCount: url.VisitCount!.Value,
                CreatedAt: url.CreatedAt!.Value,
                LastModified: url.LastModified!.Value,  
                ExpiresAt: url.ExpiresAt,
                isActive: url.isActive!.Value
            );
        }
    }

    // Seperate Get from just returning FullUrlDTO
    // Get -> bussiness constraint -> URL -> GetSendToController -> FullUrlDTO
    public class UrlService : IUrlService
    {
        private readonly IRepositoryManager       _repo;
        private readonly IServiceManager          _sm;
        private readonly ILoggerManager           _logger;
        public UrlService(IRepositoryManager repo, IServiceManager sm, ILoggerManager logger) 
        {
            _repo = repo;
            _sm = sm;   
            _logger = logger;
        }

        #region HelperMethods
        private Result<bool> EnsureValidID(int urlID)
            => urlID > 0 ? true : Error.BadRequest($"Id must be greater than zero (Parameter 'urlID')");

        private Result<IEnumerable<int>> EnsureValidIDs(IEnumerable<int> urlIDs)
        {
            if (urlIDs.Count() == 0) return Error.BadRequest($"Require one valid ID or more.");

            var errors = new Dictionary<string, List<string>>{{"IDs",[]}};
            var set = new HashSet<int>();

            foreach (int id in urlIDs)
            {
                if (id < 1)
                {
                    errors["IDs"].Add(id.ToString());
                }

                set.Add(id);
            }

            if (errors.Count() - 1 > 0)
                return new ValidationError(errors, "Validation Failed", "One or more url IDs are invalid. Id must be greater than zero.", enErrorType.BadRequest);
            return set;
        }

        private async Task<Result<URL>> InjectShortCode(CreateUrlDTO dto, User user)
        {
            var shortCodeResult = await _sm.ShortCodeService.Generate();
            return shortCodeResult.IsSuccess ? new URL
            {
                Source = dto.Source,
                ShortCode = shortCodeResult.Value,
                Title = dto.Title,
                CreatedBy = _sm.User.Id,
                ExpiresAt = DateTime.UtcNow.Date.AddDays(user.Plan.UrlExpiresAfter),
                isActive = dto.isActive
            } : shortCodeResult.Error;
        }

        private async Task<Result<IEnumerable<URL>>> InjectShortCode(IEnumerable<CreateUrlDTO> dtos, User user)
        {
            var urls = new List<URL>();
            foreach(var dto in dtos)
            {
                var res = await InjectShortCode(dto, user);
                if (res.IsFailure)
                    return res.Error;
                urls.Add(res.Value);
            }
            return urls;
        }

        public async Task RegisterView(Result<URL> url, string userAgent, string visitorIP)
            => await url
              .Tap(_repo.Url.Update)
              .Tap(u => u.VisitCount++)
              .Tap(u => u.Visits.Add(new URLVisit { UrlId = u.ID, UserAgent = userAgent, VisitorIP = visitorIP }))
              .TapAsync(_ => _repo.SaveAsync())
              .TryCatch(ex => { _logger.LogError("Regestiring view failed", ex); return Error.Unexpected; });
        #endregion
        #region Create
        public async Task<Result<FullUrlDTO>> CreateUrlAsync(CreateUrlDTO UrlDTO)
            => await UrlDTO.Validate()
                   .BindAsync(_ => _sm.UserService.RetrieveOwnedUserAsync(withPlan: true))
                   .BindAsync(user => InjectShortCode(UrlDTO, user))
                   .Tap(_repo.Url.Add)
                   .TapAsync(_ => _repo.SaveAsync())
                   .TryCatch(ex => { _logger.LogError("Creating Url has failed, {managerId}", ex, args: [_sm.User.Id]); return ex.ToDbError(); })
                   .Map(url => url.ToFullDTO());


        public async Task<Result<IEnumerable<FullUrlDTO>>> CreateUrlsAsync(IEnumerable<CreateUrlDTO> UrlDTOs, CancellationToken ct = default)
        => await Result<int>.Create(_sm.User.Id)
                .Ensure(_ => UrlDTOs.All(u => u.Validate().IsSuccess), Err => Err)
                .BindAsync(_ => _sm.UserService.RetrieveOwnedUserAsync(withPlan: true))
                .BindAsync(user => InjectShortCode(UrlDTOs, user))
                .Tap(urls => _repo.Url.AddOrUpdate(urls, ct))
                .TapAsync(urls => _repo.SaveAsync())
                .TryCatch(ex => { _logger.LogError("Creating Urls has failed, {managerId}", ex, args: [_sm.User.Id]); return ex.ToDbError(); })
                .Map(urls => urls.Select(u => u.ToFullDTO()));
        #endregion
        #region Read

        // ====== Retrieve URL Entity ====== //
        public async Task<Result<IEnumerable<URL>>> RetrieveAllUrlsAsync(Result<UrlParameters> UrlParam, CancellationToken ct = default)
            => await UrlParam
                    .MapAsync(param => _repo.Url.FetchAllUrlsAsync(param, ct))
                    .TryCatch(ex => { _logger.LogError("Retrieving Urls has failed, {managerId}", ex, args: [_sm.User.Id]); return ex.ToDbError(); });
        public async Task<Result<IEnumerable<URL>>> RetrieveAllOwnedUrlsAsync(Result<UrlParameters> UrlParam, int userId, CancellationToken ct = default)
            => await UrlParam
                    .MapAsync(param => _repo.Url.FetchAllOwnedUrlsAsync(param, userId, ct))
                    .TryCatch(ex => { _logger.LogError("Retrieving Owned Urls has failed, {managerId}", ex, args: [_sm.User.Id]); return ex.ToDbError(); });

        public async Task<Result<IEnumerable<URL?>>> RetrieveUrlsByIdAsync(IEnumerable<int> ids, CancellationToken ct = default)
            => await EnsureValidIDs(ids)
                    .MapAsync(validId => _repo.Url.FetchByIdsAsync(validId, ct))
                    .TryCatch(ex => { _logger.LogError("Retrieving Urls has failed, {managerId}", ex, args: [_sm.User.Id]); return ex.ToDbError(); });
        public async Task<Result<IEnumerable<URL?>>> RetrieveOwnedUrlsByIdAsync(IEnumerable<int> ids, CancellationToken ct = default)
            => await EnsureValidIDs(ids)
                    .MapAsync(validId => _repo.Url.FetchByOwnedIdsAsync(validId, _sm.User.Id, ct))
                    .TryCatch(ex => { _logger.LogError("Retrieving Owned Urls has failed, {managerId}", ex, args: [_sm.User.Id]); return ex.ToDbError(); });

        public async Task<Result<URL?>> RetrieveUrlByIdAsync(int id)
            => await EnsureValidID(id)
                    .MapAsync(_ => _repo.Url.FetchByIdAsync(id))
                    .TryCatch(ex => { _logger.LogError("Retreiving Url:{urlId} has failed, {managerId}", ex, args: [id, _sm.User.Id]); return ex.ToDbError(); });
        public async Task<Result<URL>> RetrieveUrlAndValidateExistenceAsync(int id)
            => (await RetrieveUrlByIdAsync(id).Ensure(url => url != null, _ => NotFound(id)))!;
        public async Task<Result<URL>> RetrieveOwnedUrlByIdAsync(int id)
            => await RetrieveUrlAndValidateExistenceAsync(id)
                    .Ensure(url => url.CreatedBy == _sm.User.Id, _ => Forbidden);


        public async Task<Result<URL?>> RetrieveUrlByShortCode(Result<string> shortCode)
            => (await shortCode.MapAsync(sc => _repo.Url.GetByShortCode(sc)))!;

        // ====== Mapping Entity To DTO ====== //

        public async Task<Result<PagedList<FullUrlDTO>>> RetrieveAllUrlsWithPagingAsync(UrlParameters UrlParam, CancellationToken ct = default)
            => await RetrieveAllUrlsAsync(UrlParam, ct).Map(urls => PagedList<FullUrlDTO>.ToPagedList([..urls.Select(u => u.ToFullDTO())], UrlParam.PageNumber, UrlParam.PageSize));
        public async Task<Result<PagedList<FullUrlDTO>>> RetrieveAllOwnedUrlsWithPagingAsync(UrlParameters UrlParam, CancellationToken ct = default)
            => await RetrieveAllOwnedUrlsAsync(UrlParam, _sm.User.Id, ct).Map(urls => PagedList<FullUrlDTO>.ToPagedList([.. urls.Select(u => u.ToFullDTO())], UrlParam.PageNumber, UrlParam.PageSize));

        public async Task<Result<IEnumerable<FullUrlDTO>>> RetrieveUrlsWithDTOAsync(IEnumerable<int> ids, CancellationToken ct = default)
            => await RetrieveUrlsByIdAsync(ids, ct).Map(urls => urls.Select(u => u.ToFullDTO()));
        public async Task<Result<IEnumerable<FullUrlDTO>>> RetrieveOwnedUrlsWithDTOAsync(IEnumerable<int> ids, CancellationToken ct = default)
            => await RetrieveOwnedUrlsByIdAsync(ids, ct).Map(urls => urls.Select(u => u.ToFullDTO()));

        public async Task<Result<FullUrlDTO>> RetrieveUrlWithDTOAsync(int id)
            => await RetrieveUrlAndValidateExistenceAsync(id).Map(url => url.ToFullDTO());
        public async Task<Result<FullUrlDTO>> RetrieveOwnedUrlWithDTOAsync(int id) 
            => await RetrieveOwnedUrlByIdAsync(id).Map(url => url.ToFullDTO());


        public async Task<Result<FullUrlDTO>> RetrieveUrlByShortCodeWithDTO(string shortCode)
            => await RetrieveUrlByShortCode(shortCode)
                    .Ensure(url => url != null, err => NotFound(shortCode))
                    .Map(url => url!.ToFullDTO());

        #endregion
        #region Update
        // = Update Methods = //

        public async Task<Result<FullUrlDTO>> UpdateUrlAsync(int id, JsonPatchDocument<UpdateUrlDTO> dto)
        {
            throw new NotImplementedException();
        }

        #endregion
        #region Delete
        public async Task<Result> RemoveByIDsAsync(IEnumerable<int> ids, CancellationToken ct = default)
        => await EnsureValidIDs(ids)
                .TapAsync(validIDs => _repo.Url.ExecuteDeleteAsync(validIDs, ct))
                .TryCatch(ex => { _logger.LogError("Removing Urls has failed, {managerId}", ex, args: [_sm.User.Id]); return ex.ToDbError(); });

        public async Task<Result> RemoveUrlAsync(int id)
            => await EnsureValidID(id)
               .BindAsync(_ => RetrieveUrlAndValidateExistenceAsync(id))
               .Tap(_repo.Url.Delete)
               .TapAsync(_=>_repo.SaveAsync())
               .TryCatch(ex => { _logger.LogError("Removing Url:{urlId] has failed, {managerId}", ex, args: [id, _sm.User.Id]); return ex.ToDbError(); });
        #endregion

    }
}
