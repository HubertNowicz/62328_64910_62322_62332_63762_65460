using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organizer_przepisów_kulinarnych.DAL.DbContexts;
using Organizer_przepisów_kulinarnych.DAL.Entities;
using Organizer_przepisów_kulinarnych.Models;

namespace Organizer_przepisów_kulinarnych.Controllers
{
    public class RecipeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationDbContext _userManager;
        public RecipeController(ApplicationDbContext context, ApplicationDbContext userManager)
        {
            _context = context;
            
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var recipes = await _context.Recipes.ToListAsync();
            var users = await _context.Users.ToListAsync(); // tabela z userami (musi zawierać FirstName, SurName)
            var categories = await _context.Categories.ToListAsync();

      
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out var userIdRecipe))
            {
                return Unauthorized(); // brak ID => nie zalogowany albo claim nie dodany
            }
            else
            {
                var favouriteIds = await _context.FavoriteRecipes
                      .Where(f => f.UserId == userIdRecipe)
                      .Select(f => f.RecipeId)
                      .ToListAsync();
                var viewModels = recipes.Select(r =>
                {
                    var user = users.FirstOrDefault(u => u.Id == r.UserId);
                    var category = categories.FirstOrDefault(c => c.Id == r.CategoryId);

                    return new RecipeViewModel
                    {
                        FirstName = user.FirstName ?? "Brak",
                        SurName = user.Surname ?? "Brak",
                        RecipeName = r.RecipeName,
                        Description = r.Description,
                        Ingredients = r.Ingredients,
                        Instructions = r.Instructions,
                        Preptime = r.Preptime,
                        CreatedAt = r.CreatedAt,
                        UserId = r.UserId,
                        CategoryId = r.CategoryId,
                        Name = category?.Name ?? "Brak",
                        RecipeId = r.Id,
                        FavoriteRecipe = favouriteIds.Contains(r.Id)



                    };
                }).ToList();
                return View(viewModels);
            }
                

            
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Users = await _context.Users.ToListAsync();
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }

        // POST: Recipes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddRecipeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdString, out var userId))
                {
                    return Unauthorized(); // brak ID => nie zalogowany albo claim nie dodany
                }
                var recipe = new Recipe
                {
                    RecipeName = model.RecipeName,
                    Description = model.Description,
                    Ingredients = model.Ingredients,
                    Instructions = model.Instructions,
                    Preptime = model.Preptime,
                    CreatedAt = DateTime.Now,
                    UserId =  userId,
                    CategoryId = model.CategoryId
                };

                _context.Recipes.Add(recipe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Users = await _context.Users.ToListAsync();
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> IndexUserRecipe()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return Unauthorized(); // brak ID => nie zalogowany albo claim nie dodany
            }
            var recipes = await _context.Recipes
                .Where(r => r.UserId == userId)
                .ToListAsync();

            var users = await _context.Users.ToListAsync();
            var categories = await _context.Categories.ToListAsync();

            var viewModel = recipes.Select(r =>
            {
                var user = users.FirstOrDefault(u => u.Id == r.UserId);
                var category2 = categories.FirstOrDefault(c => c.Id == r.CategoryId);

                return new RecipeViewModel
                {
                    RecipeId = r.Id,
                    RecipeName = r.RecipeName,
                    Description = r.Description,
                    Ingredients = r.Ingredients,
                    Instructions = r.Instructions,
                    Preptime = r.Preptime,
                    CreatedAt = r.CreatedAt,
                    UserId = r.UserId,
                    CategoryId = r.CategoryId,
                    Name = category2?.Name ?? "Brak"
                };
            }).ToList();

            return View(viewModel);
        }

    }
}
