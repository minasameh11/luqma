namespace Luqma.Application.Features.Dtos.Responses.CartResponses
{
    public class CartResponse
    {
        public List<CartItemResponse> Items { get; set; } = new();
        public int TotalQuantity { get; set; }
        public decimal SubTotal { get; set; }
    }

    public class CartCountResponse
    {
        public int LinesCount { get; set; }
        public int TotalQuantity { get; set; }
    }
}
