using System.ComponentModel.DataAnnotations;

namespace Organizer_przepisów_kulinarnych.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

}
