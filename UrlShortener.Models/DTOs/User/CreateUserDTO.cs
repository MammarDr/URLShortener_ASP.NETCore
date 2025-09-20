
﻿using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using UrlShortener.Models.DTOs.Url;

namespace UrlShortener.Models.DTOs.User
{
    // If possible turn it to class, so you can add business constraint like Trim(), ToLower() etc..
    public class CreateUserDTO : IDto
    {
        [Required(ErrorMessage = "Email is a required field.")]
        [MaxLength(60, ErrorMessage = "Maximum length for the Email is 60 characters.")]
        [EmailAddress(ErrorMessage = "Email is invalid.")] 
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is a required field.")]
        [MinLength(8, ErrorMessage = "Minimum length for the Password is 8 characters.")]
        [MaxLength(120, ErrorMessage = "Maximum length for the Password is 120 characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Plan id is a required field.")]
        [Range(1, Int32.MaxValue)]
        public int PlanId { get; set; }

        [Required(ErrorMessage = "Role id is a required field.")]
        [Range(1, Int32.MaxValue)]
        public int RoleId { get; set; }

        public IEnumerable<CreateUrlDTO> Urls { get; set; } = default;

    }


}
