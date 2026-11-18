using Luqma.Application.Features.Dtos.Requests.AccountRequests;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.AccountCommands.RegisterCommand
{
    public class RegisterCommand : IRequest<Response<string>>
    {
        public RegisterRequest RegisterRequest { get; set; } = new();
    }
}
