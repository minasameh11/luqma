namespace Luqma.Application.Features.Dtos.Requests.CartRequests
{
    public class UpdateCartRequest
    {
        // Identifies the existing cart line
        public Guid MenuItemId { get; set; }
        public Guid? MenuItemOptionId { get; set; }
        public List<Guid>? AddonIds { get; set; }

        // New quantity of the line; zero removes it from the cart
        public int Quantity { get; set; }
    }
}
