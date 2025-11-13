using backend_yourmycelebrity.Data;
using backend_yourmycelebrity.Dto.ArtistProfile;
using backend_yourmycelebrity.Models;
using backend_yourmycelebrity.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace backend_yourmycelebrity.Repositories.Implementations
{
    public class ArtistProfileRepository : IArtistProfileRepository
    {
        private readonly AppDbContext _context;

        public ArtistProfileRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ArtistProfile?> GetByIdAsync(int id)
        {
            return await _context.ArtistProfiles
                .FirstOrDefaultAsync(a => a.ArtistId == id);
        }
        public async Task<ArtistProfile?> UpdateAsync(int id, UpdateArtistDto dto)
        {
            var existing = await _context.ArtistProfiles.FindAsync(id);

            if (existing == null)
                return null;

            existing.OfficialName = dto.OfficialName;
            existing.Birthdate = dto.Birthdate;
            existing.Type = dto.Type;
            existing.RealName = dto.RealName;
            existing.Mbti = dto.Mbti;
            existing.Bloodtype = dto.Bloodtype;
            existing.DebutDate = dto.DebutDate;
            existing.Picture = dto.Picture;
            existing.UserId = dto.UserId;
            existing.NativeRealName = dto.NativeRealName;
            existing.HeightCm = dto.HeightCm;
            existing.WeightKg = dto.WeightKg;
            existing.Nationality = dto.Nationality;
            existing.NativeOffcialName = dto.NativeOffcialName;

            await _context.SaveChangesAsync();
            return existing;
        } 
    }
}
