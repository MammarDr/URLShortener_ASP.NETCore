using UrlShortener.Core.Domain.Results;
using UrlShortener.Core.Services;

namespace UrlShortener.Core.Domain.Errors
{
    public partial record Error(string Code, string? Description = null, enErrorType Type = enErrorType.Unexpected)
    {
        public static readonly Error None = new("None", null, enErrorType.None);
        public static readonly Error NullValue = new("NullException", "Value was null", enErrorType.NullValue);
        public static readonly Error NotFound = new("NotFound", "Resource not found", enErrorType.NotFound);
        public static readonly Error Unexpected = new("ServerInternal", "Unexpected error occurred.", enErrorType.Unexpected);
        public static readonly Error Canceled = new("TaskCanceled", "Timeout occurred.", enErrorType.Unexpected);

        public static Error AlreadyExists(string Resource = "Resource") => new("AlreadyExists", $"{Resource} Already exists in this context.", enErrorType.Conflict);
        public static Error BadRequest(string Details) => new("BadRequest", Details, enErrorType.BadRequest);

        public static implicit operator Result(Error error) => Result.Failure(error);
        public static implicit operator Task<Result>(Error error) => Task.FromResult(Result.Failure(error));
    }

}
