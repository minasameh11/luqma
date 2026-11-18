using Luqma.Application.Features.Dtos.Requests.AccountRequests;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.AccountCommands.ConfirmEmailCommand
{
    public class ConfirmEmailCommand : IRequest<Response<string>>
    {
        public ConfirmEmailRequest ConfirmEmailRequest { get; set; } = new();
    }
}
