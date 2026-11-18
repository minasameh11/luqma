using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Queries.FavoriteQueries.GetFavoritesCountQuery
{
    public class GetFavoritesCountQuery : IRequest<Response<int>>
    {
    }
}
