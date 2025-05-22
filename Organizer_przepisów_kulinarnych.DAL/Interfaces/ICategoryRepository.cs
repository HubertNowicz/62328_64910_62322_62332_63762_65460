using Organizer_przepisów_kulinarnych.DAL.Entities;

namespace Organizer_przepisów_kulinarnych.DAL.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync();
    }
}
