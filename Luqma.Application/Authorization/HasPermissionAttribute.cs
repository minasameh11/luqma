using Luqma.Domain.Models;
using Microsoft.AspNetCore.Authorization;

namespace Luqma.Application.Authorization
{
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(PermissionEnum permission)
            : base(policy: permission.ToString())
        {
        }
    }
}
