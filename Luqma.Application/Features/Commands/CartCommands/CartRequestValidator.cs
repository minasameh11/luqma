using Luqma.Application.Features.Dtos.Requests.CartRequests;

namespace Luqma.Application.Features.Commands.CartCommands
{
    public static class CartRequestValidator
    {
        public const int MinQuantity = 0;
        public const int MaxQuantity = 99;
        public const int MaxAddonsPerLine = 20;

        // Adding always creates a line with at least one unit
        public static string? ValidateAdd(AddToCartRequest request)
        {
            if (request.Quantity is < 1 or > MaxQuantity)
                return $"الكمية يجب أن تكون بين 1 و {MaxQuantity}";

            return ValidateSelection(request.MenuItemId, request.MenuItemOptionId, request.AddonIds);
        }

        // Updating sets the line quantity; zero removes the line
        public static string? ValidateUpdate(UpdateCartRequest request)
        {
            if (request.Quantity is < MinQuantity or > MaxQuantity)
                return $"الكمية يجب أن تكون بين {MinQuantity} و {MaxQuantity}";

            return ValidateSelection(request.MenuItemId, request.MenuItemOptionId, request.AddonIds);
        }

        private static string? ValidateSelection(Guid menuItemId, Guid? menuItemOptionId, List<Guid>? addonIds)
        {
            if (menuItemId == Guid.Empty)
                return "معرف الصنف مطلوب";

            if (menuItemOptionId == Guid.Empty)
                return "معرف الخيار غير صحيح";

            if (addonIds is null)
                return null;

            if (addonIds.Count > MaxAddonsPerLine)
                return $"لا يمكن اختيار أكثر من {MaxAddonsPerLine} إضافة";

            if (addonIds.Any(id => id == Guid.Empty))
                return "معرف الإضافة غير صحيح";

            if (addonIds.Distinct().Count() != addonIds.Count)
                return "لا يمكن تكرار الإضافة نفسها";

            return null;
        }
    }
}
