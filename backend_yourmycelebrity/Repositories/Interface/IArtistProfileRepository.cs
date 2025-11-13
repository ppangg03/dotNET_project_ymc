using backend_yourmycelebrity.Dto.ArtistProfile;
using backend_yourmycelebrity.Models;

namespace backend_yourmycelebrity.Repositories.Interface
{
    public interface IArtistProfileRepository
    {
        Task<ArtistProfile?> GetByIdAsync(int id);
        Task<ArtistProfile?> UpdateAsync(int id, UpdateArtistDto artist);

    }
}
