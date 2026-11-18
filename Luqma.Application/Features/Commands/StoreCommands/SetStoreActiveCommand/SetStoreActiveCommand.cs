using Luqma.Application.Features.Dtos.Responses.StoreResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.StoreCommands.SetStoreActiveCommand
{
    public class SetStoreActiveCommand : IRequest<Response<StoreResponse>>
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
    }
}
