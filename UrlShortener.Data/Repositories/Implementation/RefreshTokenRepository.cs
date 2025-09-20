using UrlShortener.Data.Entities.Permissions;
using UrlShortener.Data.Entities.Plans;

namespace UrlShortener.Data.Repositories.Implementation
{
    public class RefreshTokenRepository(AppDbContext _context) : RepositoryBase<RefreshToken>(_context), IRefreshTokenRepository
    {

    }
}
