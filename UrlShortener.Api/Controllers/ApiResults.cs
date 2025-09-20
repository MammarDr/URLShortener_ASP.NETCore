using Microsoft.AspNetCore.Mvc;
using UrlShortener.Core.Domain.Errors;
using UrlShortener.Core.Domain.Results;
using UrlShortener.Core.Services;


namespace UrlShortener.Api.Controllers
{
    public static class ApiResults
        {
        public static ActionResult Problem(Result result)
        {
            var error = result.Error;

                var problem = new ProblemDetails
            {
                Title = GetTitle(error),
                Detail = GetDetail(error),
                Type = GetType(error.Type),
                Status = GetStatusCode(error.Type),
            };

            // Extension is read-only
            if(error is ValidationError)
            foreach (var kv in GetErrors(result))
                problem.Extensions[kv.Key] = kv.Value;
            

            // Wrap inside ObjectResult with status code
            return new ObjectResult(problem)
            {
                StatusCode = problem.Status
            };
        }

            private static int GetStatusCode(enErrorType type) => type switch
            {
                enErrorType.NotFound => StatusCodes.Status404NotFound,
                enErrorType.Validation => StatusCodes.Status422UnprocessableEntity,
                enErrorType.Conflict => StatusCodes.Status409Conflict,
                enErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                enErrorType.Forbidden => StatusCodes.Status403Forbidden,
                enErrorType.NullValue => StatusCodes.Status400BadRequest,
                enErrorType.BadRequest => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            private static string GetType(enErrorType type) => type switch
            {
                enErrorType.NotFound => $"https://httpstatuses.com/404",
                enErrorType.Validation => $"https://httpstatuses.com/422",
                enErrorType.Conflict => $"https://httpstatuses.com/409",
                enErrorType.Unauthorized => $"https://httpstatuses.com/401",
                enErrorType.Forbidden => $"https://httpstatuses.com/403",
                enErrorType.NullValue => $"https://httpstatuses.com/400",
                _ => $"https://httpstatuses.com/500",
            };

            private static string GetDetail(Error error) =>
                error.Description ?? "An unexpected error occurred.";

            private static string GetTitle(Error error) =>
                error.Code;

            private static IDictionary<string, object?> GetErrors(Result result) {
                if(result.Error is not ValidationError validation)
                    return null;

                return new Dictionary<string, object?>
                {
                    {"errors", validation.Errors }
                };
            }
        }
}
