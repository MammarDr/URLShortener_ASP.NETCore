using System.Linq.Expressions;
using UrlShortener.Data.Entities.Permissions;
using UrlShortener.Data.Entities.Plans;
using UrlShortener.Data.Repositories.Implementation;
using UrlShortener.Data.Repositories.Interfaces;

namespace UrlShortener.Data.Repositories
{
    public interface IRefreshTokenRepository : IRepositoryBase<RefreshToken>
    {
        
    }
}
