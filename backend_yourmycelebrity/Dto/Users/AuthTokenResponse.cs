namespace backend_yourmycelebrity.Dto.Users
{
    public class AuthTokenResponse
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
