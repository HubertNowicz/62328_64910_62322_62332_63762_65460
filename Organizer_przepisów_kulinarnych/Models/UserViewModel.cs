using System.ComponentModel.DataAnnotations;

namespace Organizer_przepisów_kulinarnych.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        [Required]
        public required string Username { get; set; }
        [Required]
        public required string FirstName { get; set; }
        [Required]
        public required string Surname { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
    }

}
