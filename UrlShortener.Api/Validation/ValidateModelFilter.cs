using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace UrlShortener.Validation
{
    public class ValidateModelFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var problem = new ValidationProblemDetails(context.ModelState)
                {
                    Title = "Validation Failed",
                    Detail = "One or more validation errors occurred.",
                    Status = StatusCodes.Status422UnprocessableEntity,
                    Type = "https://httpstatuses.com/422"
                };

                context.Result = new UnprocessableEntityObjectResult(problem);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
    public class EmptyModelValidator : IObjectModelValidator
    {
        public void Validate(ActionContext actionContext, ValidationStateDictionary validationState, string prefix, object model)
        {
        }
    }
}
