namespace UrlShortener.Core.Domain.Exceptions.Base
{
    public class HttpStatusCodeException : Exception
    {
        public readonly int StatusCode;
        public HttpStatusCodeException(string message, int statusCode)  : base(message)
        { 
            StatusCode = statusCode;
        }
    }

}
