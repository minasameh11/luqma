using Luqma.Application.Features.Dtos.Responses.MenuResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.MenuCommands.SetMenuItemActiveCommand
{
    public class SetMenuItemActiveCommand : IRequest<Response<MenuItemResponse>>
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
    }
}
