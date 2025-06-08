using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Organizer_przepisów_kulinarnych.BLL.Common;
using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;
using Organizer_przepisów_kulinarnych.BLL.Helpers;
using Organizer_przepisów_kulinarnych.BLL.Interfaces;
using Organizer_przepisów_kulinarnych.DAL.Entities;
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

        public async Task<Result<List<RecipeDto>>> GetAllRecipesAsync()
        {
            try
            {
                var recipes = await _recipeRepository.GetAllAsync();
                return Result<List<RecipeDto>>.Ok(_mapper.Map<List<RecipeDto>>(recipes));
            }
            catch (Exception ex)
            {
                return Result<List<RecipeDto>>.Fail($"Error fetching recipes: {ex.Message}");
            }
        }

        public Task<Result<List<RecipeDto>>> GetFilteredRecipes(List<RecipeDto> recipes, RecipeFilter filter)
        {
            try
            {
                if (filter.FilterUnder30 || filter.FilterBetween30And60 || filter.FilterOver60)
                {
                    recipes = recipes.Where(r =>
                        (filter.FilterUnder30 && r.Preptime <= 30) ||
                        (filter.FilterBetween30And60 && r.Preptime > 30 && r.Preptime <= 60) ||
                        (filter.FilterOver60 && r.Preptime > 60)).ToList();
                }

                recipes = filter.SortOption switch
                {
                    DAL.Entities.Enums.RecipeSortOption.Newest => recipes.OrderByDescending(r => r.CreatedAt).ToList(),
                    DAL.Entities.Enums.RecipeSortOption.Popularity => recipes.OrderByDescending(r => r.FavoriteCount).ToList(),
                    _ => recipes
                };

                return Task.FromResult(Result<List<RecipeDto>>.Ok(recipes));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result<List<RecipeDto>>.Fail($"Error filtering recipes: {ex.Message}"));
            }
        }

        public async Task<Result<List<RecipeDto>>> GetUserRecipesAsync(int userId)
        {
            try
            {
                var recipes = await _recipeRepository.GetByUserIdAsync(userId);
                return Result<List<RecipeDto>>.Ok(_mapper.Map<List<RecipeDto>>(recipes));
            }
            catch (Exception ex)
            {
                return Result<List<RecipeDto>>.Fail($"Error retrieving user recipes: {ex.Message}");
            }
        }

        public async Task<Result<RecipeDto>> GetRecipeByIdAsync(int id)
        {
            var recipe = await _recipeRepository.GetByIdAsync(id);
            return recipe == null
                ? Result<RecipeDto>.Fail("Recipe not found.")
                : Result<RecipeDto>.Ok(_mapper.Map<RecipeDto>(recipe));
        }

        public async Task<Result<List<RecipeDto>>> GetRecipesByCategoryAsync(string categoryName)
        {
            try
            {
                var recipes = await _recipeRepository.GetByCategoryAsync(categoryName);
                return Result<List<RecipeDto>>.Ok(_mapper.Map<List<RecipeDto>>(recipes));
            }
            catch (Exception ex)
            {
                return Result<List<RecipeDto>>.Fail($"Error retrieving category recipes: {ex.Message}");
            }
        }

        public async Task<Result<List<CategoryDto>>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync();
                return Result<List<CategoryDto>>.Ok(_mapper.Map<List<CategoryDto>>(categories));
            }
            catch (Exception ex)
            {
                return Result<List<CategoryDto>>.Fail($"Error fetching categories: {ex.Message}");
            }
        }

        public async Task<Result<List<IngredientDto>>> GetAllIngredientsAsync()
        {
            try
            {
                var ingredients = await _ingredientRepository.GetAllAsync();
                return Result<List<IngredientDto>>.Ok(_mapper.Map<List<IngredientDto>>(ingredients));
            }
            catch (Exception ex)
            {
                return Result<List<IngredientDto>>.Fail($"Error fetching ingredients: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<MeasurementUnitDto>>> GetAllUnitsAsync()
        {
            try
            {
                var units = await _unitRepository.GetAllAsync();
                return Result<IEnumerable<MeasurementUnitDto>>.Ok(_mapper.Map<IEnumerable<MeasurementUnitDto>>(units));
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<MeasurementUnitDto>>.Fail($"Error retrieving units: {ex.Message}");
            }
        }

        public async Task<Result<List<MeasurementUnit>>> GetUnitsForIngredientAsync(string ingredientName)
        {
            try
            {
                var units = await _ingredientRepository.GetUnitsByIngredientNameAsync(ingredientName);
                return Result<List<MeasurementUnit>>.Ok(units);
            }
            catch (Exception ex)
            {
                return Result<List<MeasurementUnit>>.Fail($"Error fetching units for ingredient: {ex.Message}");
            }
        }

        public async Task<Result> CreateRecipeAsync(RecipeCreateDto recipeDto)
        {
            try
            {
                var recipe = _mapper.Map<Recipe>(recipeDto);
                var allIngredients = await _ingredientRepository.GetAllAsync();
                var allPending = await _ingredientRepository.GetAllPendingAsync();

                foreach (var ri in recipe.RecipeIngredients)
                {
                    var existing = allIngredients.FirstOrDefault(i => StringHelper.FuzzyMatch(i.Name, ri.Name));
                    if (existing != null)
                    {
                        ri.IngredientId = existing.Id;
                        ri.Name = existing.Name;
                        ri.Ingredient = null;
                    }
                    else
                    {
                        var capitalized = StringHelper.CapitalizeFirstLetter(ri.Name);
                        var alreadySuggested = allPending.Any(p => StringHelper.FuzzyMatch(p.Name, capitalized));
                        if (!alreadySuggested)
                        {
                            await _ingredientRepository.AddPendingAsync(new PendingIngredient
                            {
                                Name = capitalized,
                                SuggestedByUserId = recipeDto.UserId,
                                MeasurementUnitId = ri.UnitId
                            });
                        }
                    }
                }

                await _recipeRepository.AddAsync(recipe);
                await _recipeRepository.SaveChangesAsync();

                return Result.Ok("Recipe created successfully.");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Error creating recipe: {ex.Message}");
            }
        }

        public async Task<Result> UpdateRecipeAsync(int recipeId, RecipeCreateDto recipeDto, int userId)
        {
            try
            {
                var recipe = await _recipeRepository.GetByIdAsync(recipeId);
                if (recipe == null) return Result.Fail("Recipe not found.");

                recipe.RecipeName = recipeDto.RecipeName;
                recipe.Description = recipeDto.Description;
                recipe.Preptime = recipeDto.Preptime;
                recipe.CategoryId = recipeDto.CategoryId;

                var allIngredients = await _ingredientRepository.GetAllAsync();
                var allPending = await _ingredientRepository.GetAllPendingAsync();

                recipe.RecipeIngredients.Clear();
                recipe.InstructionSteps.Clear();

                foreach (var ri in recipeDto.RecipeIngredients)
                {
                    var existing = allIngredients.FirstOrDefault(i => StringHelper.FuzzyMatch(i.Name, ri.Name));
                    var capitalized = StringHelper.CapitalizeFirstLetter(ri.Name);

                    if (existing != null)
                    {
                        recipe.RecipeIngredients.Add(new RecipeIngredient
                        {
                            IngredientId = existing.Id,
                            Name = existing.Name,
                            Amount = ri.Amount,
                            UnitId = ri.UnitId
                        });
                    }
                    else
                    {
                        var alreadySuggested = allPending.Any(p => StringHelper.FuzzyMatch(p.Name, capitalized));
                        if (!alreadySuggested)
                        {
                            await _ingredientRepository.AddPendingAsync(new PendingIngredient
                            {
                                Name = capitalized,
                                SuggestedByUserId = userId,
                                MeasurementUnitId = ri.UnitId
                            });
                        }

                        recipe.RecipeIngredients.Add(new RecipeIngredient
                        {
                            IngredientId = null,
                            Name = capitalized,
                            Amount = ri.Amount,
                            UnitId = ri.UnitId
                        });
                    }
                }

                foreach (var step in recipeDto.InstructionSteps)
                {
                    recipe.InstructionSteps.Add(new RecipeInstructionStep
                    {
                        StepNumber = step.StepNumber,
                        Description = step.Description
                    });
                }

                await _recipeRepository.SaveChangesAsync();
                return Result.Ok("Recipe updated successfully.");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Error updating recipe: {ex.Message}");
            }
        }

        public async Task<Result> DeleteRecipeAsync(int recipeId)
        {
            try
            {
                var recipe = await _recipeRepository.GetByIdAsync(recipeId);
                if (recipe == null) return Result.Fail("Recipe not found.");

                await _recipeRepository.DeleteAsync(recipe);
                await _recipeRepository.SaveChangesAsync();

                return Result.Ok("Recipe deleted successfully.");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Error deleting recipe: {ex.Message}");
            }
        }

        public async Task<Result<List<string>>> MatchingIngredients(string term)
        {
            try
            {
                var ingredientsResult = await GetAllIngredientsAsync();
                if (!ingredientsResult.Success)
                    return Result<List<string>>.Fail(ingredientsResult.Error);

                var matches = ingredientsResult.Data
                    .Where(i => i.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
                    .Select(i => i.Name)
                    .Distinct()
                    .Take(10)
                    .ToList();

                return Result<List<string>>.Ok(matches);
            }
            catch (Exception ex)
            {
                return Result<List<string>>.Fail($"Error searching ingredients: {ex.Message}");
            }
        }
    }
}
