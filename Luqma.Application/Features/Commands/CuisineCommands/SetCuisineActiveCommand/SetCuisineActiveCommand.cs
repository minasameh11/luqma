using Luqma.Application.Features.Dtos.Responses.CuisineResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.CuisineCommands.SetCuisineActiveCommand
{
    public class SetCuisineActiveCommand : IRequest<Response<CuisineResponse>>
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
    }
}
