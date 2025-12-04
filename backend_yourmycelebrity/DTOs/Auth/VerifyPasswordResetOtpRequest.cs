using System.ComponentModel.DataAnnotations;

namespace backend_yourmycelebrity.Dto.Otp
{
    public class VerifyPasswordResetOtpRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6)]
        [RegularExpression(@"^\d{6}$")]
        public string Otp { get; set; }
    }
}
