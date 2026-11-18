using Luqma.Domain.Enums;

namespace Luqma.Application.Features.Dtos.Requests.AddressRequests
{
    public class SaveAddressRequest
    {
        public AddressLabel Label { get; set; } = AddressLabel.Home;
        public string Title { get; set; } = string.Empty;
        public string? District { get; set; }
        public string City { get; set; } = string.Empty;
        public string? Details { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsDefault { get; set; }
    }
}
