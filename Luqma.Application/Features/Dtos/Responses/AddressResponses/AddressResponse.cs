using Luqma.Domain.Enums;
using Luqma.Domain.Models;
using System.Linq.Expressions;

namespace Luqma.Application.Features.Dtos.Responses.AddressResponses
{
    public class AddressResponse
    {
        public Guid Id { get; set; }
        public AddressLabel Label { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? District { get; set; }
        public string City { get; set; } = string.Empty;
        public string? Details { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsDefault { get; set; }

        public static readonly Expression<Func<Address, AddressResponse>> Projection = a => new AddressResponse
        {
            Id = a.ID,
            Label = a.Label,
            Title = a.Title,
            District = a.District,
            City = a.City,
            Details = a.Details,
            Latitude = a.Latitude,
            Longitude = a.Longitude,
            IsDefault = a.IsDefault
        };

        private static readonly Func<Address, AddressResponse> Compiled = Projection.Compile();

        public static AddressResponse From(Address address) => Compiled(address);
    }
}
