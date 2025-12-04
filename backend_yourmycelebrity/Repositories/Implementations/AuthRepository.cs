using backend_yourmycelebrity.Data;
using backend_yourmycelebrity.Dto.Otp;
using backend_yourmycelebrity.Models;
using backend_yourmycelebrity.Models.Enum;
using backend_yourmycelebrity.Repositories.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SecurityToken = backend_yourmycelebrity.Models.SecurityToken;

namespace backend_yourmycelebrity.Repositories.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<User?> GetUserByEmailAsync(string email )
        {
             return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(a => a.Username == username);
        }
        public async Task<User?> GetUserByUsernameOrEmail(string username,string email)
        {
            return await _context.Users.FirstOrDefaultAsync(a => a.Username == username || a.Email == email);
        }

        public async Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);
        }
        public async Task<User?> GetUserByUsernameOrPasswordAsync(string usernameOrEmail,string passwordHash)
        {
            return await _context.Users.FirstOrDefaultAsync(u => 
            (u.Username == usernameOrEmail || u.Email == usernameOrEmail)
            && u.PasswordHash == passwordHash);
        }

        //Delete OldOtp For reset Password
        public async Task RemoveOleTokenForResetPassword(string email)
        {
            var oldTokens = await _context.SecurityTokens
                .Where(t => t.Email == email
                && t.TokenType == TokenType.OTP
                && t.Purpose == TokenPurpose.PasswordReset
                && t.IsUsed == false)
                .ToListAsync();

            _context.SecurityTokens.RemoveRange(oldTokens);
            await _context.SaveChangesAsync();

        }
        //Remove OldOtp For register
        public async Task RemoveOldTokenForRegister(string email)
        {
            var oldTokens = await _context.SecurityTokens
                .Where(t => t.Email == email
                && t.TokenType == TokenType.OTP
                && t.Purpose == TokenPurpose.Register
                && t.IsUsed == false)
                .ToListAsync();

            if (oldTokens.Any())
            {
                _context.SecurityTokens.RemoveRange(oldTokens);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Removed {oldTokens.Count} old OTP tokens");
            }
        }


        public async Task SetTokenForResetPassword(SecurityToken newOtpToeken)
        {
            _context.SecurityTokens.Add(newOtpToeken);
            await _context.SaveChangesAsync();
        }


        public async Task<SecurityToken?> checkOtp(string email,string otp)
        {
            var resetToken = await _context.SecurityTokens
                .Where(t => t.Email == email
               && t.Token == otp
               && t.TokenType == Models.Enum.TokenType.OTP
               && t.Purpose == Models.Enum.TokenPurpose.PasswordReset
               && t.IsUsed == false
               && t.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();
            if (resetToken == null)
            {
                return null;
            }
            // ทำเครื่องหมายว่า OTP ถูก verify แล้ว (แต่ยังไม่ used)
            resetToken.IsVerified = true;
            await _context.SaveChangesAsync();

            return resetToken;
        }

        public async Task<SecurityToken?> ResetPasswordTokenRequest(string email , string otp)
        {
            var resetToken = await _context.SecurityTokens
                    .Include(t => t.User)
                    .Where(t => t.Email == email
                           && t.Token == otp
                           && t.TokenType == Models.Enum.TokenType.OTP
                           && t.Purpose == Models.Enum.TokenPurpose.PasswordReset
                           && t.IsUsed == false
                           && t.IsVerified == true
                           && t.ExpiresAt > DateTime.UtcNow)
                    .OrderByDescending(t => t.CreatedAt)
                    .FirstOrDefaultAsync();

            if (resetToken == null)
            {
                return null;
            }
            // Mark token IsUsed
            //resetToken.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            resetToken.IsUsed = true;
            await _context.SaveChangesAsync();
            return resetToken;
        }
        public async Task<SecurityToken> GetTokenOTP(string email,string otp)
        {
            var otpToken = await _context.SecurityTokens
               .Where(t => t.Email == email
                      && t.Token == otp
                      && t.TokenType == TokenType.OTP
                      && t.Purpose == TokenPurpose.Register
                      && t.IsUsed == false
                      && t.ExpiresAt > DateTime.UtcNow)
               .OrderByDescending(t => t.CreatedAt)
               .FirstOrDefaultAsync();

            return otpToken;
        }

        public async Task UpdateToken(SecurityToken securityToken)
        {
            _context.SecurityTokens.Update(securityToken);
            await _context.SaveChangesAsync();
        }

        public async Task AddOtpToken(SecurityToken securityToken)
        {
            _context.SecurityTokens.Add(securityToken);
            await _context.SaveChangesAsync();

            
        }

    }
}
