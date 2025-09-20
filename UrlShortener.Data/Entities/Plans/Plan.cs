using System.Text.Json.Serialization;
using UrlShortener.Data.Entities.Users;
using UrlShortener.Models.Enums;

namespace UrlShortener.Data.Entities.Plans
{
    public class Plan
    {
        public int              ID { get; set; }
        public string           Name { get; set; } = default!;
        public decimal          Price { get; set; }
        public int              MaxDailyURL { get; set; }
        public bool             HasCustomSlugs { get; set; }
        public int              UrlExpiresAfter { get; set; }
        public enSupportLevel   SupportLevel {  get; set; }

        [JsonIgnore]
        public ICollection<User>   Users { get; set; } = new List<User>();
    }
}

