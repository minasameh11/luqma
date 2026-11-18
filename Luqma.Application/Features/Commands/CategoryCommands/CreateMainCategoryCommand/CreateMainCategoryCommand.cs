using Luqma.Application.Features.Dtos.Requests.CategoryRequests;
using Luqma.Application.Features.Dtos.Responses.CategoryResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.CategoryCommands.CreateMainCategoryCommand
{
    public class CreateMainCategoryCommand : IRequest<Response<MainCategoryResponse>>
    {
        public SaveMainCategoryRequest Request { get; set; } = new();
    }
}
