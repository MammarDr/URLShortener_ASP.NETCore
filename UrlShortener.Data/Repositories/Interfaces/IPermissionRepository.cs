using UrlShortener.Models.Enums;

namespace UrlShortener.Data.Repositories
{
    public interface IPermissionRepository
    {
       public Task<IReadOnlyDictionary<enResource, HashSet<enPermission>>> GetAsync(int id);
    }
}
