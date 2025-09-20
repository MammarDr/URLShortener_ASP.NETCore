using NLog.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Core.Domain.Errors;
using UrlShortener.Core.Domain.Results;
using UrlShortener.Core.Services;
using UrlShortener.Models.DTOs;

namespace UrlShortener.Core.Validators
{
    public static class DtoValidator
    {
        public static Result<bool> Validate(this IDto dto)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(dto);
            if (!Validator.TryValidateObject(dto, context, results, validateAllProperties: true))
            {
                var errors = new Dictionary<string, List<string>>();
                foreach (var validationResult in results)
                {
                    var key = validationResult.MemberNames.ElementAt(0);

                    if (errors.ContainsKey(key))
                    {
                        errors[key].Add(validationResult?.ErrorMessage ?? "Unkown Error");
                    }
                    else
                    {
                        errors[key] = [validationResult.ErrorMessage];
                    }
                }
                return new ValidationError(errors, "Validation Failed", "One or more validation errors occurred.", enErrorType.Validation);
            }

            return true;
        }
    }
}
