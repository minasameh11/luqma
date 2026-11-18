using Luqma.Application.Features.Dtos.Responses.AccountResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Queries.AccountQueries.GetProfileQuery
{
    public class GetProfileQuery : IRequest<Response<UserResponse>>
    {
    }
}
