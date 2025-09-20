using UrlShortener.Data.Repositories.Interfaces;

namespace UrlShortener.Data.Repositories.Implementation
{
    public interface IRepositoryManager
    {
        IUserRepository User { get; }
        IUrlRepository Url { get; }
        IPermissionRepository Permission { get; }
        IPlanRepository Plan { get; }

        IRefreshTokenRepository RefreshToken { get; }

        Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync();

        Task SaveAsync(CancellationToken ct = default);

    }

}
