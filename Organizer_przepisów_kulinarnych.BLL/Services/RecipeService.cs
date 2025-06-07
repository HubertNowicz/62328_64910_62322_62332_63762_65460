using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;
using Organizer_przepisów_kulinarnych.BLL.Helpers;
using Organizer_przepisów_kulinarnych.BLL.Interfaces;
using Organizer_przepisów_kulinarnych.DAL.Entities;
using Organizer_przepisów_kulinarnych.DAL.Entities.Enums;
using Organizer_przepisów_kulinarnych.DAL.Interfaces;

namespace Organizer_przepisów_kulinarnych.BLL.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMeasurementUnitRepository _unitRepository;
        private readonly IMapper _mapper;

        public RecipeService(
            IRecipeRepository recipeRepository,
            IIngredientRepository ingredientRepository,
            ICategoryRepository categoryRepository,
            IMeasurementUnitRepository unitRepository,
            IMapper mapper)
        {
            _recipeRepository = recipeRepository;
            _ingredientRepository = ingredientRepository;
            _categoryRepository = categoryRepository;
            _unitRepository = unitRepository;
            _mapper = mapper;
        }
        public async Task<List<RecipeDto>> GetAllRecipesAsync()
        {
            var recipes = await _recipeRepository.GetAllAsync();
            return _mapper.Map<List<RecipeDto>>(recipes);
        }

        public Task<List<RecipeDto>> GetFilteredRecipes(List<RecipeDto> recipes, RecipeFilter filter)
        {
            if (filter.FilterUnder30 || filter.FilterBetween30And60 || filter.FilterOver60)
            {
                recipes = recipes.Where(r =>
                    (filter.FilterUnder30 && r.Preptime <= 30) ||
                    (filter.FilterBetween30And60 && r.Preptime >= 30 && r.Preptime <= 60) ||
                    (filter.FilterOver60 && r.Preptime > 60)).ToList();
            }

            recipes = filter.SortOption switch
            {
                RecipeSortOption.Newest => recipes.OrderByDescending(r => r.CreatedAt).ToList(),
                RecipeSortOption.Popularity => recipes.OrderByDescending(r => r.FavoriteCount).ToList(),
                _ => recipes
            };

            return Task.FromResult(recipes);
        }

        public async Task<List<RecipeDto>> GetUserRecipesAsync(int userId)
        {
            var recipes = await _recipeRepository.GetByUserIdAsync(userId);
            return _mapper.Map<List<RecipeDto>>(recipes);
        }


        public async Task<RecipeDto> GetRecipeByIdAsync(int id)
        {
            var recipe = await _recipeRepository.GetByIdAsync(id);
            return _mapper.Map<RecipeDto>(recipe);
        }

        public async Task<List<RecipeDto>> GetRecipesByCategoryAsync(string categoryName)
        {
            var recipes = await _recipeRepository.GetByCategoryAsync(categoryName);
            return _mapper.Map<List<RecipeDto>>(recipes);
        }


        public async Task<List<IngredientDto>> GetAllIngredientsAsync()
        {
            var ingredients = await _ingredientRepository.GetAllAsync();
            return _mapper.Map<List<IngredientDto>>(ingredients);
        }

        public async Task<IEnumerable<MeasurementUnitDto>> GetAllUnitsAsync()
        {
            var units = await _unitRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<MeasurementUnitDto>>(units);
        }

        public async Task<List<MeasurementUnit>> GetUnitsForIngredientAsync(string ingredientName)
        {
            return await _ingredientRepository.GetUnitsByIngredientNameAsync(ingredientName);
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return _mapper.Map<List<CategoryDto>>(categories);
        }

        public async Task CreateRecipeAsync(RecipeCreateDto recipeDto)
        {
            var recipe = _mapper.Map<Recipe>(recipeDto);
            var allIngredients = await _ingredientRepository.GetAllAsync();
            var allPendingIngredients = await _ingredientRepository.GetAllPendingAsync();

            foreach (var recipeIngredient in recipe.RecipeIngredients)
            {
                var existing = allIngredients.FirstOrDefault(i => StringHelper.FuzzyMatch(i.Name, recipeIngredient.Name));
                if (existing != null)
                {
                    recipeIngredient.IngredientId = existing.Id;
                    recipeIngredient.Name = existing.Name;
                    recipeIngredient.Ingredient = null;
                }
                else
                {
                    recipeIngredient.IngredientId = null;
                    recipeIngredient.Ingredient = null;
                    var capitalized = StringHelper.CapitalizeFirstLetter(recipeIngredient.Name);

                    var alreadySuggested = allPendingIngredients.Any(s => StringHelper.FuzzyMatch(s.Name, capitalized));
                    if (!alreadySuggested)
                    {
                        await _ingredientRepository.AddPendingAsync(new PendingIngredient
                        {
                            Name = capitalized,
                            SuggestedByUserId = recipeDto.UserId,
                            MeasurementUnitId = recipeIngredient.UnitId
                        });
                    }
                }
            }

            await _recipeRepository.AddAsync(recipe);
            await _recipeRepository.SaveChangesAsync();
        }

        public async Task UpdateRecipeAsync(int recipeId, RecipeCreateDto recipeDto, int userId)
        {
            var recipe = await _recipeRepository.GetByIdAsync(recipeId);
            if (recipe == null)
            {
                throw new KeyNotFoundException("Recipe not found");
            }

            recipe.RecipeName = recipeDto.RecipeName;
            recipe.Description = recipeDto.Description;
            recipe.Preptime = recipeDto.Preptime;
            recipe.CategoryId = recipeDto.CategoryId;

            var allIngredients = await _ingredientRepository.GetAllAsync();
            var allPendingIngredients = await _ingredientRepository.GetAllPendingAsync();

            recipe.RecipeIngredients.Clear();
            recipe.InstructionSteps.Clear();

            foreach (var recipeIngredientDto in recipeDto.RecipeIngredients)
            {
                var existingIngredient = allIngredients.FirstOrDefault(i => StringHelper.FuzzyMatch(i.Name, recipeIngredientDto.Name));
                if (existingIngredient != null)
                {
                    recipe.RecipeIngredients.Add(new RecipeIngredient
                    {
                        IngredientId = existingIngredient.Id,
                        Name = existingIngredient.Name,
                        Amount = recipeIngredientDto.Amount,
                        UnitId = recipeIngredientDto.UnitId
                    });
                }
                else
                {
                    var capitalized = StringHelper.CapitalizeFirstLetter(recipeIngredientDto.Name);

                    var alreadySuggested = allPendingIngredients.Any(s => StringHelper.FuzzyMatch(s.Name, capitalized));
                    if (!alreadySuggested)
                    {
                        await _ingredientRepository.AddPendingAsync(new PendingIngredient
                        {
                            Name = capitalized,
                            SuggestedByUserId = userId,
                            MeasurementUnitId = recipeIngredientDto.UnitId
                        });
                    }

                    recipe.RecipeIngredients.Add(new RecipeIngredient
                    {
                        IngredientId = null,
                        Name = capitalized,
                        Amount = recipeIngredientDto.Amount,
                        UnitId = recipeIngredientDto.UnitId
                    });
                }
            }

            foreach (var stepDto in recipeDto.InstructionSteps)
            {
                recipe.InstructionSteps.Add(new tRecipeInstructionStep
                {
                    StepNumber = stepDto.StepNumber,
                    Description = stepDto.Description
                });
            }

            await _recipeRepository.SaveChangesAsync();
        }

        public async Task DeleteRecipeAsync(int recipeId)
        {
            var recipe = await _recipeRepository.GetByIdAsync(recipeId);
            if (recipe == null) throw new Exception("Recipe not found.");

            await _recipeRepository.DeleteAsync(recipe);
            await _recipeRepository.SaveChangesAsync();
        }

        public async Task<List<string>> MatchingIngredients(string term)
        {
            var ingredients = await GetAllIngredientsAsync();
            return ingredients
                .Where(i => i.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
                .Select(i => i.Name)
                .Distinct()
                .Take(10)
                .ToList();
        }

    }
}
