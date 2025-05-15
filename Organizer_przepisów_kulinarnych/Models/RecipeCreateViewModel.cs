using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Organizer_przepisów_kulinarnych.Models
{
    public class RecipeCreateViewModel
    {
        [Required(ErrorMessage = "Nazwa przepisu jest wymagana.")]
        [Display(Name = "Nazwa")]
        public string RecipeName { get; set; }

        [Required(ErrorMessage = "Opis jest wymagany.")]
        [Display(Name = "Opis")]
        public string Description { get; set; }

        [Required(ErrorMessage = "At least one ingredient is required.")]
        [Display(Name = "Składniki")]
        public List<RecipeIngredientViewModel> Ingredients { get; set; } = [];

        [Required]
        [Display(Name = "Instrukcja")]
        public List<RecipeInstructionStepViewModel> InstructionSteps { get; set; } = [];


        [Required(ErrorMessage = "Czas przygotowania jest wymagany.")]
        [Range(1, int.MaxValue, ErrorMessage = "Preparation time must be a positive number.")]
        [Display(Name = "Czas przygotowania (w minutach)")]
        public int Preptime { get; set; }

        [Required(ErrorMessage = "Katgoria jest wymagana.")]
        [Display(Name = "Kategoria")]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Units { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}