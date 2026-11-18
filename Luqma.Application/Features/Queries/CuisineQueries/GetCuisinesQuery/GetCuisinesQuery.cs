using Luqma.Application.Features.Dtos.Responses.CuisineResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Queries.CuisineQueries.GetCuisinesQuery
{
    public class GetCuisinesQuery : IRequest<Response<List<CuisineResponse>>>
    {
        // When true only active cuisines are returned (customer filter chips)
        public bool OnlyActive { get; set; }
    }
}
