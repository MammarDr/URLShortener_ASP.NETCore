
﻿using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Models.DTOs.User
{

    public class UpdateUserDTO : IDto
    {
        public UpdateUserDTO(string email, string password, int planId)
        {
            Email = email;
            Password = password;
            PlanId = planId;
        }

        [Required(ErrorMessage = "Email is a required field.")]
        [MaxLength(60, ErrorMessage = "Maximum length for the Email is 60 characters.")]
        [EmailAddress(ErrorMessage = "Email is invalid.")] 
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is a required field.")]
        [MaxLength(120, ErrorMessage = "Maximum length for the Email is 120 characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Plan id is a required field.")]
        [Range(1, Int32.MaxValue)]
        public int PlanId { get; set; }

    }

}
