
﻿using System.ComponentModel.DataAnnotations;
using UrlShortener.Models.Enums;

namespace UrlShortener.Models.DTOs.Plan
{
    public class UpdatePlanDTO : IDto
    {
        [MaxLength(20, ErrorMessage = "Maximum length for the Name is 20 characters."), MinLength(3)] public string Name { get; set; }

        [Range(0, 99999, ErrorMessage = "Pice mus be positive and <= 99999")]
        public decimal Price { get; set; }

        [Range(0, 100000, ErrorMessage = "Daily Url : Min = 0, Max = 100000")]
        public int MaxDailyURL { get; set; }

        public bool HasCustomSlugs { get; set; }

        [Range(1, 3650, ErrorMessage = "Url Expairy must be between 1 day to 3650 days (10 years).")]
        public int UrlExpiresAfter { get; set; }

        [EnumDataType(typeof(enSupportLevel), ErrorMessage = "Invalid Support Level.")]
        public enSupportLevel SupportLevel { get; set; }

    }

}

