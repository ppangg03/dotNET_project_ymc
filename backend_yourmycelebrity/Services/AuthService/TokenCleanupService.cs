
using backend_yourmycelebrity.Data;
using Microsoft.EntityFrameworkCore;

namespace backend_yourmycelebrity.Services.AuthService
{
    public class TokenCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public TokenCleanupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var expiredTokens = await dbContext.SecurityTokens
                        .Where(t => t.ExpiresAt < DateTime.UtcNow)
                        .ToListAsync();

                    dbContext.SecurityTokens.RemoveRange(expiredTokens);
                    await dbContext.SaveChangesAsync();
                }
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
        //private async Task CleanupExpiredTokens()
        //{
        //    using (var scope = _serviceProvider.CreateScope())
        //    {
        //        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        //        // ลบ token ที่หมดอายุ
        //        var expiredTokens = await context.SecurityTokens
        //            .Where(t => t.ExpiresAt < DateTime.UtcNow)
        //            .ToListAsync();

        //        if (expiredTokens.Any())
        //        {
        //            context.SecurityTokens.RemoveRange(expiredTokens);
        //            await context.SaveChangesAsync();

        //            Console.WriteLine($"Cleaned up {expiredTokens.Count} expired tokens");
        //        }
        //    }
        //}
    }
}
