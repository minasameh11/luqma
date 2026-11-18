using Luqma.Domain.Enums;
using Luqma.Domain.Helpers;

namespace Luqma.Domain.Models
{
    public class Store : BaseEntity
    {
        public string NameAr { get; set; } = null!;
        public string NameEn { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;

        // Icon image shown for the store (relative path or absolute URL)
        public string IconUrl { get; set; } = null!;

        // Optional long description shown on the store page
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }

        public Guid MainCategoryId { get; set; }
        public MainCategory? MainCategory { get; set; }

        // Open/close times are local Egypt times; equal values mean open 24 hours
        public TimeOnly OpenTime { get; set; }
        public TimeOnly CloseTime { get; set; }

        // Rough price band shown to the customer as $, $$ or $$$
        public PriceLevel PriceLevel { get; set; } = PriceLevel.Moderate;

        // Average of the customer ratings (0..5) and how many ratings produced it.
        // Kept as an aggregate on the store so lists do not need to scan reviews.
        public double AverageRating { get; set; }
        public int RatingsCount { get; set; }

        // Estimated delivery time in minutes and the fixed delivery fee
        public int DeliveryTimeMinutes { get; set; }
        public decimal DeliveryFee { get; set; }

        // Map location of the store; (0,0) means the location is not set yet
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public bool IsActive { get; set; } = true;

        // Owner of the store; null means the store is managed by the platform only
        public Guid? OwnerId { get; set; }

        // Gallery photos shown on the store page, in display order
        public ICollection<StoreImage> Images { get; set; } = new List<StoreImage>();

        public bool IsOpenAt(TimeOnly time) => IsActive && WorkingHours.IsOpen(OpenTime, CloseTime, time);
    }
}
