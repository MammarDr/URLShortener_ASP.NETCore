

using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Models.DTOs.User
{
    public class AuthenticationRequest : IDto
    {
        [Required(ErrorMessage = "Email is a required field.")]
        [MaxLength(60, ErrorMessage = "Maximum length for the Email is 60 characters.")]
        [EmailAddress(ErrorMessage = "Email is invalid.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is a required field.")]
        [MinLength(8, ErrorMessage = "Minimum length for the Password is 8 characters.")]
        [MaxLength(120, ErrorMessage = "Maximum length for the Password is 120 characters.")]
        public required string Password { get; set; }
    }

}

