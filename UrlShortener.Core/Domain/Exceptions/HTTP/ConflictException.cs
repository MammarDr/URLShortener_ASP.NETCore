using Microsoft.AspNetCore.Http;
using UrlShortener.Core.Domain.Exceptions.Base;

namespace UrlShortener.Core.Domain.Exceptions.Http
{
    public class ConflictException : HttpStatusCodeException
    {
        public ConflictException(string message) : base(message, StatusCodes.Status409Conflict) { }
    }
}

