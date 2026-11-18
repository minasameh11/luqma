using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.AccountCommands.UnarchiveUserCommand
{
    public class UnarchiveUserCommand : IRequest<Response<string>>
    {
        public Guid Id { get; set; }
    }
}
