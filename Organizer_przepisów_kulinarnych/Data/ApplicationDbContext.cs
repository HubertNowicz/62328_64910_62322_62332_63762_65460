using Microsoft.EntityFrameworkCore;
using Organizer_przepisów_kulinarnych.Models;

namespace Organizer_przepisów_kulinarnych.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        //public DbSet<FavouriteRecipe> FavouriteRecipes { get; set; }
    }
}
