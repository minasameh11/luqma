using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.AccountCommands.ArchiveUserCommand
{
    public class ArchiveUserCommand : IRequest<Response<string>>
    {
        public Guid Id { get; set; }
    }
}
