using Luqma.Application.Features.Dtos.Responses.CartResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Queries.CartQueries.GetMyCartQuery
{
    public class GetMyCartQuery : IRequest<Response<CartResponse>>
    {
    }
}
