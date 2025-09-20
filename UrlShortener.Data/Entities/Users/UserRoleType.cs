using System.Text.Json.Serialization;
using UrlShortener.Data.Entities.Permissions;

namespace UrlShortener.Data.Entities.Users
{
    public class UserRoleType
    {
        public string          ID { get; set; }
        public required string Name { get; set; }

        [JsonIgnore]
        public ICollection<UserRoleMapping> UserMappings { get; set; } = new List<UserRoleMapping>();
        [JsonIgnore]
        public ICollection<Permission> Permissions { get; set; } = new List<Permission>(); // "what permissions does this role have?"
    }
}
