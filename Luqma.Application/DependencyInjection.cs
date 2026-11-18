using Luqma.Application.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Luqma.Application.Mapping;

namespace Luqma.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(options =>
            {
                options.RegisterServicesFromAssemblies(
                    //typeof(CategoryAddCommands).Assembly,
                    Assembly.GetExecutingAssembly()
                    );
            });
            services.AddAutoMapper(options =>
            {
                options.AddProfile(new MappingProfile());
            });

            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            return services;
        }
    }
}
