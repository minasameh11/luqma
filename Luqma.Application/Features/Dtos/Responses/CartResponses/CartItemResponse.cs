using Luqma.Application.Helpers;
using Luqma.Domain.Models;

namespace Luqma.Application.Features.Dtos.Responses.CartResponses
{
    public class CartItemResponse
    {
        public Guid Id { get; set; }
        public Guid MenuItemId { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }

        public Guid? MenuItemOptionId { get; set; }
        public string? OptionNameAr { get; set; }
        public string? OptionNameEn { get; set; }
        public decimal OptionPrice { get; set; }

        public List<CartItemAddonResponse> Addons { get; set; } = new();

        // Item price plus the selected option and addons
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
        public bool IsRemoved { get; set; }

        // False when the item is no longer sold (archived or deactivated) and should not be checked out
        public bool IsAvailable { get; set; } = true;

        public static CartItemResponse From(
            CartItem cartItem,
            MenuItem menuItem,
            MenuItemOption? option,
            IEnumerable<MenuItemAddon> addons,
            string baseUrl)
        {
            var selectedAddons = addons
                .OrderBy(a => a.NameAr, StringComparer.Ordinal)
                .Select(a => new CartItemAddonResponse
                {
                    Id = a.ID,
                    NameAr = a.NameAr,
                    NameEn = a.NameEn,
                    Price = a.Price
                })
                .ToList();

            var unitPrice = menuItem.Price + (option?.Price ?? 0m) + selectedAddons.Sum(a => a.Price);

            return new CartItemResponse
            {
                Id = cartItem.ID,
                MenuItemId = cartItem.MenuItemId,
                NameAr = menuItem.NameAr,
                NameEn = menuItem.NameEn,
                ImageUrl = ImageUrlResolver.ToAbsoluteUrl(baseUrl, menuItem.ImageUrl),
                MenuItemOptionId = option?.ID,
                OptionNameAr = option?.NameAr,
                OptionNameEn = option?.NameEn,
                OptionPrice = option?.Price ?? 0m,
                Addons = selectedAddons,
                UnitPrice = unitPrice,
                Quantity = cartItem.Quantity,
                LineTotal = unitPrice * cartItem.Quantity,
                IsRemoved = cartItem.IsArchived,
                IsAvailable = !menuItem.IsArchived && menuItem.IsActive
            };
        }
    }

    public class CartItemAddonResponse
    {
        public Guid Id { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
