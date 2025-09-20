using UrlShortener.Core.Domain.Exceptions.Base;

namespace UrlShortener.Core.Domain.Exceptions.Auth
{
    public class TokenGenerationException : AuthException
    {

        public TokenGenerationException(string message) : base(message) { }
    }
}
