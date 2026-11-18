using Luqma.Domain.Helpers;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Commands.CartCommands
{
    public sealed class CartLineSelection
    {
        public string? Error { get; init; }
        public MenuItem? MenuItem { get; init; }
        public MenuItemOption? Option { get; init; }
        public IReadOnlyList<MenuItemAddon> Addons { get; init; } = Array.Empty<MenuItemAddon>();
        public string Signature { get; init; } = string.Empty;

        public bool IsResolved => Error is null;
    }

    // Resolves and validates the item, option and addons that identify a cart line
    public static class CartLineResolver
    {
        public static async Task<CartLineSelection> ResolveAsync(
            Guid menuItemId,
            Guid? menuItemOptionId,
            IReadOnlyCollection<Guid>? addonIds,
            IGenericRepository<MenuItem> menuItems,
            IGenericRepository<MenuItemOption> options,
            IGenericRepository<MenuItemAddon> addons,
            CancellationToken cancellationToken)
        {
            var menuItem = await menuItems.GetAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.ID == menuItemId, cancellationToken);

            if (menuItem is null || menuItem.IsArchived || !menuItem.IsActive)
                return new CartLineSelection { Error = "الصنف غير متاح" };

            MenuItemOption? option = null;
            if (menuItemOptionId.HasValue)
            {
                option = await options.GetAll()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        o => o.ID == menuItemOptionId.Value && o.MenuItemId == menuItem.ID,
                        cancellationToken);

                if (option is null || option.IsArchived)
                    return new CartLineSelection { Error = "الخيار غير متاح" };
            }

            var requestedAddonIds = addonIds?.Distinct().ToList() ?? new List<Guid>();
            var selectedAddons = new List<MenuItemAddon>();

            if (requestedAddonIds.Count > 0)
            {
                selectedAddons = await addons.GetAll()
                    .AsNoTracking()
                    .Where(a => requestedAddonIds.Contains(a.ID) && a.MenuItemId == menuItem.ID)
                    .ToListAsync(cancellationToken);

                selectedAddons = selectedAddons.Where(a => !a.IsArchived).ToList();

                if (selectedAddons.Count != requestedAddonIds.Count)
                    return new CartLineSelection { Error = "إحدى الإضافات غير متاحة" };
            }

            return new CartLineSelection
            {
                MenuItem = menuItem,
                Option = option,
                Addons = selectedAddons,
                Signature = CartConfigurationSignature.Build(option?.ID, selectedAddons.Select(a => a.ID))
            };
        }
    }
}
