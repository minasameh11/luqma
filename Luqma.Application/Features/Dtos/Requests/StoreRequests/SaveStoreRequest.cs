using Luqma.Domain.Enums;

namespace Luqma.Application.Features.Dtos.Requests.StoreRequests
{
    public class SaveStoreRequest
    {
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;

        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }

        public Guid MainCategoryId { get; set; }

        // Local Egypt time, e.g. "09:00:00"
        public TimeOnly OpenTime { get; set; }
        public TimeOnly CloseTime { get; set; }

        // Rough price band: Economy (1), Moderate (2) or Premium (3)
        public PriceLevel PriceLevel { get; set; } = PriceLevel.Moderate;

        // Estimated delivery time in minutes and the fixed delivery fee
        public int DeliveryTimeMinutes { get; set; }
        public decimal DeliveryFee { get; set; }

        // Map location of the store; both values are required together
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public bool IsActive { get; set; } = true;

        // Kitchen types the store serves, e.g. grill and seafood
        public List<Guid> CuisineIds { get; set; } = new();

        // Gallery photos in display order; the icon stays in IconUrl
        public List<string> ImageUrls { get; set; } = new();
    }
}
