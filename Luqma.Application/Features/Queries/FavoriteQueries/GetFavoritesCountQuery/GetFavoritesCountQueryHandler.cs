using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Queries.FavoriteQueries.GetFavoritesCountQuery
{
    public class GetFavoritesCountQueryHandler : IRequestHandler<GetFavoritesCountQuery, Response<int>>
    {
        private readonly IGenericRepository<Favorite> _favorites;
        private readonly ICurrentUserService _currentUser;

        public GetFavoritesCountQueryHandler(IGenericRepository<Favorite> favorites, ICurrentUserService currentUser)
        {
            _favorites = favorites;
            _currentUser = currentUser;
        }

        public async Task<Response<int>> Handle(GetFavoritesCountQuery query, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUser.UserId, out var userId))
                return Response<int>.Fail("غير مصرح");

            var count = await _favorites.GetAll()
                .AsNoTracking()
                .CountAsync(
                    f => f.UserId == userId && !f.IsArchived && f.MenuItem != null && !f.MenuItem.IsArchived,
                    cancellationToken);

            return Response<int>.Success(count, "تم جلب عدد أصناف المفضلة بنجاح");
        }
    }
}
