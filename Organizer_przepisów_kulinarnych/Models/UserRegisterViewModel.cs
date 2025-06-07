using System.ComponentModel.DataAnnotations;
namespace Organizer_przepisów_kulinarnych.Models
{
    public class UserRegisterViewModel
    {
        [Required]
        public required string Username { get; set; }

        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string Surname { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public required string ConfirmPassword { get; set; }
    }
}