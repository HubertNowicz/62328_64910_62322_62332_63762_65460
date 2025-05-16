using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;
using Organizer_przepisów_kulinarnych.DAL.Entities;
using System.Security.Claims;

namespace Organizer_przepisów_kulinarnych.BLL.Interfaces
{
    public interface IUserService
    {
        User? ValidateCredentials(string username, string password);
        ClaimsPrincipal? AuthenticateUser(string username, string password);
        Task<RegistrationResult> RegisterUserAsync(UserRegistrationDto dto);
        User? GetByUsername(string username);
        Task CreateAsync(User user);
        Task<int> GetCurrentUserIdAsync(ClaimsPrincipal user);
        Task<List<User>> GetAllUsersAsync();
    }
}
