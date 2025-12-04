namespace backend_yourmycelebrity.DTOs.Auth
{
    public class AuthTokenResponse
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
