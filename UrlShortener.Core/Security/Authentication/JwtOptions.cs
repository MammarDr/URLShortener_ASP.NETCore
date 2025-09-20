namespace UrlShortener.Core.Security.Authentication
{
    public class JwtOptions
    {
        public required string Issuer { get; init; } = default!;
        public required string Audiences { get; init; } = default!;
//        public required string SigningKey { get; init; } = default!;
        public required int Lifetime { get; init; } = 30;

    }

}
