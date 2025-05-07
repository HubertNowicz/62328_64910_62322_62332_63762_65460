using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Organizer_przepisów_kulinarnych.Models
{
    public class RecipeCreateViewModel
    {
        [Required]
        public string RecipeName { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public List<IngredientViewModel> Ingredients { get; set; } = [];

        [Required]
        public string Instructions { get; set; }

        [Required]
        [Display(Name = "Preparation Time (minutes)")]
        public int Preptime { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}