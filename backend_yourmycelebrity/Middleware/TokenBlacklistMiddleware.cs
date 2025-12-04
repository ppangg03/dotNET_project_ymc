using backend_yourmycelebrity.Data;
using backend_yourmycelebrity.Models.Enum;
using Microsoft.EntityFrameworkCore;

namespace backend_yourmycelebrity.Middleware
{
    public class TokenBlacklistMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly AppDbContext _context;

        public TokenBlacklistMiddleware(RequestDelegate next)
        {
            _next = next;
            //_context = context;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();

            var dbContext = context.RequestServices.GetRequiredService<AppDbContext>();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Replace("Bearer ", "");

                // เช็คว่า token ถูก blacklist หรือไม่
                var isBlacklisted = await dbContext.SecurityTokens
                    .AnyAsync(t => t.Token == token
                              && t.TokenType == TokenType.Blacklist
                              && t.ExpiresAt > DateTime.UtcNow);

                if (isBlacklisted)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new { message = "Token has been revoked" });
                    return;
                }
            }

            await _next(context);
        }
    }
}
