namespace Luqma.Domain.Models
{
    // One line in a user's cart; the same menu item with the same option/addons is stored once per user
    public class CartItem : BaseEntity
    {
        public Guid UserId { get; set; }

        public Guid MenuItemId { get; set; }
        public MenuItem? MenuItem { get; set; }

        // Selected option of the item (e.g. "نفرين"), null when the item has no option selected
        public Guid? MenuItemOptionId { get; set; }
        public MenuItemOption? MenuItemOption { get; set; }

        public int Quantity { get; set; }

        // Deterministic key of the selected option and addons; one line per configuration
        public string ConfigurationSignature { get; set; } = null!;

        // Paid additions selected for this line
        public ICollection<CartItemAddon> Addons { get; set; } = new List<CartItemAddon>();
    }
}
