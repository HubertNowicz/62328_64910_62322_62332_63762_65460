using Organizer_przepisów_kulinarnych.BLL.Common;
using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;

namespace Organizer_przepisów_kulinarnych.BLL.Interfaces
{
    public interface IAdminService
    {
        Task<Result<List<PendingIngredientDto>>> GetAllPendingIngredientsAsync();
        Task<Result> ApprovePendingIngredientAsync(int suggestedIngredientId);
        Task<Result> RejectPendingIngredientAsync(int suggestedIngredientId);
        Task<Result> AddIngredientAsync(string ingredientName, List<int> selectedUnitIds);
        Task<Result> DeleteIngredientAsync(int id);
    }
}
