﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Organizer_przepisów_kulinarnych.Models
{
    public class RecipeViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string RecipeName { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public List<RecipeInstructionStepViewModel> InstructionSteps { get; set; } = [];

        [Required]
        public List<RecipeIngredientViewModel> RecipeIngredients { get; set; } = [];

        [Required]
        public int Preptime { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public UserViewModel User { get; set; }

        [Required]
        public CategoryViewModel Category { get; set; }

        public bool IsFavorite { get; set;}
        public bool IsUserRecipe { get; set; }
        public List<SelectListItem> Units { get; set; }

    }
}
