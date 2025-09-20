using Microsoft.AspNetCore.Http;
using UrlShortener.Core.Domain.Exceptions.Base;

namespace UrlShortener.Core.Domain.Exceptions.Http
{
    public class BadRequestException : HttpStatusCodeException
    {
        public BadRequestException(string message) : base(message, StatusCodes.Status400BadRequest) { }
    }
}
