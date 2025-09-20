using Microsoft.AspNetCore.Http;
using UrlShortener.Core.Domain.Exceptions.Base;

namespace UrlShortener.Core.Domain.Exceptions.Http
{
    public class ForbiddenException : HttpStatusCodeException
    {
        public ForbiddenException(string message) : base(message, StatusCodes.Status403Forbidden) { }
    }
}
