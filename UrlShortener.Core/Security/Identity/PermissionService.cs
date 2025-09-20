using UrlShortener.Core.Domain.Results;
using UrlShortener.Core.Security.Identity;
using UrlShortener.Data.Repositories;
using UrlShortener.Models.Enums;

namespace UrlShortener.Core.Services.Implementations
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;

        public PermissionService(IPermissionRepository repo) => _permissionRepository = repo;

        public async Task<Result<IReadOnlyDictionary<enResource, HashSet<enPermission>>>> GetPermissionsAsync(int userId)
            => Result.Success(await _permissionRepository.GetAsync(userId));

        public async Task<Result<bool>> UserHasPermissionAsync(int userId, enResource resource, enPermission permission)
            => !(await _permissionRepository.GetAsync(userId)).TryGetValue(resource, out var permissions) ? false
                : permissions.Contains(permission);

           
    }
}
