using UrlShortener.Core.Domain.Results;
using UrlShortener.Data.Entities.Users;
using UrlShortener.Models.DTOs.User;
using UrlShortener.Models.Enums;

namespace UrlShortener.Core.Security.Authentication
{
    public interface ITokenService
    {
        Result<AccessTokenDTO> CreateToken(User user, IReadOnlyDictionary<enResource, HashSet<enPermission>> permissions);
    }
}
