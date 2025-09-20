using UrlShortener.Core.Services;

namespace UrlShortener.Core.Domain.Errors
{
    public partial record Error
    {
        public static class UserError
        {
            public static readonly Error SelfDelete = new("Users.SelfDelete", "Self-deletion is restricted. Use the designated endpoint to ensure secure and auditable account removal.", enErrorType.Forbidden);
            public static readonly Error InvalidCredentials = new("Users.InvalidCredentials", "Invalid credentials", enErrorType.BadRequest);
            public static readonly Error EmailExists = new("Users.EmailExists", "Email already exists", enErrorType.Conflict);
            
            public static Error NotFound(int userId) =>
                new("Users.NotFound", $"User with ID '{userId}' was not found.", enErrorType.NotFound);
            public static Error NotFound(string email) =>
                new("Users.NotFound", $"User with Email '{email}' was not found.", enErrorType.NotFound);
        }
    }

    

}
