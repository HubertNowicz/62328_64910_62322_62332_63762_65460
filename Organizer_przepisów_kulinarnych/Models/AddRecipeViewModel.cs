using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Organizer_przepisów_kulinarnych.Models
{
    public class AddRecipeViewModel
    {
       

        [Required]
        public string RecipeName { get; set; }

        [Required]
        public string Description { get; set; }


        [Required]
        public string Ingredients { get; set; }

        [Required]
        public string Instructions { get; set; }

        [Required]
        public int Preptime { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int CategoryId { get; set; }

     



    }
}
