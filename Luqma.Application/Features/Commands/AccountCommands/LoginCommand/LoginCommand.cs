using Luqma.Application.Features.Dtos.Requests.AccountRequests;
using Luqma.Application.Features.Dtos.Responses.AccountResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.AccountCommands.LoginCommand
{
    public class LoginCommand : IRequest<Response<AuthResponse>>
    {
        public LoginRequest LoginRequest { get; set; } = new();
    }
}
