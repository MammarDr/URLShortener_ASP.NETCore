
﻿
using System.ComponentModel.DataAnnotations;
using UrlShortener.Core.Validators;


namespace UrlShortener.Models.Attributes
{
    public class UrlFormatAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
           => value is string url ? URLValidator.Validate(url) : false ;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string url && URLValidator.Validate(url))
                return ValidationResult.Success;

            return new ValidationResult("Invalid URL format.", [validationContext.MemberName]);
        }
    }
}

