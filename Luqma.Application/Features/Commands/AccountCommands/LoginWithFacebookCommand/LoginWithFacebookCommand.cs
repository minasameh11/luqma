using Luqma.Application.Features.Dtos.Requests.AccountRequests;
using Luqma.Application.Features.Dtos.Responses.AccountResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.AccountCommands.LoginWithFacebookCommand
{
    public class LoginWithFacebookCommand : IRequest<Response<AuthResponse>>
    {
        public FacebookLoginRequest FacebookLoginRequest { get; set; } = new();
    }
}
