using AutoMapper;
using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;
using Organizer_przepisów_kulinarnych.BLL.Interfaces;
using Organizer_przepisów_kulinarnych.DAL.Entities;
using Organizer_przepisów_kulinarnych.DAL.Interfaces;

namespace Organizer_przepisów_kulinarnych.BLL.Services
{
    public class FavoriteRecipeService : IFavortieRecipeService
    {
        private readonly IFavoriteRecipeRepository _favRepo;
        private readonly IMapper _mapper;

        public FavoriteRecipeService(IFavoriteRecipeRepository favRepo, IMapper mapper)
        {
            _favRepo = favRepo;
            _mapper = mapper;
        }
        public async Task ToggleFavoriteAsync(int userId, int recipeId)
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
        }

        public async Task<List<int>> GetFavoriteRecipesIdsForUserAsync(int userId)
        {
            return await _favRepo.GetRecipeIdsByUserAsync(userId);
        }

        public async Task<List<RecipeDto>> GetFavoriteRecipesForUserAsync(int userId)
        {
            var recipes = await _favRepo.GetFavoriteRecipesByUserAsync(userId);
            var dtos = _mapper.Map<List<RecipeDto>>(recipes);

            foreach (var dto in dtos)
            {
                dto.IsFavorite = true;
            }

            return dtos;
        }

    }
}
//.ProjectTo<RecipeDto>()?