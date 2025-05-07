using Organizer_przepisów_kulinarnych.DAL.DbContexts;
using Organizer_przepisów_kulinarnych.DAL.Entities;
using Organizer_przepisów_kulinarnych.BLL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Organizer_przepisów_kulinarnych.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public User? ValidateCredentials(string username, string password)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return user;
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

    }
}
