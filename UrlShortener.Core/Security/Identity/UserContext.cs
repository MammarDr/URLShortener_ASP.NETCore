using System.Collections.ObjectModel;
using UrlShortener.Models.Enums;

namespace UrlShortener.Core.Security.Identity
{
    public class UserContext
    {
         public int Id { get; set; }
         public string Email { get; set; }
         public ReadOnlyDictionary<enResource, HashSet<enPermission>> Permissions { get; set; }
    }
}
