using backend_yourmycelebrity.Data;
using backend_yourmycelebrity.Models;
using backend_yourmycelebrity.Repositories.Interface;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace backend_yourmycelebrity.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail, string password)
        {
             return await _context.Users.FirstOrDefaultAsync(u =>
                (u.Username == usernameOrEmail || u.Email == usernameOrEmail)
                && u.Password == password);
        }
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(a => a.Username == username);
        }
    }
}
