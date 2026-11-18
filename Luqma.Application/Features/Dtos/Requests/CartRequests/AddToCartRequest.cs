namespace Luqma.Application.Features.Dtos.Requests.CartRequests
{
    public class AddToCartRequest
    {
        public Guid MenuItemId { get; set; }

        // Quantity of the new cart line; changing an existing line is done through UpdateToCart
        public int Quantity { get; set; } = 1;

        // Optional choice of the item, e.g. "نفر واحد" / "نفرين"
        public Guid? MenuItemOptionId { get; set; }

        // Optional paid additions selected for this line
        public List<Guid>? AddonIds { get; set; }
    }
}
