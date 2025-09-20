using UrlShortener.Data.Entities.Users;

namespace UrlShortener.Data.Entities.Permissions
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public required string Token { get; set; }
        public required int UserId { get; set; }
        public required DateTime ExpiresOnUtc { get; set; }
        public User User { get; set; }
    }
}
