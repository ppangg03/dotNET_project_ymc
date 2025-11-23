using backend_yourmycelebrity.Models;

namespace backend_yourmycelebrity.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail);

        Task<User?> GetUserByUsernameOrPasswordAsync( string usernameOrEmail, string password );
    }
}
