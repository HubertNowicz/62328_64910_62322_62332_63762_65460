using Microsoft.EntityFrameworkCore;
using Organizer_przepisów_kulinarnych.BLL.Common;
using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;
using Organizer_przepisów_kulinarnych.DAL.Entities;
using System.Security.Claims;

namespace Organizer_przepisów_kulinarnych.BLL.Interfaces
{
    public interface IUserService
    {
        Result<User> ValidateCredentials(string username, string password);
        Result<ClaimsPrincipal> AuthenticateUser(string username, string password);
        Task<Result> RegisterUserAsync(UserRegistrationDto dto);
        Task<Result> CreateAsync(User user);
        Task<Result<UserDto>> GetUserByUsernameAsync(string username);
        Task<Result<int>> GetCurrentUserIdAsync(ClaimsPrincipal user);
        Task<Result<List<User>>> GetAllUsersAsync();
        Task<Result<UserDto>> GetUserByIdAsync(int id);
        Task<Result> DeleteUserAsync(int id);
        Task<Result> UpdateUserAsync(UserDto updatedDto);
    }
}
