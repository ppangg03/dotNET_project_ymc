namespace backend_yourmycelebrity.Dto.Users
{
    public class RegisterRequestDto
    {
        
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public DateOnly createdAt { get; set; }
    }
}
