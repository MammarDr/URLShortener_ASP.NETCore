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
        public static class TokenError
        {
            public static readonly Error InvalidCredentials = new("Token.InvalidCredentials", "User or Permissions are Invalid.", enErrorType.BadRequest);
            public static readonly Error SigningKeyMissing = new("Token.SigningKeyMissing", "Signing Key is missing.", enErrorType.Unexpected);
            public static readonly Error Unexpected = new("Token.Unexpected", "Unexpected Error, Contact the admin.", enErrorType.Unexpected);
            public static readonly Error ExpiredToken = new("Token.ExpiredToken", "Expired Token, Login again.", enErrorType.BadRequest);
            public static Error NotFound(int userId) =>
                new("Users.NotFound", $"User with ID '{userId}' was not found.", enErrorType.NotFound);

        }
    }
}
