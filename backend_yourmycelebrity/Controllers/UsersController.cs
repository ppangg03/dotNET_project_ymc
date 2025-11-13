using backend_yourmycelebrity.Dto.Users;
using backend_yourmycelebrity.Models;
using backend_yourmycelebrity.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace backend_yourmycelebrity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IGenericRepository<User> _repository;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UsersController(IGenericRepository<User> repository,IUserRepository userRepository, IConfiguration configuration)
        {
            _repository = repository;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> login([FromBody] LoginRequestDto loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.UsernameOrEmail))
            {
                return BadRequest("Invalid client request");
            }
            
            var findUser = await _userRepository.GetUserByUsernameOrEmailAsync(
                    loginRequest.UsernameOrEmail,loginRequest.Password);

            if (findUser != null)
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "A1B2C3D4E5F6G7H8I9J0K1L2M3N4O5P6"));
                var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
                {
                new Claim(ClaimTypes.NameIdentifier, findUser.UserId.ToString()),
                new Claim(ClaimTypes.Name, findUser.Username),
                new Claim(ClaimTypes.Email, findUser.Email)
                };

                var tokenOptions = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"] ?? "https://localhost:7008",
                    audience: _configuration["Jwt:Audience"] ?? "https://localhost:7008",
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(30),
                    signingCredentials: signingCredentials
                    );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                return Ok(new 
                {
                    Token = tokenString,
                    Username = findUser.Username,
                    Email = findUser.Email,
                    ExpiresAt = tokenOptions.ValidTo
                });
            }
            return Unauthorized(new { Message = "Invalid username or password" });

        }

        [HttpGet]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (user == null || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Invalid user data");
            }

            var existingUser = await _userRepository.GetUserByUsernameAsync(user.Username);
            if (existingUser != null)
            {
                return BadRequest("Username already exists");
            }

            //user.CreatedAt = DateTime.UtcNow;
            var newUser = await _repository.Add(user);

            return Ok(new { Message = "User registered successfully", UserId = newUser.UserId });
        }

        //GET: api/Users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repository.GetAllWithInclude(a => a.ArtistProfiles);
            return Ok(users);
            
        }

        //GET: api/User/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _repository.GetByID(id);
            return Ok(user);
        }

        //POST: api/Users
        [HttpPost]
        public async Task<IActionResult> PostUser(User user)
        {
            if ( user == null)
            {
                return BadRequest();
            }
            try
            {
               return Ok(await _repository.Add(user));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        //PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id,User user)
        {
            if (user == null || id != user.UserId)
            {
                return BadRequest();
            }
            try
            {
                return Ok(await _repository.Update(id,user));
            }
            catch (ArgumentException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        //Delete: api/Users/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if(!await _repository.Delete(id))
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}





