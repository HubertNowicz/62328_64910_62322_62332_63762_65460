
using Microsoft.EntityFrameworkCore;
using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;
using Organizer_przepisów_kulinarnych.BLL.Interfaces;
using Organizer_przepisów_kulinarnych.DAL.DbContexts;
using Organizer_przepisów_kulinarnych.DAL.Entities;

namespace Organizer_przepisów_kulinarnych.BLL.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly ApplicationDbContext _context;

        public AdminUserService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }


        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }
        //public async Task<List<UserDto>> GetAllUsersAsync()
        //{
        //    return await _context.Users
        //        .Select(u => new UserDto
        //        {
        //            Id = u.Id,
        //            Email = u.Email,
        //            FirstName = u.FirstName,
        //            Surname = u.Surname,
                
        //        }).ToListAsync();
        //}
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserAsync(User updatedUser)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == updatedUser.Id);
            if (user == null) return false;

            user.Email = updatedUser.Email;
            user.FirstName = updatedUser.FirstName;
            user.Surname = updatedUser.Surname;
        

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
