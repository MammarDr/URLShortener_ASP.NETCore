namespace UrlShortener.Data.Entities.Urls
{
    public class URLVisit
    {
        public int       UrlId { get; set; }
        public URL       Url { get; set; } = default!;
        public string    VisitorIP { get; set; } = default!;
        public string    UserAgent { get; set; } = default!;
        public DateTime?  VisitedAt { get; set; }
    }
}
