using AutoMapper;
using Organizer_przepisów_kulinarnych.BLL.Interfaces;
using Organizer_przepisów_kulinarnych.DAL.Entities;
using Organizer_przepisów_kulinarnych.BLL.Helpers;
using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;
using Organizer_przepisów_kulinarnych.DAL.Interfaces;

namespace Organizer_przepisów_kulinarnych.BLL.Services
{
    public class AdminService : IAdminService
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;

        public AdminService(IIngredientRepository ingredientRepository, IMapper mapper)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
        }
        public async Task<List<PendingIngredientDto>> GetAllPendingIngredientsAsync()
        {
            var suggestions = await _ingredientRepository.GetAllPendingAsync();
            return _mapper.Map<List<PendingIngredientDto>>(suggestions);
        }

        public async Task ApprovePendingIngredientAsync(int suggestedIngredientId)
        {
            var allIngredients = await _ingredientRepository.GetAllAsync();
            var allPendingIngredients = await _ingredientRepository.GetAllPendingAsync();

            var suggestion = allPendingIngredients.First(s => s.Id == suggestedIngredientId);

            int ingredientId = 0;
            var exists = allIngredients.Any(i => StringHelper.FuzzyMatch(i.Name, suggestion.Name));

            if (!exists)
            {
                var capitalizedName = StringHelper.CapitalizeFirstLetter(suggestion.Name);
                var newIngredient = new Ingredient
                {
                    Name = capitalizedName
                };

                await _ingredientRepository.AddIngredientAsync(newIngredient);
                ingredientId = newIngredient.Id;

                var unit = new IngredientUnit
                {
                    IngredientId = ingredientId,
                    UnitId = suggestion.MeasurementUnitId
                };

                await _ingredientRepository.AddIngredientUnitAsync(unit);
            }

            var matchingSuggestions = allPendingIngredients
                     .Where(s => StringHelper.FuzzyMatch(s.Name, suggestion.Name));

            await _ingredientRepository.RemovePendingRangeAsync(matchingSuggestions);


            var recipeIngredients = await _ingredientRepository.GetAllRecipeIngredientsAsync();

            var matchingRecipesIngredients = recipeIngredients
                .Where(ri => StringHelper
                .FuzzyMatch(ri.Name, suggestion.Name))
                .ToList();

            foreach (var recipeIngredient in matchingRecipesIngredients)
            {
                recipeIngredient.IngredientId = ingredientId;
            }
            await _ingredientRepository.SaveChangesAsync();
        }

        public async Task RejectPendingIngredientAsync(int suggestedIngredientId)
        {
            var allPendingIngredients = await _ingredientRepository.GetAllPendingAsync();

            var suggestion = allPendingIngredients.FirstOrDefault(s => s.Id == suggestedIngredientId);

            if (suggestion != null)
            {
                var matchingSuggestions = allPendingIngredients
                    .Where(s => StringHelper.FuzzyMatch(s.Name, suggestion.Name));

                await _ingredientRepository.RemovePendingRangeAsync(matchingSuggestions);
            }
        }

        public async Task<(bool Success, string ErrorMessage)> AddIngredientAsync(string ingredientName, List<int> selectedUnitIds)
        {
            var allIngredients = await _ingredientRepository.GetAllAsync();
            var selectedUnits = await _ingredientRepository.GetUnitsByIdsAsync(selectedUnitIds);

            var matchingIngredient = allIngredients
                    .FirstOrDefault(i => StringHelper.FuzzyMatch(i.Name, ingredientName));

            if (matchingIngredient != null)
            {
                foreach (var unit in selectedUnits)
                {
                    if (!matchingIngredient.IngredientUnits.Any(iu => iu.UnitId == unit.Id))
                    {
                        matchingIngredient.IngredientUnits.Add(new IngredientUnit
                        {
                            IngredientId = matchingIngredient.Id,
                            UnitId = unit.Id
                        });
                    }
                }

                await _ingredientRepository.SaveChangesAsync();
                return (true, "Ingredient already existed, units were added.");
            }

            var capitalizedName = StringHelper.CapitalizeFirstLetter(ingredientName);
            var newIngredient = new Ingredient
            {
                Name = capitalizedName,
                IngredientUnits = selectedUnits.Select(unit => new IngredientUnit
                {
                    UnitId = unit.Id
                }).ToList()
            };

            await _ingredientRepository.AddIngredientAsync(newIngredient);
            await _ingredientRepository.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool Success, string Message)> DeleteIngredientAsync(int id)
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(id);

            if (ingredient == null)
            {
                return (false, "Ingredient not found.");
            }

            var isInUse = await _ingredientRepository.IsIngredientUsedInRecipesAsync(id);

            if (isInUse)
            {
                var recipeNames = await _ingredientRepository.GetRecipeNamesUsingIngredientAsync(id);

                string recipeList = string.Join(", ", recipeNames);
                return (false, $"Cannot delete this ingredient as it is used in the following recipes: {recipeList}.");
            }

            await _ingredientRepository.RemoveIngredientAsync(ingredient);
            await _ingredientRepository.SaveChangesAsync();

            return (true, "Ingredient deleted successfully.");
        }
    }
}
