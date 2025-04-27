using System.ComponentModel.DataAnnotations;

namespace Organizer_przepisów_kulinarnych.Models
{
    public class UserRole
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }


    }
}
