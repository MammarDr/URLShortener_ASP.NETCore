using Microsoft.AspNetCore.Http;
using UrlShortener.Core.Domain.Exceptions.Base;

namespace UrlShortener.Core.Domain.Exceptions.Http
{
    public class UnauthorizedAccessException : HttpStatusCodeException
    {
        public UnauthorizedAccessException(string message) : base(message, StatusCodes.Status401Unauthorized) { }
    }
}
