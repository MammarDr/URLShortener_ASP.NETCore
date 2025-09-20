using System.Text.Json.Serialization;
using UrlShortener.Data.Entities.Users;

namespace UrlShortener.Data.Entities.Urls
{
    
    public class URL
    {
        public int              ID           { get; set; }
        public string           ShortCode    { get; set; }
        public string           Source       { get; set; }
        public string           Title        { get; set; }
        public int?             VisitCount   { get; set; }
        public int              CreatedBy    { get; set; }
        public User             CreatedByUser{ get; set; }
        public DateTime?        CreatedAt    { get; set; }
        public DateTime?        LastModified { get; set; }
        public DateTime         ExpiresAt    { get; set; }
        public bool?            isActive     { get; set; }

        [JsonIgnore]
        public ICollection<URLVisit>   Visits       { get; set; }
                                = new List<URLVisit>();
    }
}
