using UrlShortener.Core.Domain.Results;

namespace UrlShortener.Core.Services.Interfaces
{
    public interface IShortCodeService
    {
        Task<Result<string>> Generate();
    }
}
