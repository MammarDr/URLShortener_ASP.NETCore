
namespace UrlShortener.Models.DTOs.User
{
    public record AccessTokenDTO(string AccessToken, string RefreshToken);
    public record RefreshTokenDTO(string RefreshToken);
}
