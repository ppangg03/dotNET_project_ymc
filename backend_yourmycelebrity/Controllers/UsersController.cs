using backend_yourmycelebrity.Dto.Users;
using backend_yourmycelebrity.Models;
using backend_yourmycelebrity.Repositories.Interface;
using backend_yourmycelebrity.Services.UserService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly IJwtService _jwtService;

        public UsersController(IGenericRepository<User> repository,
            IUserRepository userRepository, 
            IConfiguration configuration,
             IJwtService jwtService
            )
        {
            _repository = repository;
            _userRepository = userRepository;
            _configuration = configuration;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> login([FromBody] LoginRequestDto loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.UsernameOrEmail))
            {
                return BadRequest("Invalid client request");
            }
            var findUser = await _userRepository.GetUserByUsernameOrEmailAsync(loginRequest.UsernameOrEmail);

            if(findUser == null)
            {
                return BadRequest("This Username Or Email not Account!");
            }

            if (!BCrypt.Net.BCrypt.Verify(
                loginRequest.Password,
                findUser.PasswordHash
            ))
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var token = _jwtService.GenerateToken(findUser);

            return Ok(new
            {
                Token = token,
                Username = findUser.Username,
                Email = findUser.Email,
                ExpiresAt = DateTime.UtcNow.AddMinutes(
            int.Parse(_configuration["Jwt:ExpireMinutes"] ?? "30"))
            });
        }

        [HttpPost("register")]
        //[Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Invalid user data");
            }

            if (await _userRepository.GetUserByEmailAsync(request.Email) != null)
            {
                return BadRequest("Email already exists!");
            }
            else if (await _userRepository.GetUserByUsernameAsync(request.Username) != null)
            {
                return BadRequest("Username already exists!");
            }

            var plainPassword = request.Password;
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            var newUser = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                CreatedAt = request.createdAt,
                Role = "customer"   //set default customer
            };

            var user = await _repository.Add(newUser);

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
            if (user == null)
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
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (user == null || id != user.UserId)
            {
                return BadRequest();
            }
            try
            {
                return Ok(await _repository.Update(id, user));
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
            if (!await _repository.Delete(id))
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}





