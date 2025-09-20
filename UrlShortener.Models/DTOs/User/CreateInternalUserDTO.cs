using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Models.DTOs.User
{
    public class CreateInternalUserDTO : IDto
    {
        [Required(ErrorMessage = "Email is a required field.")]
        [MaxLength(60, ErrorMessage = "Maximum length for the Email is 60 characters.")]
        [EmailAddress(ErrorMessage = "Email is invalid.")] 
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is a required field."), MinLength(8)]
        [MaxLength(120, ErrorMessage = "Maximum length for the Email is 120 characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password is a required field.")]
        [Range(1, Int32.MaxValue)]
        public int PlanId { get; set; }

        [Required(ErrorMessage = "Role id is a required field.")]
        [Range(1, Int32.MaxValue)]
        public int RoleId { get; set; }

    }


}
