using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Core.Services;

namespace UrlShortener.Core.Domain.Errors
{
    public partial record Error
    {

        public class UrlError
        {
            public static readonly Error InvalidSource = new("Urls.InvalidSource", $"Url provided is invalid.", enErrorType.BadRequest);
            public static readonly Error Forbidden     = new($"Urls.Forbidden", $"Forbidden from taking this action.", enErrorType.Forbidden);

            public static Error NotFound(int urlId) =>
                new("Urls.NotFound", $"Url with ID '{urlId}' was not found.", enErrorType.NotFound);
            public static Error NotFound(string sc) =>
                new("Urls.NotFound", $"Url with ShortCode '{sc}' was not found.", enErrorType.NotFound);
            public static  Error ShortCode(int tries) 
                => new($"Urls.ShortCode", $"Server can't generate valid short code after {tries} tries", enErrorType.Unexpected);

            
        }
    }
}
