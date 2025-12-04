
using backend_yourmycelebrity.Models;

namespace backend_yourmycelebrity.Services.Interface
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
