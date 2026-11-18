using Luqma.Application.Features.Dtos.Requests.AccountRequests;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.AccountCommands.ResetPasswordCommand
{
    public class ResetPasswordCommand : IRequest<Response<string>>
    {
        public ResetPasswordRequest ResetPasswordRequest { get; set; } = new();
    }
}
