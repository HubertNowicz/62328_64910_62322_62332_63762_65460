using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Organizer_przepisów_kulinarnych.Models
{
    public class Recipe
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tytuł przepisu jest wymagany.")]
        [StringLength(200)]
        public string RecipeName { get; set; }

        [Required(ErrorMessage = "Składniki są wymagane.")]
        public string Ingredients { get; set; }

        [Required(ErrorMessage = "Instrukcje są wymagane.")]
        public string Instructions { get; set; }

        [Required(ErrorMessage = "Czas przygotowania jest wymagany")]
        public int Preptime { get; set; }

       
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

       
        public int UserId { get; set; }
        public virtual User User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
