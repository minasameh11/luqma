using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Queries.FavoriteQueries.IsFavoriteQuery
{
    public class IsFavoriteQuery : IRequest<Response<bool>>
    {
        public Guid MenuItemId { get; set; }
    }
}
