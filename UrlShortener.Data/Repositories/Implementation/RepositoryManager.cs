using Microsoft.EntityFrameworkCore.Storage;
using UrlShortener.Data.Entities.Users;
using UrlShortener.Data.Repositories.Interfaces;

namespace UrlShortener.Data.Repositories.Implementation
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly AppDbContext _context;

        private readonly Lazy<IUserRepository> _userRepo;
        private readonly Lazy<IUrlRepository> _urlRepo;
        private readonly Lazy<IPermissionRepository> _permissionRepo;
        private readonly Lazy<IPlanRepository> _planRepo;
        private readonly Lazy<IRefreshTokenRepository> _refreshTokenRepo;

        public RepositoryManager(
            AppDbContext context,
            Lazy<IUserRepository> userRepo,
            Lazy<IUrlRepository> urlRepo,
            Lazy<IPermissionRepository> permissionRepo,
            Lazy<IPlanRepository> planRepo,
            Lazy<IRefreshTokenRepository> refreshTokenRepo)
        {
            _context = context;

            _userRepo = userRepo;
            _urlRepo = urlRepo;
            _permissionRepo = permissionRepo;
            _planRepo = planRepo;
            _refreshTokenRepo = refreshTokenRepo;
        }

        public IUserRepository User => _userRepo.Value;
        public IUrlRepository Url => _urlRepo.Value;
        public IPermissionRepository Permission => _permissionRepo.Value;
        public IPlanRepository Plan => _planRepo.Value;
        public IRefreshTokenRepository RefreshToken => _refreshTokenRepo.Value;

        public Task SaveAsync(CancellationToken ct = default)
            => _context.SaveChangesAsync();
        public Task<IDbContextTransaction> BeginTransactionAsync()
         => _context.Database.BeginTransactionAsync();
    }

}
