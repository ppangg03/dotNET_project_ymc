using backend_yourmycelebrity.Dto.ArtistProfile;
using backend_yourmycelebrity.Models;
using backend_yourmycelebrity.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend_yourmycelebrity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistProfilesController : ControllerBase
    {
        private readonly IGenericRepository<ArtistProfile> _repository;
        private readonly IArtistProfileRepository _artistProfileRepository;

        public ArtistProfilesController(IGenericRepository<ArtistProfile> repository, IArtistProfileRepository artistProfileRepository)
        {
            _repository = repository;
            _artistProfileRepository = artistProfileRepository;
        }

        //GET
        [HttpGet]
        public async Task<IActionResult> GetAllArtist()
        {
            return Ok( await _repository.GetAllWithInclude());
        }
        //GET: api/
        [HttpGet("{id}")]
        public async Task<IActionResult> GetArtistById(int id)
        {
            return Ok( await _repository.GetByID(id));
        }
        //POST
        [HttpPost]
        public async Task<IActionResult> PostNewArtist(ArtistProfile artist)
        {
            if (artist == null)
            {
                return BadRequest();
            }
            try
            {
                return Ok(await _repository.Add(artist));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        //PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArtist(int id, [FromBody] UpdateArtistDto model)
        {
            if (id != model.ArtistId)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var updated = await _artistProfileRepository.UpdateAsync(id, model);

                if (updated == null)
                    return NotFound();

                return Ok(updated);
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

        //DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArtist(int id)
        {
            if(!await _repository.Delete(id))
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}
