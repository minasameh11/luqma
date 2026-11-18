using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using Luqma.Infrastructure.Database;
using Luqma.Infrastructure.Repositories;
using Luqma.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Luqma.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<IAccountManager, AccountManager>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ICurrentRequestService, CurrentRequestService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IFirebaseService, FirebaseService>();
            services.AddHttpClient(); // enables IHttpClientFactory (used by EmailService Brevo provider)
            services.AddHttpClient<FacebookAuthService>();
            services.AddScoped<FacebookAuthService>();
            services.AddHttpClient<AppleAuthService>();
            services.AddScoped<AppleAuthService>();
            services.AddHttpClient<GoogleAuthService>();
            services.AddScoped<GoogleAuthService>();
            //services.AddScoped<IOrderStatusHub, OrderStatusHubService>();
            services.AddHostedService<UnverifiedUserCleanupService>();

            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<LuqmaContext>()
                .AddDefaultTokenProviders();

            services.AddHttpContextAccessor();

            var securityKeyString = configuration["Jwt:SecurityKey"];
            var securityKeyByte = Encoding.ASCII.GetBytes(securityKeyString);
            SecurityKey securityKey = new SymmetricSecurityKey(securityKeyByte);

            // Database
            services.AddDbContext<LuqmaContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection")
                ));

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer( builder =>
            {
                builder.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JWT:IssuerIP"],
                    ValidAudience = configuration["JWT:AudienceIP"],
                    IssuerSigningKey = securityKey
                };
            });

            return services;
        }
    }
}
