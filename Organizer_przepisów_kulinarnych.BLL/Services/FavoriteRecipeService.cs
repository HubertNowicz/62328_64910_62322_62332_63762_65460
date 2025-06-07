using AutoMapper;
using Organizer_przepisów_kulinarnych.BLL.Common;
using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;
using Organizer_przepisów_kulinarnych.BLL.Interfaces;
using Organizer_przepisów_kulinarnych.DAL.Entities;
using Organizer_przepisów_kulinarnych.DAL.Interfaces;

namespace Organizer_przepisów_kulinarnych.BLL.Services
{
    public class FavoriteRecipeService : IFavoriteRecipeService
    {
        private readonly IFavoriteRecipeRepository _favRepo;
        private readonly IMapper _mapper;

        public FavoriteRecipeService(IFavoriteRecipeRepository favRepo, IMapper mapper)
        {
            _favRepo = favRepo;
            _mapper = mapper;
        }
        public async Task<Result> ToggleFavoriteAsync(int userId, int recipeId)
        {
            if (userId <= 0 || recipeId <= 0)
                return Result.Fail("Invalid user or recipe ID.");

            try
            {
                var existing = await _favRepo.GetByUserAndRecipeAsync(userId, recipeId);

                if (existing != null)
                {
                    _favRepo.Remove(existing);
                }
                else
                {
                    var newFavorite = new FavoriteRecipe
                    {
                        UserId = userId,
                        RecipeId = recipeId
                    };

                    await _favRepo.AddAsync(newFavorite);
                }

                await _favRepo.SaveChangesAsync();
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail($"An error occurred while toggling the favorite recipe. {ex}");
            }
        }


        public async Task<Result<List<int>>> GetFavoriteRecipesIdsForUserAsync(int userId)
        {
            try
            {
                var recipeIds = await _favRepo.GetRecipeIdsByUserAsync(userId);
                return Result<List<int>>.Ok(recipeIds);
            }
            catch (Exception ex)
            {
                return Result<List<int>>.Fail($"Failed to retrieve favorite recipe IDs: {ex.Message}");
            }
        }


        public async Task<Result<List<RecipeDto>>> GetFavoriteRecipesForUserAsync(int userId)
        {
            if (userId <= 0)
                return Result<List<RecipeDto>>.Fail("Invalid user ID.");

            try
            {
                var recipes = await _favRepo.GetFavoriteRecipesByUserAsync(userId);
                var dtos = _mapper.Map<List<RecipeDto>>(recipes);

                foreach (var dto in dtos)
                {
                    dto.IsFavorite = true;
                }

                return Result<List<RecipeDto>>.Ok(dtos);
            }
            catch (Exception)
            {
                return Result<List<RecipeDto>>.Fail("Failed to retrieve favorite recipes.");
            }
        }
    }
}