using Luqma.Application.Features.Dtos.Responses.CartResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Queries.CartQueries.GetCartCountQuery
{
    public class GetCartCountQuery : IRequest<Response<CartCountResponse>>
    {
    }
}
