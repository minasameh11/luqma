using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.AddressCommands.DeleteAddressCommand
{
    public class DeleteAddressCommand : IRequest<Response<string>>
    {
        public Guid Id { get; set; }
    }
}
