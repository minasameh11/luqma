using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.CartCommands.ClearCartCommand
{
    public class ClearCartCommand : IRequest<Response<int>>
    {
    }
}
