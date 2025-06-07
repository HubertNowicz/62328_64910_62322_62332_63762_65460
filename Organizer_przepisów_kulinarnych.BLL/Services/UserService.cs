using AutoMapper;
using System.Security.Claims;
using Organizer_przepisów_kulinarnych.DAL.Entities;
using Organizer_przepisów_kulinarnych.BLL.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Organizer_przepisów_kulinarnych.DAL.Entities.Enums;
using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;
using Organizer_przepisów_kulinarnych.BLL.Common;
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

        public async Task<Result<UserDto>> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null
                ? Result<UserDto>.Fail("User not found.")
                : Result<UserDto>.Ok(_mapper.Map<UserDto>(user));
        }

        public async Task<Result<UserDto>> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            return user == null
                ? Result<UserDto>.Fail("User not found.")
                : Result<UserDto>.Ok(_mapper.Map<UserDto>(user));
        }

        public async Task<Result> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return Result.Fail("User not found.");

            if (user.UserRole == UserRole.Admin)
                return Result.Fail("Cannot delete an Admin account.");

            await _userRepository.DeleteAsync(user);
            await _userRepository.SaveChangesAsync();
            return Result.Ok();
        }

        public async Task<Result> UpdateUserAsync(UserDto updatedDto)
        {
            var user = await _userRepository.GetByIdAsync(updatedDto.Id);
            if (user == null)
                return Result.Fail("User not found.");

            user.Email = updatedDto.Email;
            user.FirstName = updatedDto.FirstName;
            user.Surname = updatedDto.Surname;
            user.Username = updatedDto.Username;

            await _userRepository.SaveChangesAsync();
            return Result.Ok();
        }

        public async Task<Result<List<User>>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return Result<List<User>>.Ok(users);
        }

        public async Task<Result<int>> GetCurrentUserIdAsync(ClaimsPrincipal principal)
        {
            var username = principal.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Result<int>.Fail("User is not authenticated.");

            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
                return Result<int>.Fail("User not found.");

            return Result<int>.Ok(user.Id);
        }

        public async Task<Result> RegisterUserAsync(UserRegistrationDto dto)
        {
            if (await _userRepository.UsernameExistsAsync(dto.Username))
                return Result.Fail("Username is already taken.");

            if (await _userRepository.EmailExistsAsync(dto.Email))
                return Result.Fail("Email is already in use.");

            var user = _mapper.Map<User>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.UserRole = UserRole.User;

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return Result.Ok();
        }

        public Result<User> ValidateCredentials(string username, string password)
        {
            var user = _userRepository.GetByUsernameAsync(username).Result;

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return Result<User>.Fail("Invalid credentials.");

            return Result<User>.Ok(user);
        }

        public Result<ClaimsPrincipal> AuthenticateUser(string username, string password)
        {
            var userResult = ValidateCredentials(username, password);
            if (!userResult.Success)
                return Result<ClaimsPrincipal>.Fail(userResult.Error);

            var user = userResult.Data;
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Role, user.UserRole.ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return Result<ClaimsPrincipal>.Ok(new ClaimsPrincipal(identity));
        }

        public async Task<Result> CreateAsync(User user)
        {
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
            return Result.Ok();
        }
    }
}
