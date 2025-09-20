using UrlShortener.Core.Domain.Results;
using UrlShortener.Data.Entities.Users;
using UrlShortener.Models.DTOs.User;

namespace UrlShortener.Core.Security.Authentication
{
    public interface IAuthService
    {
        Task<Result<User?>> isAuthenticatedAsync(User user, AuthenticationRequest dto);
        Task<Result<AccessTokenDTO>> AuthenticateAsync(User user, AuthenticationRequest dto);
        Task<Result<AccessTokenDTO>> RefreshTokenAsync(string RefreshToken);
    }

}
