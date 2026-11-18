using Luqma.Application.Features.Dtos.Requests.MenuRequests;
using Luqma.Application.Features.Dtos.Responses.MenuResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.MenuCommands.CreateMenuCategoryCommand
{
    public class CreateMenuCategoryCommand : IRequest<Response<MenuCategoryResponse>>
    {
        public SaveMenuCategoryRequest Request { get; set; } = new();
    }
}
