using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Queries.FavoriteQueries.IsFavoriteQuery
{
    public class IsFavoriteQueryHandler : IRequestHandler<IsFavoriteQuery, Response<bool>>
    {
        private readonly IGenericRepository<Favorite> _favorites;
        private readonly ICurrentUserService _currentUser;

        public IsFavoriteQueryHandler(IGenericRepository<Favorite> favorites, ICurrentUserService currentUser)
        {
            _favorites = favorites;
            _currentUser = currentUser;
        }

        public async Task<Response<bool>> Handle(IsFavoriteQuery query, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUser.UserId, out var userId))
                return Response<bool>.Fail("غير مصرح");

            if (query.MenuItemId == Guid.Empty)
                return Response<bool>.Fail("معرف الصنف مطلوب");

            var isFavorite = await _favorites.GetAll()
                .AsNoTracking()
                .AnyAsync(
                    f => f.UserId == userId && f.MenuItemId == query.MenuItemId && !f.IsArchived,
                    cancellationToken);

            return Response<bool>.Success(isFavorite);
        }
    }
}
