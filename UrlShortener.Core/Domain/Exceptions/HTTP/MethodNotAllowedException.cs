using Microsoft.AspNetCore.Http;
using UrlShortener.Core.Domain.Exceptions.Base;

namespace UrlShortener.Core.Domain.Exceptions.Http
{

    public class MethodNotAllowedException : HttpStatusCodeException
    {
        public MethodNotAllowedException(string message) : base(message, StatusCodes.Status405MethodNotAllowed) { }
    }
}
