using backend_yourmycelebrity.Dto.Otp;
using backend_yourmycelebrity.Models;

namespace backend_yourmycelebrity.Repositories.Interface
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByUsernameAsync(string username);

        Task<User?> GetUserByUsernameOrEmail(string username, string email);
        Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail);
        Task<User?> GetUserByUsernameOrPasswordAsync( string usernameOrEmail, string password );

        //reset password
        Task RemoveOleTokenForResetPassword(string email);
        Task RemoveOldTokenForRegister(string email)

        //set token
        Task SetTokenForResetPassword(SecurityToken newOtpToekn);

        //
        Task<SecurityToken?> checkOtp(string email, string otp);
        Task<SecurityToken?> ResetPasswordTokenRequest(string email, string otp);
        Task<SecurityToken> GetTokenOTP(string email, string otp);
        Task UpdateToken(SecurityToken securityToken);
        Task AddOtpToken(SecurityToken securityToken);
    }
}
