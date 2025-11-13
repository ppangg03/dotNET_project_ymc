namespace backend_yourmycelebrity.Dto.ArtistProfile
{
    public class ArtistDto
    {
        public int ArtistId { get; set; }
        public string? OfficialName { get; set; }
        public DateOnly? Birthdate { get; set; }
        public string? Type { get; set; }
        public string? RealName { get; set; }
        public string? Mbti { get; set; }
        public string? Bloodtype { get; set; }
        public DateOnly? DebutDate { get; set; }
        public string? Picture { get; set; }
        public int UserId { get; set; }
        public string? NativeRealName { get; set; }
        public string? HeightCm { get; set; }
        public string? WeightKg { get; set; }
        public string? Nationality { get; set; }
        public string? NativeOffcialName { get; set; }
       
    }
}
