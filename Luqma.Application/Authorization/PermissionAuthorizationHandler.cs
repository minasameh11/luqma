using Luqma.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Luqma.Infrastructure.Database;

namespace Luqma.Application.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IServiceProvider _serviceProvider;

        public PermissionAuthorizationHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                context.Fail();
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<LuqmaContext>();

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                context.Fail();
                return;
            }

            // SuperAdmin يعمل أي حاجة
            var roles = await userManager.GetRolesAsync(user);
            if (roles.Contains("SuperAdmin"))
            {
                context.Succeed(requirement);
                return;
            }

            // نجيب الـ RoleIds الخاصة باليوزر
            var roleIds = await dbContext.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            // نشوف لو أي Role من دول عندها الـ Permission المطلوبة
            var hasPermission = await dbContext.RolePermissions
                .Include(rp => rp.PermissionObj)
                .Where(rp => roleIds.Contains(rp.RoleId) && !rp.IsArchived)
                .AnyAsync(rp => rp.PermissionObj != null &&
                                rp.PermissionObj.PermissionType == requirement.Permission &&
                                !rp.PermissionObj.IsArchived);

            if (hasPermission)
                context.Succeed(requirement);
            else
                context.Fail();
        }
    }
}
