
using Microsoft.EntityFrameworkCore;
using UrlShortener.Data.Entities.Permissions;
using UrlShortener.Data.Entities.Urls;
using UrlShortener.Models.Enums;

namespace UrlShortener.Data.Repositories.Implementation
{
    public class PermissionRepository(AppDbContext _context) : RepositoryBase<URL>(_context), IPermissionRepository
    {
        public async Task<IReadOnlyDictionary<enResource, HashSet<enPermission>>> GetAsync(int id)
        {
            var permissions = await _context.RoleMappings
                .Where(rm => rm.UserId == id)
                .SelectMany(rm => rm.RoleType.Permissions)
                .Select(p => new
                {
                    p.ResourceId,
                    p.ActionId
                })
                .Distinct()
                .ToListAsync();

            var dict = permissions
            .GroupBy(p => (enResource)p.ResourceId)
            .ToDictionary(
                g => g.Key,
                g => g.Select( p => Enum.Parse<enPermission>(p.ActionId)).ToHashSet()
            );

            return dict;               
        }
    }
}
