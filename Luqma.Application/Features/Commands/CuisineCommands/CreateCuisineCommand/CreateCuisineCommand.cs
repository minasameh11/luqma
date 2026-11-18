using Luqma.Application.Features.Dtos.Requests.CuisineRequests;
using Luqma.Application.Features.Dtos.Responses.CuisineResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.CuisineCommands.CreateCuisineCommand
{
    public class CreateCuisineCommand : IRequest<Response<CuisineResponse>>
    {
        public SaveCuisineRequest Request { get; set; } = new();
    }
}
