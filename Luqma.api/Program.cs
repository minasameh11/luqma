using Luqma.Application;
using Luqma.Domain.Interfaces;
using Microsoft.Extensions.FileProviders;
using Luqma.Infrastructure;
using Microsoft.OpenApi.Models;

namespace Luqma.api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", false);

            var builder = WebApplication.CreateBuilder(args);

            // Uploaded files live under wwwroot; make sure it exists before static files are set up
            var webRootPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot");
            Directory.CreateDirectory(webRootPath);

            // Infrastructure
            builder.Services.AddInfrastructureServices(
                builder.Configuration);

            // Application
            builder.Services.AddApplicationServices();

            // Controllers
            builder.Services.AddControllers();

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(setupAction =>
            {
                setupAction.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token.\r\n\r\nExample: \"Bearer 12345abcdef\""
                });

                setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var accountManager = scope.ServiceProvider.GetRequiredService<IAccountManager>();
                await accountManager.SeedDefaultAdminAsync();
            }

            //if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Serves uploaded files saved under wwwroot (e.g. category icons)
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(webRootPath)
            });

            // CORS
            app.UseCors("AllowAll");

            // Authentication
            app.UseAuthentication();

            // Authorization
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
