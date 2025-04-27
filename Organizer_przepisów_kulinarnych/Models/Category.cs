using System.ComponentModel.DataAnnotations;

namespace Organizer_przepisów_kulinarnych.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa kategorii jest wymagana.")]
        [StringLength(100)]
        public string Name { get; set; }

       
        public virtual ICollection<Recipe> Recipes { get; set; }


    }
}
