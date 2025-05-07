using Microsoft.EntityFrameworkCore;
using Organizer_przepisów_kulinarnych.DAL.Entities;

namespace Organizer_przepisów_kulinarnych.DAL.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<FavoriteRecipe> FavoriteRecipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<PendingIngredient> PendingIngredients { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.UserRole)
                    .IsRequired()
                    .HasConversion<int>();

                entity.HasMany(u => u.Recipes)
                    .WithOne(r => r.User)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.FavoriteRecipes)
                    .WithOne(fr => fr.User)
                    .HasForeignKey(fr => fr.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.HasMany(c => c.Recipes)
                    .WithOne(r => r.Category)
                    .HasForeignKey(r => r.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.HasOne(r => r.User)
                    .WithMany(u => u.Recipes)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Category)
                    .WithMany(c => c.Recipes)
                    .HasForeignKey(r => r.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(r => r.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");
            });

            modelBuilder.Entity<FavoriteRecipe>(entity =>
            {
                entity.HasKey(fr => new { fr.UserId, fr.RecipeId });

                entity.HasOne(fr => fr.User)
                    .WithMany(u => u.FavoriteRecipes)
                    .HasForeignKey(fr => fr.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(fr => fr.Recipe)
                    .WithMany(r => r.FavoriteRecipes)
                    .HasForeignKey(fr => fr.RecipeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.HasIndex(i => i.Name)
                      .IsUnique();
            });

            modelBuilder.Entity<RecipeIngredient>(entity =>
            {
                entity.HasKey(ri => ri.Id);

                entity.HasOne(ri => ri.Recipe)
                    .WithMany(r => r.RecipeIngredients)
                    .HasForeignKey(ri => ri.RecipeId);

                entity.HasOne(ri => ri.Ingredient)
                    .WithMany(i => i.RecipeIngredients)
                    .HasForeignKey(ri => ri.IngredientId)
                    .IsRequired(false);
            });
            modelBuilder.Entity<PendingIngredient>(entity =>
            {
                entity.HasKey(si => si.Id);

                entity.Property(si => si.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(si => si.SuggestedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(si => si.SuggestedByUser)
                    .WithMany()
                    .HasForeignKey(si => si.SuggestedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

    }
}
