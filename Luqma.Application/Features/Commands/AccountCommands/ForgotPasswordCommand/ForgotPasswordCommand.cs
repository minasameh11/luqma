using Luqma.Application.Features.Dtos.Requests.AccountRequests;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.AccountCommands.ForgotPasswordCommand
{
    public class ForgotPasswordCommand : IRequest<Response<string>>
    {
        public ForgotPasswordRequest ForgotPasswordRequest { get; set; } = new();
    }
}
