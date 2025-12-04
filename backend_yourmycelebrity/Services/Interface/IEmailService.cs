namespace backend_yourmycelebrity.Services.Interface
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string email,string otpCcode);
        Task SendPasswordResetOtpAsync(string email, string otpCode, string username);
        Task SendPasswordChangedNotificationAsync(string email, string username);
    

    }
}
