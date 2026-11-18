using Luqma.Application.Features.Dtos.Requests.StoreRequests;
using Luqma.Application.Features.Dtos.Responses.StoreResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.StoreCommands.CreateStoreCommand
{
    public class CreateStoreCommand : IRequest<Response<StoreResponse>>
    {
        public SaveStoreRequest Request { get; set; } = new();
    }
}
