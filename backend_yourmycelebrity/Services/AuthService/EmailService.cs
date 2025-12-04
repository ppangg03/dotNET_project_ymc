using backend_yourmycelebrity.Services.Interface;
using System.Net;
using System.Net.Mail;

namespace backend_yourmycelebrity.Services.UserService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendOtpEmailAsync(string email, string otpCode)
        {
            var smtpHost = _configuration["Email:SmtpHost"];
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"]);
            var smtpUsername = _configuration["Email:SmtpUsername"];
            var smtpPassword = _configuration["Email:SmtpPassword"];
            var fromEmail = _configuration["Email:FromEmail"];
            var fromName = _configuration["Email:FromName"];

            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = "YMC - Email Verification Code",
                    Body = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                                <h2 style='color: #667eea;'>Email Verification</h2>
                                <p>Your verification code is:</p>
                                <div style='background: #f5f5f5; padding: 20px; text-align: center; font-size: 32px; font-weight: bold; letter-spacing: 5px; color: #667eea;'>
                                    {otpCode}
                                </div>
                                <p style='color: #666; margin-top: 20px;'>This code will expire in 10 minutes.</p>
                                <p style='color: #666;'>If you didn't request this code, please ignore this email.</p>
                                <hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;'>
                                <p style='color: #999; font-size: 12px;'>© 2024 YMC. All rights reserved.</p>
                            </div>
                        </body>
                        </html>
                    ",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
            }
        }

        public async Task SendPasswordChangedNotificationAsync(string email, string username)
        {
            var smtpHost = _configuration["Email:SmtpHost"];
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"]);
            var smtpUsername = _configuration["Email:SmtpUsername"];
            var smtpPassword = _configuration["Email:SmtpPassword"];
            var fromEmail = _configuration["Email:FromEmail"];
            var fromName = _configuration["Email:FromName"];

            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = "YMC - Password Changed Successfully",
                    Body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #28a745;'>Password Changed Successfully</h2>
                        <p>Hi {username},</p>
                        <p>Your password has been changed successfully.</p>
                        <p style='color: #666;'>If you didn't make this change, please contact us immediately.</p>
                        <p style='color: #666; margin-top: 20px;'>Changed at: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>
                        <hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;'>
                        <p style='color: #999; font-size: 12px;'>© 2024 YMC. All rights reserved.</p>
                    </div>
                </body>
                </html>
            ",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
            }
        }

        public async Task SendPasswordResetOtpAsync(string email, string otpCode, string username)
        {
            var smtpHost = _configuration["Email:SmtpHost"];
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"]);
            var smtpUsername = _configuration["Email:SmtpUsername"];
            var smtpPassword = _configuration["Email:SmtpPassword"];
            var fromEmail = _configuration["Email:FromEmail"];
            var fromName = _configuration["Email:FromName"];

            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = "YMC - Password Reset Code",
                    Body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #667eea;'>Password Reset Request</h2>
                        <p>Hi {username},</p>
                        <p>You requested to reset your password. Use the code below to reset it:</p>
                        <div style='background: #f5f5f5; padding: 20px; text-align: center; font-size: 32px; font-weight: bold; letter-spacing: 5px; color: #667eea; margin: 20px 0;'>
                            {otpCode}
                        </div>
                        <p style='color: #666;'>This code will expire in 10 minutes.</p>
                        <p style='color: #666;'>If you didn't request this, please ignore this email and your password will remain unchanged.</p>
                        <hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;'>
                        <p style='color: #999; font-size: 12px;'>© 2024 YMC. All rights reserved.</p>
                    </div>
                </body>
                </html>
            ",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
