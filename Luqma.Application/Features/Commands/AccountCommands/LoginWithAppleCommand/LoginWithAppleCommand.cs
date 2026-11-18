using Luqma.Application.Features.Dtos.Requests.AccountRequests;
using Luqma.Application.Features.Dtos.Responses.AccountResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.AccountCommands.LoginWithAppleCommand
{
    public class LoginWithAppleCommand : IRequest<Response<AuthResponse>>
    {
        public AppleLoginRequest AppleLoginRequest { get; set; } = new();
    }
}
