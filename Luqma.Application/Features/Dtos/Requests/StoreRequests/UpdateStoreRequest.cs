using Luqma.Domain.Enums;

namespace Luqma.Application.Features.Dtos.Requests.StoreRequests
{
    // Every field is optional: send only what you want to change.
    // Omitted (null) fields keep the stored value.
    public class UpdateStoreRequest
    {
        public string? NameAr { get; set; }
        public string? NameEn { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? IconUrl { get; set; }

        // Sending an empty string clears the stored description
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }

        public Guid? MainCategoryId { get; set; }

        public TimeOnly? OpenTime { get; set; }
        public TimeOnly? CloseTime { get; set; }

        public PriceLevel? PriceLevel { get; set; }

        public int? DeliveryTimeMinutes { get; set; }
        public decimal? DeliveryFee { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public bool? IsActive { get; set; }

        // Admin only: links the store to the user who can manage its menu
        public Guid? OwnerId { get; set; }

        // null keeps the stored cuisines, an empty list clears them
        public List<Guid>? CuisineIds { get; set; }

        // null keeps the stored gallery, an empty list clears it
        public List<string>? ImageUrls { get; set; }
    }
}
