using System.ComponentModel.DataAnnotations;

namespace Organizer_przepisów_kulinarnych.Models
{
    public class RecipeInstructionStepViewModel
    {
        [Required]
        public int StepNumber { get; set; }

        [Required]
        public required string Description { get; set; }
    }
}
