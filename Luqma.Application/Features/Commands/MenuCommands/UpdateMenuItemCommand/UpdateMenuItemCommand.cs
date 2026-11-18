using Luqma.Application.Features.Dtos.Requests.MenuRequests;
using Luqma.Application.Features.Dtos.Responses.MenuResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.MenuCommands.UpdateMenuItemCommand
{
    public class UpdateMenuItemCommand : IRequest<Response<MenuItemResponse>>
    {
        public Guid Id { get; set; }
        public SaveMenuItemRequest Request { get; set; } = new();
    }
}
