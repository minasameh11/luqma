using Luqma.Application.Features.Dtos.Responses.AddressResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Queries.AddressQueries.GetMyAddressesQuery
{
    public class GetMyAddressesQuery : IRequest<Response<List<AddressResponse>>>
    {
    }
}
