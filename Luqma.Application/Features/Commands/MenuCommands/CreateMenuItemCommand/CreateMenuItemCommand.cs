using Luqma.Application.Features.Dtos.Requests.MenuRequests;
using Luqma.Application.Features.Dtos.Responses.MenuResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.MenuCommands.CreateMenuItemCommand
{
    public class CreateMenuItemCommand : IRequest<Response<MenuItemResponse>>
    {
        public SaveMenuItemRequest Request { get; set; } = new();
    }
}
