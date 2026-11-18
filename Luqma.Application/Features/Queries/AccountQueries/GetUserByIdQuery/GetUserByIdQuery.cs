using Luqma.Application.Features.Dtos.Responses.AccountResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Queries.AccountQueries.GetUserByIdQuery
{
    public class GetUserByIdQuery : IRequest<Response<UserResponse>>
    {
        public Guid Id { get; set; }
    }
}
