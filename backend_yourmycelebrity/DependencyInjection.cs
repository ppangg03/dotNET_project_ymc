using backend_yourmycelebrity.Data;
using backend_yourmycelebrity.Repositories.Implementations;
using backend_yourmycelebrity.Repositories.Interface;
using backend_yourmycelebrity.Services.Interface;
using backend_yourmycelebrity.Services.UserService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace backend_yourmycelebrity
{
    public static class DependencyInjectionExtensions
    {
        //private readonly IServiceCollection 
        public static IServiceCollection AddPersistence(this IServiceCollection services) 
        {
            //Repositories
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IArtistProfileRepository), typeof(ArtistProfileRepository));
            services.AddScoped(typeof(IAuthRepository),typeof( AuthRepository));


            //Services
            services.AddScoped(typeof(IJwtService), typeof( JwtService ));
            services.AddScoped(typeof(IEmailService), typeof(EmailService));
            return services;
        }
        public static IServiceCollection AddJWTConfig(this IServiceCollection services)
        {
            services.AddAuthentication(opt => {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
             })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = "https://localhost:7008",
                    ValidAudience = "https://localhost:7008",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("A1B2C3D4E5F6G7H8I9J0K1L2M3N4O5P6"))
                };
            });
            return services;
        }
        public static IServiceCollection AddConfigurationService(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("EnableCORS", builder =>
                {
                    builder.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
            return services;
        }

        
    }
}
