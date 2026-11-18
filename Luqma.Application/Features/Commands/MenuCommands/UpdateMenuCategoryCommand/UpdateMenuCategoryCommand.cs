using Luqma.Application.Features.Dtos.Requests.MenuRequests;
using Luqma.Application.Features.Dtos.Responses.MenuResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.MenuCommands.UpdateMenuCategoryCommand
{
    public class UpdateMenuCategoryCommand : IRequest<Response<MenuCategoryResponse>>
    {
        public Guid Id { get; set; }
        public SaveMenuCategoryRequest Request { get; set; } = new();
    }
}
