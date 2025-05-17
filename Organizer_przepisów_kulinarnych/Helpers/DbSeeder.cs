using Organizer_przepisów_kulinarnych.BLL.Interfaces;
using Organizer_przepisów_kulinarnych.DAL.Entities.Enums;
using Organizer_przepisów_kulinarnych.DAL.Entities;
using Organizer_przepisów_kulinarnych.DAL.DbContexts;
using Microsoft.EntityFrameworkCore;

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
                new Category { Name = "Obiad" },
                new Category { Name = "Przekąska" },
                new Category { Name = "Ciasto" },
                new Category { Name = "Kolacja" }
            };

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        public static async Task SeedMeasurementUnitsAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (context.MeasurementUnits.Any())
                return;

            var measurementUnits = new List<MeasurementUnit>
            {
                new MeasurementUnit { /*Id = 1,*/ Name = "gram", Abbreviation = "g" },
                new MeasurementUnit { /*Id = 2,*/ Name = "kilogram", Abbreviation = "kg" },
                new MeasurementUnit { /*Id = 3,*/ Name = "mililitr", Abbreviation = "ml" },
                new MeasurementUnit {/* Id = 4,*/ Name = "litr", Abbreviation = "l" },
                new MeasurementUnit { /*Id = 5,*/ Name = "sztuka", Abbreviation = "szt" },
                new MeasurementUnit { /*Id = 6,*/ Name = "łyżka", Abbreviation = "łyżka" },
                new MeasurementUnit { /*Id = 7,*/ Name = "łyżeczka", Abbreviation = "łyżeczka" }
            };

            context.MeasurementUnits.AddRange(measurementUnits);
            await context.SaveChangesAsync();
        }

        public static async Task SeedIngredientsAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (context.Ingredients.Any())
                return;

            var measurementUnits = await context.MeasurementUnits.ToListAsync();

            var ingredients = new List<Ingredient>
            {
                new() { Name = "Jajko", IngredientUnits = [new() { Unit = measurementUnits.First(u => u.Abbreviation == "szt") }] },
                new() { Name = "Mąka pszenna", IngredientUnits = [new() { Unit = measurementUnits.First(u => u.Abbreviation == "g") }] },
                new() { Name = "Mleko", IngredientUnits = [new() { Unit = measurementUnits.First(u => u.Abbreviation == "ml") }] },
                new() { Name = "Cukier", IngredientUnits = [new() { Unit = measurementUnits.First(u => u.Abbreviation == "g") }] },
                new() { Name = "Ser żółty", IngredientUnits = [new() { Unit = measurementUnits.First(u => u.Abbreviation == "g") }] },
                new() { Name = "Szynka", IngredientUnits = [new() { Unit = measurementUnits.First(u => u.Abbreviation == "g") }] },
                new() { Name = "Pomidor", IngredientUnits = [new() { Unit = measurementUnits.First(u => u.Abbreviation == "szt") }] },
                new() { Name = "Chleb", IngredientUnits = [new() { Unit = measurementUnits.First(u => u.Abbreviation == "szt") }] },
                new() { Name = "Masło", IngredientUnits = [new() { Unit = measurementUnits.First(u => u.Abbreviation == "g") }] },
                new() { Name = "Cebula", IngredientUnits = [new() { Unit = measurementUnits.First(u => u.Abbreviation == "szt") }] },
                new() { Name = "Papryka", IngredientUnits = [new() { Unit = measurementUnits.First(u => u.Abbreviation == "szt") }] },
                new() { Name = "Pomidory w puszce", IngredientUnits = [new() { Unit = measurementUnits.First(u => u.Abbreviation == "g") }] },
                new() { Name = "Czosnek", IngredientUnits = [new() { Unit = measurementUnits.First(u => u.Abbreviation == "szt") }] },
                new() { Name = "Oliwa z oliwek", IngredientUnits = [new() { Unit = measurementUnits.First(u => u.Abbreviation == "ml") }] },
                new() { Name = "Kurczak", IngredientUnits = [new() { Unit = measurementUnits.First(u => u.Abbreviation == "g") }] },
                new() { Name = "Sałata rzymska", IngredientUnits = [new() { Unit = measurementUnits.First(u => u.Abbreviation == "szt") }] },
                new() { Name = "Parmezan", IngredientUnits = [new() { Unit = measurementUnits.First(u => u.Abbreviation == "g") }] },
                new() { Name = "Grzanki", IngredientUnits = [new() { Unit = measurementUnits.First(u => u.Abbreviation == "g") }] },
                new() { Name = "Dynia", IngredientUnits = [new() { Unit = measurementUnits.First(u => u.Abbreviation == "g") }] },
                new() { Name = "Śmietanka 30%", IngredientUnits = [new() { Unit = measurementUnits.First(u => u.Abbreviation == "ml") }] },
                new() { Name = "Serek mascarpone", IngredientUnits = [new() { Unit = measurementUnits.First(u => u.Abbreviation == "g") }] },
                new() { Name = "Herbatniki", IngredientUnits = [new() { Unit = measurementUnits.First(u => u.Abbreviation == "g") }] },
                new() { Name = "Cukier waniliowy", IngredientUnits = [new() { Unit = measurementUnits.First(u => u.Abbreviation == "g") }] }
            };

            await context.Ingredients.AddRangeAsync(ingredients);
            await context.SaveChangesAsync();
        }

        public static async Task SeedRecipesAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (context.Recipes.Any())
                return;

            var user = context.Users.FirstOrDefault(u => u.Username == "admin");
            if (user == null) return;

            var ingredients = context.Ingredients.ToDictionary(i => i.Name);
            var units = context.MeasurementUnits.ToDictionary(u => u.Abbreviation);
            var categories = context.Categories.ToDictionary(c => c.Name);

            var recipes = new List<Recipe>
            {
                new()
                {
                    RecipeName = "Shakshuka",
                    Description = "Jajka duszone w pikantnym sosie pomidorowym.",
                    Preptime = 30,
                    CategoryId = categories["Śniadanie"].Id,
                    UserId = user.Id,
                    RecipeIngredients =
                    {
                        new() { Name = "Jajko", Amount = 3, UnitId = units["szt"].Id, IngredientId = ingredients["Jajko"].Id },
                        new() { Name = "Cebula", Amount = 1, UnitId = units["szt"].Id, IngredientId = ingredients["Cebula"].Id },
                        new() { Name = "Papryka", Amount = 1, UnitId = units["szt"].Id, IngredientId = ingredients["Papryka"].Id },
                        new() { Name = "Pomidory w puszce", Amount = 400, UnitId = units["g"].Id, IngredientId = ingredients["Pomidory w puszce"].Id },
                        new() { Name = "Czosnek", Amount = 2, UnitId = units["szt"].Id, IngredientId = ingredients["Czosnek"].Id },
                        new() { Name = "Oliwa z oliwek", Amount = 15, UnitId = units["ml"].Id, IngredientId = ingredients["Oliwa z oliwek"].Id }
                    },
                    InstructionSteps =
                    {
                        new() { StepNumber = 1, Description = "Podsmaż cebulę i paprykę na oliwie przez 5 minut." },
                        new() { StepNumber = 2, Description = "Dodaj czosnek, a po chwili pomidory z puszki. Gotuj 10 minut." },
                        new() { StepNumber = 3, Description = "Zrób wgłębienia w sosie i wbij jajka. Duś pod przykryciem aż się zetną." }
                    }
                },
                new()
                {
                    RecipeName = "Sałatka Cezar z kurczakiem",
                    Description = "Klasyczna sałatka z grillowanym kurczakiem i grzankami.",
                    Preptime = 25,
                    CategoryId = categories["Śniadanie"].Id,
                    UserId = user.Id,
                    RecipeIngredients =
                    [
                        new() { Name = "Kurczak", Amount = 150, UnitId = units["g"].Id, IngredientId = ingredients["Kurczak"].Id },
                        new() { Name = "Sałata rzymska", Amount = 1, UnitId = units["szt"].Id, IngredientId = ingredients["Sałata rzymska"].Id },
                        new() { Name = "Parmezan", Amount = 30, UnitId = units["g"].Id, IngredientId = ingredients["Parmezan"].Id },
                        new() { Name = "Grzanki", Amount = 50, UnitId = units["g"].Id, IngredientId = ingredients["Grzanki"].Id },
                        new() { Name = "Oliwa z oliwek", Amount = 10, UnitId = units["ml"].Id, IngredientId = ingredients["Oliwa z oliwek"].Id }
                    ],
                    InstructionSteps =
                    {
                        new() { StepNumber = 1, Description = "Usmaż kurczaka i pokrój na paski." },
                        new() { StepNumber = 2, Description = "Połącz z pokrojoną sałatą, grzankami i parmezanem." },
                        new() { StepNumber = 3, Description = "Skrop oliwą i wymieszaj." }
                    }
                },
                new()
                {
                    RecipeName = "Kurczak curry z ryżem",
                    Description = "Orientalne danie z kurczakiem i przyprawami.",
                    Preptime = 40,
                    CategoryId = categories["Obiad"].Id,
                    UserId = user.Id,
                    RecipeIngredients =
                    {
                        new() { Name = "Kurczak", Amount = 200, UnitId = units["g"].Id, IngredientId = ingredients["Kurczak"].Id },
                        new() { Name = "Cebula", Amount = 1, UnitId = units["szt"].Id, IngredientId = ingredients["Cebula"].Id },
                        new() { Name = "Czosnek", Amount = 2, UnitId = units["szt"].Id, IngredientId = ingredients["Czosnek"].Id },
                        new() { Name = "Mleko", Amount = 200, UnitId = units["ml"].Id, IngredientId = ingredients["Mleko"].Id }
                    },
                    InstructionSteps =
                    {
                        new() { StepNumber = 1, Description = "Podsmaż cebulę i czosnek, dodaj pokrojonego kurczaka." },
                        new() { StepNumber = 2, Description = "Dodaj przyprawy curry i wlej mleko, gotuj do zgęstnienia." }
                    }
                },
                new()
                {
                    RecipeName = "Sernik na zimno z mascarpone",
                    Description = "Delikatny deser na bazie mascarpone i herbatników.",
                    Preptime = 20,
                    CategoryId = categories["Przekąska"].Id,
                    UserId = user.Id,
                    RecipeIngredients =
                    {
                        new() { Name = "Serek mascarpone", Amount = 250, UnitId = units["g"].Id, IngredientId = ingredients["Serek mascarpone"].Id },
                        new() { Name = "Cukier waniliowy", Amount = 10, UnitId = units["g"].Id, IngredientId = ingredients["Cukier waniliowy"].Id },
                        new() { Name = "Herbatniki", Amount = 150, UnitId = units["g"].Id, IngredientId = ingredients["Herbatniki"].Id }
                    },
                    InstructionSteps =
                    {
                        new() { StepNumber = 1, Description = "Zgnieć herbatniki i wyłóż nimi spód formy." },
                        new() { StepNumber = 2, Description = "Wymieszaj mascarpone z cukrem i nałóż na spód." },
                        new() { StepNumber = 3, Description = "Wstaw do lodówki na 2h." }
                    }
                },
                new()
                {
                    RecipeName = "Zupa krem z dyni",
                    Description = "Aromatyczna zupa idealna na wieczór.",
                    Preptime = 35,
                    CategoryId = categories["Kolacja"].Id,
                    UserId = user.Id,
                    RecipeIngredients =
                    {
                        new() { Name = "Dynia", Amount = 500, UnitId = units["g"].Id, IngredientId = ingredients["Dynia"].Id },
                        new() { Name = "Cebula", Amount = 1, UnitId = units["szt"].Id, IngredientId = ingredients["Cebula"].Id },
                        new() { Name = "Śmietanka 30%", Amount = 100, UnitId = units["ml"].Id, IngredientId = ingredients["Śmietanka 30%"].Id }
                    },
                    InstructionSteps =
                    {
                        new() { StepNumber = 1, Description = "Podsmaż cebulę, dodaj pokrojoną dynię i smaż 5 minut." },
                        new() { StepNumber = 2, Description = "Zalej wodą, gotuj do miękkości." },
                        new() { StepNumber = 3, Description = "Zblenduj, dodaj śmietankę i dopraw." }
                    }
                }
            };

            await context.Recipes.AddRangeAsync(recipes);
            await context.SaveChangesAsync();
        }
    }
}
