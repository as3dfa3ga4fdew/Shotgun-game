using Microsoft.EntityFrameworkCore;
using Server.Contexts;
using Server.Models.Entities;
using Server.Repositories.Interfaces;

namespace Server.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserEntity> CreateAsync(UserEntity user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return await GetAsync(user.Username);
        }

        public async Task<UserEntity> GetAsync(string username)
        {
            return await _context.Users
                .Include(x => x.UserType)
                .Where(x => x.Username == username).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(UserEntity user)
        {
            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string username)
        {
            return await _context.Users.Where(x => x.Username == username).FirstOrDefaultAsync() != null ? true : false; 
        }
    }
}
