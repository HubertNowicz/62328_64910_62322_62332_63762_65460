using Organizer_przepisów_kulinarnych.DAL.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Organizer_przepisów_kulinarnych.DAL.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserRole UserRole { get; set; }
        public ICollection<Recipe> Recipes { get; set; } = [];
        public ICollection<FavoriteRecipe> FavoriteRecipes { get; set; } = [];
    }
}
