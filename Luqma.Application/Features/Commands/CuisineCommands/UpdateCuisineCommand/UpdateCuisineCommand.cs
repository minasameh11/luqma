using Luqma.Application.Features.Dtos.Requests.CuisineRequests;
using Luqma.Application.Features.Dtos.Responses.CuisineResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.CuisineCommands.UpdateCuisineCommand
{
    public class UpdateCuisineCommand : IRequest<Response<CuisineResponse>>
    {
        public Guid Id { get; set; }
        public SaveCuisineRequest Request { get; set; } = new();
    }
}
