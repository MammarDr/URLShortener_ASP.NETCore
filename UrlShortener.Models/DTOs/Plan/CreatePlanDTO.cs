
﻿using System.ComponentModel.DataAnnotations;
using UrlShortener.Models.Enums;

namespace UrlShortener.Models.DTOs.Plan
{
    public class CreatePlanDTO : IDto
    {
        [Required(ErrorMessage = "Name is a required field.")]
        [MaxLength(20, ErrorMessage = "Maximum length for the Name is 20 characters.")]
        [MinLength(3,  ErrorMessage = "Minimum length for the Name is 20 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Price is a required field.")]
        [Range(0, 99999, ErrorMessage = "Price mus be positive and <= 99999")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Daily Url is a required field.")]
        [Range(0, 100000, ErrorMessage = "Daily Url : Min = 0, Max = 100000")]
        public int MaxDailyURL { get; set; }

        [Required(ErrorMessage = "HasCustomSlugs is a required field.")]
        public bool HasCustomSlugs { get; set; }

        [Range(1, 3650, ErrorMessage = "Url Expairy must be between 1 day and 3650 days (10 years).")]
        public int UrlExpiresAfter { get; set; }

        [Required(ErrorMessage = "SupportLevel is a required field.")]
        [EnumDataType(typeof(enSupportLevel), ErrorMessage = "Invalid Support Level.")]
        public enSupportLevel SupportLevel { get; set; }

    }
}