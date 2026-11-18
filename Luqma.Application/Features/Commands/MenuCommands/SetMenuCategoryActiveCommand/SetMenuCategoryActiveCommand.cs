using Luqma.Application.Features.Dtos.Responses.MenuResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.MenuCommands.SetMenuCategoryActiveCommand
{
    public class SetMenuCategoryActiveCommand : IRequest<Response<MenuCategoryResponse>>
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
    }
}
