using Luqma.Application.Extentions.Pagination;
using Luqma.Application.Features.Dtos.Responses.FavoriteResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Queries.FavoriteQueries.GetMyFavoritesQuery
{
    public class GetMyFavoritesQuery : IRequest<Response<PaginatedList<FavoriteResponse>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
