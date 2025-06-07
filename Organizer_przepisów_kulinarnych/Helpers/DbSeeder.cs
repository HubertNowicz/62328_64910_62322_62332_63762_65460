using Organizer_przepisów_kulinarnych.BLL.Interfaces;
using Organizer_przepisów_kulinarnych.DAL.Entities.Enums;
using Organizer_przepisów_kulinarnych.DAL.Entities;
using Organizer_przepisów_kulinarnych.DAL.DbContexts;
using Microsoft.EntityFrameworkCore;
using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;

namespace Organizer_przepisów_kulinarnych.Helpers
{
    public static class DbSeeder
    {
        public static async Task SeedAdminUserAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
            var recipeService = scope.ServiceProvider.GetRequiredService<IRecipeService>();

            var existingAdmin = await userService.GetUserByUsernameAsync("admin");
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
                new MeasurementUnit { Name = "gram", Abbreviation = "g" },
                new MeasurementUnit { Name = "kilogram", Abbreviation = "kg" },
                new MeasurementUnit { Name = "mililitr", Abbreviation = "ml" },
                new MeasurementUnit { Name = "litr", Abbreviation = "l" },
                new MeasurementUnit { Name = "sztuka", Abbreviation = "szt" },
                new MeasurementUnit { Name = "łyżka", Abbreviation = "łyżka" },
                new MeasurementUnit { Name = "łyżeczka", Abbreviation = "łyżeczka" }
            };

            context.MeasurementUnits.AddRange(measurementUnits);
            await context.SaveChangesAsync();
        }

        public static async Task SeedIngredientsAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var adminService = scope.ServiceProvider.GetRequiredService<IAdminService>();

            if (context.Ingredients.Any())
                return;

            var measurementUnits = await context.MeasurementUnits.ToListAsync();

            var ingredientsToSeed = new List<(string Name, string UnitAbbreviation)>
                {
                    ("Jajko", "szt"),
                    ("Mąka pszenna", "g"),
                    ("Mleko", "ml"),
                    ("Cukier", "g"),
                    ("Ser żółty", "g"),
                    ("Szynka", "g"),
                    ("Pomidor", "szt"),
                    ("Chleb", "szt"),
                    ("Masło", "g"),
                    ("Cebula", "szt"),
                    ("Papryka", "szt"),
                    ("Pomidory w puszce", "g"),
                    ("Czosnek", "szt"),
                    ("Oliwa z oliwek", "ml"),
                    ("Kurczak", "g"),
                    ("Sałata rzymska", "szt"),
                    ("Parmezan", "g"),
                    ("Grzanki", "g"),
                    ("Dynia", "g"),
                    ("Śmietanka 30%", "ml"),
                    ("Serek mascarpone", "g"),
                    ("Herbatniki", "g"),
                    ("Cukier waniliowy", "g")
                };

            foreach (var (name, unitAbbreviation) in ingredientsToSeed)
            {
                var unit = measurementUnits.FirstOrDefault(u => u.Abbreviation == unitAbbreviation);
                if (unit == null)
                {
                    continue;
                }

                await adminService.AddIngredientAsync(name, new List<int> { unit.Id });
            }
        }

        public static async Task SeedRecipesAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var recipeService = scope.ServiceProvider.GetRequiredService<IRecipeService>();

            if (context.Recipes.Any())
            {
                return;
            }

            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
            if (user == null)
            {
                return;
            }

            var categories = await context.Categories.ToDictionaryAsync(c => c.Name);
            var units = await context.MeasurementUnits.ToDictionaryAsync(u => u.Abbreviation);

            var recipesToSeed = new List<RecipeCreateDto>
                {
                    new()
                    {
                        RecipeName = "Shakshuka",
                        Description = "Jajka duszone w pikantnym sosie pomidorowym.",
                        Preptime = 30,
                        CategoryId = categories["Śniadanie"].Id,
                        RecipeIngredients = new List<RecipeIngredientDto>
                        {
                            new() { Name = "Jajko", Amount = 3, UnitId = units["szt"].Id },
                            new() { Name = "Cebula", Amount = 1, UnitId = units["szt"].Id },
                            new() { Name = "Papryka", Amount = 1, UnitId = units["szt"].Id },
                            new() { Name = "Pomidory w puszce", Amount = 400, UnitId = units["g"].Id },
                            new() { Name = "Czosnek", Amount = 2, UnitId = units["szt"].Id },
                            new() { Name = "Oliwa z oliwek", Amount = 15, UnitId = units["ml"].Id }
                        },
                        InstructionSteps = new List<RecipeInstructionStepDto>
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
                        RecipeIngredients = new List<RecipeIngredientDto>
                        {
                            new() { Name = "Kurczak", Amount = 150, UnitId = units["g"].Id },
                            new() { Name = "Sałata rzymska", Amount = 1, UnitId = units["szt"].Id },
                            new() { Name = "Parmezan", Amount = 30, UnitId = units["g"].Id },
                            new() { Name = "Grzanki", Amount = 50, UnitId = units["g"].Id },
                            new() { Name = "Oliwa z oliwek", Amount = 10, UnitId = units["ml"].Id }
                        },
                        InstructionSteps = new List<RecipeInstructionStepDto>
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
                        RecipeIngredients = new List<RecipeIngredientDto>
                        {
                            new() { Name = "Kurczak", Amount = 200, UnitId = units["g"].Id },
                            new() { Name = "Cebula", Amount = 1, UnitId = units["szt"].Id },
                            new() { Name = "Czosnek", Amount = 2, UnitId = units["szt"].Id },
                            new() { Name = "Mleko", Amount = 200, UnitId = units["ml"].Id }
                        },
                        InstructionSteps = new List<RecipeInstructionStepDto>
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
                        RecipeIngredients = new List<RecipeIngredientDto>
                        {
                            new() { Name = "Serek mascarpone", Amount = 250, UnitId = units["g"].Id },
                            new() { Name = "Cukier waniliowy", Amount = 10, UnitId = units["g"].Id },
                            new() { Name = "Herbatniki", Amount = 150, UnitId = units["g"].Id }
                        },
                        InstructionSteps = new List<RecipeInstructionStepDto>
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
                        RecipeIngredients = new List<RecipeIngredientDto>
                        {
                            new() { Name = "Dynia", Amount = 500, UnitId = units["g"].Id },
                            new() { Name = "Cebula", Amount = 1, UnitId = units["szt"].Id },
                            new() { Name = "Śmietanka 30%", Amount = 100, UnitId = units["ml"].Id }
                        },
                        InstructionSteps = new List<RecipeInstructionStepDto>
                        {
                            new() { StepNumber = 1, Description = "Podsmaż cebulę, dodaj pokrojoną dynię i smaż 5 minut." },
                            new() { StepNumber = 2, Description = "Zalej wodą, gotuj do miękkości." },
                            new() { StepNumber = 3, Description = "Zblenduj, dodaj śmietankę i dopraw." }
                        }
                    }
                };

            foreach (var recipeDto in recipesToSeed)
            {
                recipeDto.UserId = user.Id;
                await recipeService.CreateRecipeAsync(recipeDto);
            }
        }

    }
}
