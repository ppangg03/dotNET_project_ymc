using backend_yourmycelebrity.Models;

namespace backend_yourmycelebrity.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail, string password);
        Task<User?> GetUserByUsernameAsync(string username);
    }
}
