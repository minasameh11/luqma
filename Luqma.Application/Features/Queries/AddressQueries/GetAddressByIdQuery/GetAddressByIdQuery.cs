using Luqma.Application.Features.Dtos.Responses.AddressResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Queries.AddressQueries.GetAddressByIdQuery
{
    public class GetAddressByIdQuery : IRequest<Response<AddressResponse>>
    {
        public Guid Id { get; set; }
    }
}
