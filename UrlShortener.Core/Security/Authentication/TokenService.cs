using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UrlShortener.Core.Domain.Results;
using UrlShortener.Core.Security.Authentication;
using UrlShortener.Core.Services.Interfaces;
using UrlShortener.Data.Entities.Users;
using UrlShortener.Models.DTOs.User;
using UrlShortener.Models.Enums;
using static UrlShortener.Core.Domain.Errors.Error;

namespace UrlShortener.Core.Services.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _options;
        private readonly ILoggerManager _logger;

        public TokenService(IOptions<JwtOptions> options, ILoggerManager logger)
        {
            _options = options.Value;
            _logger = logger;
        }


        private Result<string> GenerateTokenOptions(User user, List<Claim> claims)
        {
            try
            {
                DotNetEnv.Env.Load();
                var signingKey = Environment.GetEnvironmentVariable("Jwt__SigningKey");
                if (signingKey is null)
                {
                    _logger.LogCritical("Retreiving JWT SigningKey has failed for {UserId}", args: [user.ID]);
                    return TokenError.SigningKeyMissing;
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _options.Issuer,
                    audience: _options.Audiences,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(_options.Lifetime + 120),
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Exception thrown on TokenService.GenerateTokenOptions for {userId}", ex, args: [user.ID]);
                return TokenError.Unexpected;
            }
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }

        private Result<List<Claim>> GetClaims(User user, IReadOnlyDictionary<enResource, HashSet<enPermission>> permissions)
        {
            try
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

                foreach (var kv in permissions)
                {
                    claims.AddRange(kv.Value.Select(p => new Claim(
                        $"perm.{kv.Key}",
                        $"{p}"
                        )
                    ));
                }

                return claims;
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Exception thrown on TokenService.GetClaims for {userId}", ex, args: [user.ID]);
                return TokenError.Unexpected;   
            }
            
        }


        public Result<AccessTokenDTO> CreateToken(User user, IReadOnlyDictionary<enResource, HashSet<enPermission>> permissions)
        {
            if (user is null || permissions is null)
                return TokenError.InvalidCredentials;

            var claimsResult = GetClaims(user, permissions);
            if(claimsResult.IsFailure)
                return claimsResult.Error;

            var tokenResult = GenerateTokenOptions(user, claimsResult.Value);
            if(tokenResult.IsFailure)
                return tokenResult.Error;

            var refreshToken = GenerateRefreshToken();

            return new AccessTokenDTO(tokenResult.Value, refreshToken);
        }
    }
}
