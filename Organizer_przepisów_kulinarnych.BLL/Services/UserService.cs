using Organizer_przepisów_kulinarnych.DAL.DbContexts;
using Organizer_przepisów_kulinarnych.DAL.Entities;
using Organizer_przepisów_kulinarnych.BLL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Organizer_przepisów_kulinarnych.DAL.Entities.Enums;
using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;
using AutoMapper;
using Organizer_przepisów_kulinarnych.DAL.Interfaces;

namespace Organizer_przepisów_kulinarnych.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.UserRole == UserRole.Admin)
            {
                return false;
            }

            await _userRepository.DeleteAsync(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserAsync(UserDto updatedDto)
        {
            var user = await _userRepository.GetByIdAsync(updatedDto.Id);
            if (user == null)
            {
                return false;
            }

            user.Email = updatedDto.Email;
            user.FirstName = updatedDto.FirstName;
            user.Surname = updatedDto.Surname;
            user.Username = updatedDto.Username;

            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<int> GetCurrentUserIdAsync(ClaimsPrincipal principal)
        {
            var username = principal.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                throw new InvalidOperationException("User is not authenticated.");
            }

            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            return user.Id;
        }

        public async Task<RegistrationResult> RegisterUserAsync(UserRegistrationDto dto)
        {
            if (await _userRepository.UsernameExistsAsync(dto.Username))
            {
                return RegistrationResult.Failed("Username is already taken.");
            }

            if (await _userRepository.EmailExistsAsync(dto.Email))
            {
                return RegistrationResult.Failed("Email is already in use.");
            }

            var user = _mapper.Map<User>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.UserRole = UserRole.User;

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return RegistrationResult.Successful(user);
        }

        public User? ValidateCredentials(string username, string password)
        {
            var user = _userRepository.GetByUsernameAsync(username).Result;
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return null;
            }

            return user;
        }

        public ClaimsPrincipal? AuthenticateUser(string username, string password)
        {
            var user = ValidateCredentials(username, password);
            if (user == null)
            {
                return null;
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Role, user.UserRole.ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(identity);
        }

        // for seeder
        public async Task CreateAsync(User user)
        {
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
        }

    }
}
