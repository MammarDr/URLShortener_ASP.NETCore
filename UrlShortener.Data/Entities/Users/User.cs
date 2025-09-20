using System.Text.Json.Serialization;
using UrlShortener.Data.Entities.Plans;
using UrlShortener.Data.Entities.Urls;

namespace UrlShortener.Data.Entities.Users
{
    public class User
    {
        public int           ID { get; set; }
        public string        Email { get; set; } = default!;
        public string        Password { get; set; } = default!;
        public int           PlanId { get; set; }
        public Plan          Plan { get; set; } = default!;
        public DateTime?     PlanExpiresAt { get; set; }
        public DateTime?     CreatedAt { get; set; }  // Being nullable means if its left null, SQL can run the DEFAULT

        [JsonIgnore] 
        public ICollection<URL>     URLs { get; set; } = new List<URL>(); // User has one to many relashionship with Url, EF Core understand it through this
        [JsonIgnore]
        public ICollection<UserRoleMapping> RoleMappings { get; set; } = new List<UserRoleMapping>();
    }
}
