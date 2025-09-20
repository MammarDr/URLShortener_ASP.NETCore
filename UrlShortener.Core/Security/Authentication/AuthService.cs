using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Core.Domain.Results;
using UrlShortener.Core.Security.Identity;
using UrlShortener.Core.Services.Interfaces;
using UrlShortener.Data.DbContext.Configuration;
using UrlShortener.Data.Entities.Permissions;
using UrlShortener.Data.Entities.Users;
using UrlShortener.Data.Repositories.Implementation;
using UrlShortener.Data.Repositories.Interfaces;
using UrlShortener.Models.DTOs.User;
using UrlShortener.Models.Enums;
using static UrlShortener.Core.Domain.Errors.Error;

namespace UrlShortener.Core.Security.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly IRepositoryManager _repo;
        private readonly IServiceManager _sm;
        private readonly IPasswordHasher<User> _hasher = new PasswordHasher<User>();
        private readonly int RefreshTokenAddDays = 7;

        public AuthService(IRepositoryManager repository, IServiceManager serviceManager)
        {
            _repo = repository;
            _sm = serviceManager;
        }

        private RefreshToken CreateRefreshToken(string refreshToken)
            => CreateRefreshToken(refreshToken, _sm.User.Id);
        private RefreshToken CreateRefreshToken(string refreshToken, int id)
            => new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = refreshToken,
                UserId = id,
                ExpiresOnUtc = DateTime.UtcNow.AddDays(RefreshTokenAddDays)
            };

        private async Task<Result<AccessTokenDTO>> CreateTokenAsync(User user)
        {

            var permsResult = await _sm.PermissionService.GetPermissionsAsync(user.ID);
            if (permsResult.IsFailure)
                return permsResult.Error;

            return _sm.TokenService.CreateToken(user, permsResult.Value);
        }

        public async Task<Result<User?>> isAuthenticatedAsync(User user, AuthenticationRequest dto)
        {
            if (user is null) return user;

            var result = _hasher.VerifyHashedPassword(user, user.Password, dto.Password);
            if (result == PasswordVerificationResult.Failed)
                return null;

            return user;
        }

        public async Task<Result<AccessTokenDTO>> AuthenticateAsync(User user, AuthenticationRequest dto)
        {
            if (user is null)
                return UserError.InvalidCredentials;

            var result = _hasher.VerifyHashedPassword(user, user.Password, dto.Password);
            if (result == PasswordVerificationResult.Failed)
                return UserError.InvalidCredentials;

            var tokenResult = await CreateTokenAsync(user);
            if(tokenResult.IsFailure)
                return tokenResult.Error;

            _repo.RefreshToken.Add(CreateRefreshToken(tokenResult.Value.RefreshToken, user.ID));

            await _repo.SaveAsync();

            return tokenResult;

        }

        public async Task<Result<AccessTokenDTO>> RefreshTokenAsync(string RefreshToken)
        {
            var token = await _repo.RefreshToken.FindByConditon(r => r.Token == RefreshToken)
                                    .Include(u => u.User).FirstOrDefaultAsync();

            if(token == null || token.ExpiresOnUtc < DateTime.UtcNow)
                return TokenError.ExpiredToken;

            var tokenResult = await CreateTokenAsync(token.User);

            token.Token = tokenResult.Value.RefreshToken;
            token.ExpiresOnUtc = DateTime.UtcNow.AddDays(RefreshTokenAddDays);

            await _repo.SaveAsync();    

            return tokenResult;
        }
    }
}
