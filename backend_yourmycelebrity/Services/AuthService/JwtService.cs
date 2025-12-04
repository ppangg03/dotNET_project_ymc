
using backend_yourmycelebrity.Models;
using backend_yourmycelebrity.Services.Interface;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend_yourmycelebrity.Services.UserService
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        
   

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
          
           
        }
        public string GenerateToken(User user)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "A1B2C3D4E5F6G7H8I9J0K1L2M3N4O5P6"));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
               new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
               new Claim(ClaimTypes.Name, user.Username),
               new Claim(ClaimTypes.Email, user.Email)
            };

            var tokenOptions = new JwtSecurityToken(
                  issuer: _configuration["Jwt:Issuer"] ?? "https://localhost:7008",
                    audience: _configuration["Jwt:Audience"] ?? "https://localhost:7008",
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(30),
                    signingCredentials: signingCredentials
                    );

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            
        }
    }
}
