namespace UrlShortener.Core.Security.Identity
{
    public interface IUserContextAccessor
    {
        UserContext Identity { get; set; }
        string      CorrelationId { get; set; }
    }
}
