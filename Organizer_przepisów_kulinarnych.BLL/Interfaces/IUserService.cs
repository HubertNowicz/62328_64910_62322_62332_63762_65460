using Organizer_przepisów_kulinarnych.DAL.Entities;

namespace Organizer_przepisów_kulinarnych.BLL.Interfaces
{
    public interface IUserService
    {
        User? ValidateCredentials(string username, string password);
        User? GetByUsername(string username);
        Task CreateAsync(User user);
    }
}
