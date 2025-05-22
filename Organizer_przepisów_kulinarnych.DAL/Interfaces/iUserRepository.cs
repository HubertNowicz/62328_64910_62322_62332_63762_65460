using Organizer_przepisów_kulinarnych.DAL.Entities;

namespace Organizer_przepisów_kulinarnych.DAL.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByUsernameAsync(string username);
        Task<List<User>> GetAllAsync();
        Task AddAsync(User user);
        Task DeleteAsync(User user);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task SaveChangesAsync();
    }
}
