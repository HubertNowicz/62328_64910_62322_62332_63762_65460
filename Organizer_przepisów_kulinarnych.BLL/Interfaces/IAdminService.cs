using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;
using Organizer_przepisów_kulinarnych.DAL.Entities;

namespace Organizer_przepisów_kulinarnych.BLL.Interfaces
{
    public interface IAdminService
    {
        Task ApprovePendingIngredientAsync(int suggestedIngredientId);
        Task RejectPendingIngredientAsync(int suggestedIngredientId);
        Task<List<PendingIngredientDto>> GetAllPendingIngredientsAsync();
        Task<(bool Success, string? ErrorMessage)> AddIngredientAsync(string ingredientName, List<int> selectedUnitIds);
        Task<(bool Success, string Message)> DeleteIngredientAsync(int id);
    }
}
