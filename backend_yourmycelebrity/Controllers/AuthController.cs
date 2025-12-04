using backend_yourmycelebrity.Data;
using backend_yourmycelebrity.Dto.Otp;
using backend_yourmycelebrity.DTOs.Auth;
using backend_yourmycelebrity.Models;
using backend_yourmycelebrity.Models.Enum;
using backend_yourmycelebrity.Repositories.Interface;
using backend_yourmycelebrity.Services.Interface;
using backend_yourmycelebrity.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace backend_yourmycelebrity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IGenericRepository<User> _repository;
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IJwtService _jwtService;
       
        public AuthController(IGenericRepository<User> repository,
             IAuthRepository authRepository,
             IConfiguration configuration,
             IEmailService emailService,
             IJwtService jwtService
             )
           
        {
            _repository = repository;
            _authRepository = authRepository;
            _configuration = configuration;
            _emailService = emailService;
            _jwtService = jwtService;
           
        }

        [HttpPost("login")]
        public async Task<IActionResult> login([FromBody] LoginRequestDto loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.UsernameOrEmail))
            {
                return BadRequest("Invalid client request");
            }
            var findUser = await _authRepository.GetUserByUsernameOrEmailAsync(loginRequest.UsernameOrEmail);

            if (findUser == null)
            {
                return BadRequest("This Username Or Email not Account!");
            }

            if (!BCrypt.Net.BCrypt.Verify(
                loginRequest.Password,
                findUser.PasswordHash
            ))
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var token = _jwtService.GenerateToken(findUser);

            return Ok(new
            {
                Token = token,
                Username = findUser.Username,
                Email = findUser.Email,
                ExpiresAt = DateTime.UtcNow.AddMinutes(
            int.Parse(_configuration["Jwt:ExpireMinutes"] ?? "30"))
            });
        }

        // Logout - Blacklist Token
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest(new { message = "Token not found" });
                }

                // Parse token
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var userId = int.Parse(jwtToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

                // add in blacklist
                var blacklistedToken = new SecurityToken
                {
                    TokenType = TokenType.Blacklist,
                    UserId = userId,
                    Token = token,
                    Purpose = TokenPurpose.Logout,
                    CreatedAt = DateTime.UtcNow,
                    BlacklistedAt = DateTime.UtcNow,
                    ExpiresAt = jwtToken.ValidTo,
                    IsUsed = true
                };

                await _authRepository.SetTokenForResetPassword(blacklistedToken);

                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Logout failed", error = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try 
            {
               
                var existingUser = await _authRepository.GetUserByUsernameOrEmail(request.Username,request.Email);
                if (existingUser != null)
                {
                    if (existingUser.Username == request.Username)
                        return BadRequest(new { message = "Username already exists" });
                    else
                        return BadRequest(new { message = "Email already registered" });
                }

                var otpToken = await _authRepository.GetTokenOTP(request.Email, request.Otp);
                if (otpToken == null)
                    return BadRequest(new { message = "Invalid or expired OTP" });

                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                var newUser = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    CreatedAt = request.createdAt,
                    Role = "customer",   //set default customer
                    //IsEmailVerified = true
                };
                await _repository.Add(newUser);

                // 5️⃣ อัพเดท OTP
                otpToken.IsUsed = true;
                otpToken.UserId = newUser.UserId;
                await _authRepository.UpdateToken(otpToken);

                return Ok(new { Message = "User registered successfully", UserId = newUser.UserId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Registration failed", error = ex.Message});
            }
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOTP([FromBody] SendOtpRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (string.IsNullOrEmpty(request.Email))
                {
                    return BadRequest(new { message = "Email is required" });
                }

                var existingUser = await _authRepository.GetUserByEmailAsync(request.Email);
                if(existingUser == null)
                {
                    return BadRequest(new {message = "This Email already registered"});
                   
                }
                //create OTP
                var random = new Random();
                var otpCode = random.Next(100000, 999999).ToString();

                Console.WriteLine($"Generated OTP for {request.Email}: {otpCode}");

                // remove old otp
                await _authRepository.RemoveOldTokenForRegister(otpCode);

                //new OTP
                var now = DateTime.UtcNow;
                var token = new SecurityToken
                {
                    TokenType = TokenType.OTP,
                    Email = request.Email,
                    Token = otpCode,
                    Purpose = TokenPurpose.Register,
                    CreatedAt = now,
                    ExpiresAt = now.AddMinutes(10),  // หมดอายุใน 10 นาที
                    IsUsed = false,
                    IsVerified = false
                };
                await _authRepository.AddOtpToken(token);
                Console.WriteLine($"OTP saved to database: {otpCode}");

                //sent Email
                try
                {
                    await _emailService.SendOtpEmailAsync(request.Email, otpCode);
                    Console.WriteLine($"OTP email sent successfully to {request.Email}");
                }
                catch (Exception emailEx)
                {
                    Console.WriteLine($"Failed to send email: {emailEx.Message}");
                    //always save otp
                }

                return Ok(new
                {
                    message = "OTP sent successfully",
                    email = request.Email,
                    // ไม่ควรส่ง otp กลับมา
                    // otp = otpCode only dev testing
                });
                // กลับมาทำต่อ


            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to send reset code", error = ex.Message });
            }
        }


        // Controllers/AuthController.cs
        [HttpPost("password-reset/send-otp")]
        public async Task<IActionResult> SendPasswordResetOtp([FromBody] SendOtpRequest request)
        {
            try
            {
                // ตรวจสอบว่า email มีในระบบหรือไม่
                var user = await _authRepository.GetUserByEmailAsync(request.Email);

                if (user == null)
                {
                    return BadRequest(new { message = "Email not found in our system" });
                }

                // สร้าง OTP code (6 หลัก)
                var random = new Random();
                var otpCode = random.Next(100000, 999999).ToString();

                //Delete oldOtp this Email
                await _authRepository.RemoveOleTokenForResetPassword(request.Email);

                // บันทึก OTP ใหม่
                var resetToken = new SecurityToken
                {
                    TokenType = TokenType.OTP,
                    UserId = user.UserId,
                    Email = request.Email,
                    Token = otpCode,
                    Purpose = TokenPurpose.PasswordReset,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                    IsUsed = false,
                    IsVerified = false
                };

                await _authRepository.SetTokenForResetPassword(resetToken);

                // ส่ง OTP ทาง email
                await _emailService.SendPasswordResetOtpAsync(request.Email, otpCode, user.Username);

                return Ok(new { message = "Password reset code sent to your email" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to send reset code", error = ex.Message });
            }
        }

        [HttpPost("password-reset/verify-otp")]
        public async Task<IActionResult> VerifyPasswordResetOtp([FromBody] VerifyPasswordResetOtpRequest request)
        {
            try
            {
                // check OTP -> AuthRepository
                var resetToken = await _authRepository.checkOtp(request.Email,request.Otp);

                if (resetToken == null)
                {
                    return BadRequest(new { message = "Invalid or expired OTP code" });
                }
                
                return Ok(new { message = "OTP verified successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Verification failed", error = ex.Message });
            }
        }

        [HttpPost("password-reset/reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                // check otp
                var resetToken = await _authRepository.ResetPasswordTokenRequest(request.Email,request.Otp);
                // ทำเครื่องหมายว่า token ถูกใช้แล้ว
                //resetToken.IsUsed = true;
                //await _context.SaveChangesAsync();
                if (resetToken == null)
                {
                    return BadRequest(new { message = "Invalid or expired reset token" });
                }
                // update password
                //var user = resetToken.UserId;
                var user = await _repository.GetByID(resetToken.UserId);
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                await _repository.Update(resetToken.UserId, user);
                // send Email Password is change!
                await _emailService.SendPasswordChangedNotificationAsync(user.Email, user.Username);

                return Ok(new { message = "Password reset successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Password reset failed", error = ex.Message });
            }
        }
    }
}
