using Luqma.Domain.Models;
using Microsoft.AspNetCore.Authorization;

namespace Luqma.Application.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionEnum Permission { get; }

        public PermissionRequirement(PermissionEnum permission)
        {
            Permission = permission;
        }
    }
}
