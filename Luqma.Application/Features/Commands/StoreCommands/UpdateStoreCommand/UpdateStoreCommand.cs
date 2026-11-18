using Luqma.Application.Features.Dtos.Requests.StoreRequests;
using Luqma.Application.Features.Dtos.Responses.StoreResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.StoreCommands.UpdateStoreCommand
{
    public class UpdateStoreCommand : IRequest<Response<StoreResponse>>
    {
        public Guid Id { get; set; }
        public UpdateStoreRequest Request { get; set; } = new();
    }
}
