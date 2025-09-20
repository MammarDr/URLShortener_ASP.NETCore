using UrlShortener.Core.Domain.Results;
using UrlShortener.Models.Enums;

namespace UrlShortener.Core.Security.Identity
{
    public interface IPermissionService
    {
        Task<Result<IReadOnlyDictionary<enResource, HashSet<enPermission>>>> GetPermissionsAsync(int userId);
        Task<Result<bool>> UserHasPermissionAsync(int userId, enResource resource, enPermission permission);
    }
}
