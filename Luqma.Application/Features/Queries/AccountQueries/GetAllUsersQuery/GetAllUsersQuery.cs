using Luqma.Application.Features.Dtos.Responses.AccountResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Queries.AccountQueries.GetAllUsersQuery
{
    public class GetAllUsersQuery : IRequest<Response<List<UserResponse>>>
    {
    }
}
