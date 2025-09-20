using UrlShortener.Core.Services;

namespace UrlShortener.Core.Domain.Errors
{
    public partial record Error
    {
        public static class PlanError
        {

            public static Error ListNotFound = new("Plans.NotFound", $"No Plan was found.", enErrorType.NotFound);


            public static Error NotFound(int planId) =>
                new("Plans.NotFound", $"Plan with ID '{planId}' was not found.", enErrorType.NotFound);
        }
    }
}
