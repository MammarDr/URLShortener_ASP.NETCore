using UrlShortener.Core.Domain.Errors;
using UrlShortener.Core.Domain.Results;
using UrlShortener.Core.Services.Interfaces;
using UrlShortener.Data.Repositories.Implementation;
using UrlShortener.Data.Repositories.Interfaces;
using UrlShortener.Models.Utility;

namespace UrlShortener.Core.Services.Implementations
{
    public class ShortCodeService(IRepositoryManager _repo, ILoggerManager _logger) : IShortCodeService
    {

        public async Task<Result<string>> Generate()
        {
            int count = 1;
            int tries = 10;

           while(count <= tries)
            {
                var sc = ShortCodeGenerator.Get();
               if (!await _repo.Url.ExistsAnyAsync(url => url.ShortCode == sc))
                    return sc;
            }

            _logger.LogCritical("ShortCode Generating failed due many conflication.");
            return Error.UrlError.ShortCode(tries);
        }
    }
}
