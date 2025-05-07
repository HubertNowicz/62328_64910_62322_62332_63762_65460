using Organizer_przepisów_kulinarnych.DAL.Entities;

namespace Organizer_przepisów_kulinarnych.BLL.Interfaces
{
    public interface IAdminService
    {
        Task ApprovePendingIngredientAsync(int suggestedIngredientId);
        Task RejectPendingIngredientAsync(int suggestedIngredientId);
        Task<List<PendingIngredient>> GetAllPendingIngredientsAsync();
        Task<(bool Success, string ErrorMessage)> AddIngredientAsync(string ingredientName);
        Task<(bool Success, string Message)> DeleteIngredientAsync(int id);
    }
}
