using Organizer_przepisów_kulinarnych.DAL.DbContexts;
using Organizer_przepisów_kulinarnych.DAL.Entities;
using Organizer_przepisów_kulinarnych.BLL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Organizer_przepisów_kulinarnych.DAL.Entities.Enums;
using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;
using AutoMapper;

namespace Organizer_przepisów_kulinarnych.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserService(IMapper mapper, ApplicationDbContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        public User? ValidateCredentials(string username, string password)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == username);
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

        public User? GetByUsername(string username)
        {
            return _context.Users.SingleOrDefault(u => u.Username == username);
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<int> GetCurrentUserIdAsync(ClaimsPrincipal user)
        {
            var username = user.Identity?.Name;

            if (string.IsNullOrEmpty(username))
            {
                throw new InvalidOperationException("User is not authenticated.");
            }

            var userEntity = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (userEntity == null)
            {
                throw new Exception("User not found.");
            }

            return userEntity.Id;
        }
        public async Task CreateAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        public async Task<RegistrationResult> RegisterUserAsync(UserRegistrationDto dto)
        {
            if (_context.Users.Any(u => u.Username == dto.Username))
                return RegistrationResult.Failed("Username is already taken.");

            if (_context.Users.Any(u => u.Email == dto.Email))
                return RegistrationResult.Failed("Email is already in use.");

            var user = _mapper.Map<User>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.UserRole = UserRole.User;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RegistrationResult.Successful(user);
        }

    }
}
