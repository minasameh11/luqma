using Luqma.Application.Features.Dtos.Requests.AccountRequests;
using Luqma.Application.Features.Dtos.Responses.AccountResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.AccountCommands.LoginWithGoogleCommand
{
    public class LoginWithGoogleCommand : IRequest<Response<AuthResponse>>
    {
        public GoogleLoginRequest GoogleLoginRequest { get; set; } = new();
    }
}
