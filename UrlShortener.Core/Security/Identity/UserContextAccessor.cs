namespace UrlShortener.Core.Security.Identity
{
    public class UserContextAccessor : IUserContextAccessor
    {
        public UserContext Identity { get; set; }
        public string CorrelationId { get ; set ; }
    }
}
