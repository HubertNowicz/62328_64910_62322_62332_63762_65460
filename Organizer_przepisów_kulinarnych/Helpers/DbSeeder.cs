using Organizer_przepisów_kulinarnych.BLL.Interfaces;
using Organizer_przepisów_kulinarnych.DAL.Entities.Enums;
using Organizer_przepisów_kulinarnych.DAL.Entities;
using Organizer_przepisów_kulinarnych.DAL.DbContexts;

namespace Organizer_przepisów_kulinarnych.Helpers
{
    public static class DbSeeder
    {
        public static async Task SeedAdminUserAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

            var existingAdmin = userService.GetByUsername("admin");
            if (existingAdmin != null)
                return;

            var admin = new User
            {
                Username = "admin",
                FirstName = "System",
                Surname = "Administrator",
                Email = "admin@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                UserRole = UserRole.Admin
            };

            await userService.CreateAsync(admin);
        }
        public static async Task SeedCategoriesAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (context.Categories.Any())
                return;

            var categories = new List<Category>
    {
        new Category { Name = "Śniadanie" },
        new Category { Name = "Lunch" },
        new Category { Name = "Obiad" },
        new Category { Name = "Podwieczorek" },
        new Category { Name = "Kolacja" }
    };

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }
    }

}
