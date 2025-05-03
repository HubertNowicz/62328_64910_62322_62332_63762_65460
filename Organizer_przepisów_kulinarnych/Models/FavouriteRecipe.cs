using Microsoft.AspNetCore.Mvc.Rendering;
using Organizer_przepisów_kulinarnych.DAL.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Organizer_przepisów_kulinarnych.Models
{
    public class FavouriteRecipe
    {
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; } 

        [Required]
        public int RecipeId { get; set; }

        [ForeignKey(nameof(RecipeId))]
        public virtual Recipe Recipe { get; set; }


       
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public string FirstName { get; set; }

        [Required]
        public string SurName { get; set; }

        [Required]
        public string RecipeName { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Instructions { get; set; }

        [Required]
        public string Ingredients { get; set; }

        [Required]
        public int Preptime { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }


        [Required]
        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; }

        public List<SelectListItem> Categories { get; set; }



        public bool FavoriteRecipe { get; set; }
    }

       
        
    
}
