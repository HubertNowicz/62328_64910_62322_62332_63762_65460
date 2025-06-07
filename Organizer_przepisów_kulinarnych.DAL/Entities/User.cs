using Organizer_przepisów_kulinarnych.DAL.Entities.Enums;

namespace Organizer_przepisów_kulinarnych.DAL.Entities
{
    public class User
    {
        public int Id { get; set; }

        public required string Username { get; set; }
        public required string FirstName { get; set; }
        public required string Surname { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }

        public UserRole UserRole { get; set; }

        public ICollection<Recipe> Recipes { get; set; } = [];
        public ICollection<FavoriteRecipe> FavoriteRecipes { get; set; } = [];
    }
}
