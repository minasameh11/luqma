using Luqma.Domain.Interfaces;

namespace Luqma.Application.Features.Commands.MenuCommands
{
    public static class MenuAccessGuard
    {
        // Admins manage every store; an owner manages only the store they own
        public static string? ValidateStoreAccess(Guid? ownerId, ICurrentUserService currentUser)
        {
            if (currentUser.IsInRole("Admin") || currentUser.IsInRole("SuperAdmin"))
                return null;

            if (!Guid.TryParse(currentUser.UserId, out var userId))
                return "غير مصرح";

            if (ownerId != userId)
                return "غير مصرح: هذا المحل ليس ملكك";

            return null;
        }
    }
}
