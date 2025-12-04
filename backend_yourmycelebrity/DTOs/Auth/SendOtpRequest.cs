using System.ComponentModel.DataAnnotations;

namespace backend_yourmycelebrity.Dto.Otp
{
    public class SendOtpRequest
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
