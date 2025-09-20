using UrlShortener.Data.Entities.Users;

namespace UrlShortener.Data.Entities.Permissions
{
    public class Permission
    {
        public required string    RoleId     { get; set; }
        public UserRoleType       RoleType { get; set; }
        public required string       ActionId   { get; set; }
        public UserAction         Action { get; set; }
        public required int       ResourceId { get; set; }
        public Resource           Resource { get; set; }
    }
}
