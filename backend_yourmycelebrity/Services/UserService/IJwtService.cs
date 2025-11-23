using backend_yourmycelebrity.Dto.Users;
using backend_yourmycelebrity.Models;

namespace backend_yourmycelebrity.Services.UserService
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
