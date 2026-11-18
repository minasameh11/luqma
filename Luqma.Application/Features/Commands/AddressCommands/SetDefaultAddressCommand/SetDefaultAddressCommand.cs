using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.AddressCommands.SetDefaultAddressCommand
{
    public class SetDefaultAddressCommand : IRequest<Response<string>>
    {
        public Guid Id { get; set; }
    }
}
