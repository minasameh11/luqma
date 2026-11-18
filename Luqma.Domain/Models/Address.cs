using Luqma.Domain.Enums;

namespace Luqma.Domain.Models
{
    public class Address : BaseEntity
    {
        public Guid UserId { get; set; }
        public AddressLabel Label { get; set; } = AddressLabel.Home;

        // Street / main line shown on the map card, e.g. "112 شارع المعز"
        public string Title { get; set; } = null!;
        public string? District { get; set; }
        public string City { get; set; } = null!;

        // Landmark / extra directions (building, floor, apartment, "near ...")
        public string? Details { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public bool IsDefault { get; set; }
    }
}
