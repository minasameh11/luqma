using Luqma.Application.Features.Dtos.Responses.CuisineResponses;
using Luqma.Application.Helpers;
using Luqma.Domain.Enums;
using Luqma.Domain.Helpers;
using Luqma.Domain.Models;
using System.Linq.Expressions;

namespace Luqma.Application.Features.Dtos.Responses.StoreResponses
{
    public class StoreResponse
    {
        public Guid Id { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;

        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }

        public Guid MainCategoryId { get; set; }
        public string MainCategoryNameAr { get; set; } = string.Empty;
        public string MainCategoryNameEn { get; set; } = string.Empty;

        public TimeOnly OpenTime { get; set; }
        public TimeOnly CloseTime { get; set; }

        public PriceLevel PriceLevel { get; set; }

        // Average customer rating (0..5) and how many ratings produced it
        public double AverageRating { get; set; }
        public int RatingsCount { get; set; }

        public int DeliveryTimeMinutes { get; set; }
        public decimal DeliveryFee { get; set; }

        // Map location of the store; (0,0) means the location is not set
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // Straight-line distance from the point sent by the client, null when either side is unknown
        public double? DistanceKm { get; set; }

        public bool IsActive { get; set; }

        // True only when the store is active and the current Egypt time is inside its working hours
        public bool IsOpenNow { get; set; }

        // Kitchen types the store serves
        public List<CuisineResponse> Cuisines { get; set; } = new();

        // Gallery photos in display order
        public List<string> Images { get; set; } = new();

        public static readonly Expression<Func<Store, StoreResponse>> Projection = s => new StoreResponse
        {
            Id = s.ID,
            NameAr = s.NameAr,
            NameEn = s.NameEn,
            PhoneNumber = s.PhoneNumber,
            Address = s.Address,
            IconUrl = s.IconUrl,
            DescriptionAr = s.DescriptionAr,
            DescriptionEn = s.DescriptionEn,
            MainCategoryId = s.MainCategoryId,
            MainCategoryNameAr = s.MainCategory != null ? s.MainCategory.NameAr : string.Empty,
            MainCategoryNameEn = s.MainCategory != null ? s.MainCategory.NameEn : string.Empty,
            OpenTime = s.OpenTime,
            CloseTime = s.CloseTime,
            PriceLevel = s.PriceLevel,
            AverageRating = s.AverageRating,
            RatingsCount = s.RatingsCount,
            DeliveryTimeMinutes = s.DeliveryTimeMinutes,
            DeliveryFee = s.DeliveryFee,
            Latitude = s.Latitude,
            Longitude = s.Longitude,
            IsActive = s.IsActive
        };

        private static readonly Func<Store, StoreResponse> Compiled = Projection.Compile();

        public static StoreResponse From(Store store) => Compiled(store);

        // Relative icon paths (e.g. /uploads/...) are turned into full URLs the client can open directly
        public StoreResponse WithAbsoluteIconUrl(string baseUrl)
        {
            IconUrl = ImageUrlResolver.ToAbsoluteUrl(baseUrl, IconUrl);
            return this;
        }

        // Distance from the client's point; skipped when the client sent no coordinates
        public StoreResponse WithDistanceFrom(double? latitude, double? longitude)
        {
            if (latitude.HasValue && longitude.HasValue)
                DistanceKm = GeoDistance.Kilometers(latitude.Value, longitude.Value, Latitude, Longitude);

            return this;
        }
    }
}
