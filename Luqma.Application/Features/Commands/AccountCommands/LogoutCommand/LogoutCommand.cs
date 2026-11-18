using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.AccountCommands.LogoutCommand
{
    public class LogoutCommand : IRequest<Response<string>>
    {
    }
}
