using Microsoft.EntityFrameworkCore;
using Organizer_przepisów_kulinarnych.BLL.Entities;

namespace Organizer_przepisów_kulinarnych.BLL.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<FavoriteRecipe> FavoriteRecipes { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // **User Entity**
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);  // Primary Key

                entity.Property(u => u.Username)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasComment("Nazwa użytkownika jest wymagana.");

                entity.Property(u => u.FirstName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasComment("Imię jest wymagane.");

                entity.Property(u => u.Surname)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasComment("Nazwisko jest wymagane.");

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasComment("Adres e-mail jest wymagany.");

                entity.Property(u => u.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasComment("Hasło jest wymagane.");

                // Enum for UserRole
                entity.Property(u => u.UserRole)
                    .IsRequired()
                    .HasConversion<int>();  // Store enum as int (you can use string if you prefer)

                // Navigation properties
                entity.HasMany(u => u.Recipes)
                    .WithOne(r => r.User)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);  // A user can have many recipes

                entity.HasMany(u => u.FavoriteRecipes)
                    .WithOne(fr => fr.User)
                    .HasForeignKey(fr => fr.UserId)
                    .OnDelete(DeleteBehavior.Restrict);  // Prevent cascading deletes for favorite recipes
            });

            // **Category Entity**
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);  // Primary Key

                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasComment("Nazwa kategorii jest wymagana.");

                // Navigation property
                entity.HasMany(c => c.Recipes)
                    .WithOne(r => r.Category)
                    .HasForeignKey(r => r.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);  // A category can have many recipes
            });

            // **Recipe Entity**
            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.HasKey(r => r.Id);  // Primary Key

                entity.Property(r => r.RecipeName)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasComment("Tytuł przepisu jest wymagany.");

                entity.Property(r => r.Description)
                    .HasMaxLength(1000);  // Description doesn't have a required limit in the model, just an example

                entity.Property(r => r.Ingredients)
                    .IsRequired()
                    .HasComment("Składniki są wymagane.");

                entity.Property(r => r.Instructions)
                    .IsRequired()
                    .HasComment("Instrukcje są wymagane.");

                entity.Property(r => r.Preptime)
                    .IsRequired()
                    .HasComment("Czas przygotowania jest wymagany");

                // Foreign Key to User (creator of the recipe)
                entity.HasOne(r => r.User)
                    .WithMany(u => u.Recipes)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);  // A recipe is linked to a user

                // Foreign Key to Category
                entity.HasOne(r => r.Category)
                    .WithMany(c => c.Recipes)
                    .HasForeignKey(r => r.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);  // A recipe is linked to a category

                // CreatedAt field
                entity.Property(r => r.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");  // Default value as UTC now
            });

            // **FavoriteRecipe Entity**
            modelBuilder.Entity<FavoriteRecipe>(entity =>
            {
                // Composite Primary Key
                entity.HasKey(fr => new { fr.UserId, fr.RecipeId });

                // Foreign Keys
                entity.HasOne(fr => fr.User)
                    .WithMany(u => u.FavoriteRecipes)
                    .HasForeignKey(fr => fr.UserId)
                    .OnDelete(DeleteBehavior.Restrict);  // Prevent cascading deletes for favorite recipes

                entity.HasOne(fr => fr.Recipe)
                    .WithMany(r => r.FavoriteRecipes)
                    .HasForeignKey(fr => fr.RecipeId)
                    .OnDelete(DeleteBehavior.Restrict);  // A favorite recipe is linked to a recipe
            });
        }

    }
}
