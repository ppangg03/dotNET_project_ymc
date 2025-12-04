
using backend_yourmycelebrity.Models;
using backend_yourmycelebrity.Repositories.Interface;
using backend_yourmycelebrity.Services.Interface;
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
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private readonly IJwtService _jwtService;

        public UsersController(IGenericRepository<User> repository,
            IAuthRepository authRepository, 
            IConfiguration configuration,
             IJwtService jwtService
            )
        {
            _repository = repository;
            _authRepository = authRepository;
            _configuration = configuration;
            _jwtService = jwtService;
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





