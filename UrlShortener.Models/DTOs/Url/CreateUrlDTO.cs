
﻿using System.ComponentModel.DataAnnotations;
using UrlShortener.Models.Attributes;

namespace UrlShortener.Models.DTOs.Url
{
    public class CreateUrlDTO : IDto
    {
        [Required(ErrorMessage = "Url(Source) is a required field.")]
        [MaxLength(500, ErrorMessage = "Maximum length for the Url is 500 characters.")]
        [UrlFormat]
        public string Source { get; set; }

        [MaxLength(120, ErrorMessage = "Maximum length for the Title is 120 characters.")]
        public string Title { get; set; }

        public bool isActive {  get; set; }
    }
}
