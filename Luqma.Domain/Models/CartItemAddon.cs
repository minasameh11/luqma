namespace Luqma.Domain.Models
{
    // An addon selected for a single cart line
    public class CartItemAddon : BaseEntity
    {
        public Guid CartItemId { get; set; }
        public CartItem? CartItem { get; set; }

        public Guid MenuItemAddonId { get; set; }
        public MenuItemAddon? MenuItemAddon { get; set; }
    }
}
