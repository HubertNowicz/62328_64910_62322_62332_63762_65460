using Microsoft.EntityFrameworkCore;
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
        Task CreateAsync(User user);
        Task<UserDto> GetUserByUsernameAsync(string username);
        Task<int> GetCurrentUserIdAsync(ClaimsPrincipal user);
        Task<List<User>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(int id);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> UpdateUserAsync(UserDto updatedDto);
    }
}