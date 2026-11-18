using Luqma.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Luqma.Application.Authorization
{
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly DefaultAuthorizationPolicyProvider _fallbackProvider;

        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _fallbackProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
            => _fallbackProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
            => _fallbackProvider.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (Enum.TryParse<PermissionEnum>(policyName, out var permission))
            {
                var policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new PermissionRequirement(permission))
                    .RequireAuthenticatedUser()
                    .Build();

                return Task.FromResult<AuthorizationPolicy?>(policy);
            }

            return _fallbackProvider.GetPolicyAsync(policyName);
        }
    }
}
