using Luqma.Application.Extentions.Pagination;
using Luqma.Application.Features.Dtos.Responses.FavoriteResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Queries.FavoriteQueries.GetMyFavoritesQuery
{
    public class GetMyFavoritesQueryHandler : IRequestHandler<GetMyFavoritesQuery, Response<PaginatedList<FavoriteResponse>>>
    {
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 50;

        private readonly IGenericRepository<Favorite> _favorites;
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentRequestService _currentRequest;

        public GetMyFavoritesQueryHandler(
            IGenericRepository<Favorite> favorites,
            ICurrentUserService currentUser,
            ICurrentRequestService currentRequest)
        {
            _favorites = favorites;
            _currentUser = currentUser;
            _currentRequest = currentRequest;
        }

        public async Task<Response<PaginatedList<FavoriteResponse>>> Handle(GetMyFavoritesQuery query, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUser.UserId, out var userId))
                return Response<PaginatedList<FavoriteResponse>>.Fail("غير مصرح");

            var pageNumber = query.PageNumber < 1 ? 1 : query.PageNumber;
            var pageSize = query.PageSize is < 1 or > MaxPageSize
                ? (query.PageSize is < 1 ? DefaultPageSize : MaxPageSize)
                : query.PageSize;

            var page = await _favorites.GetAll()
                .AsNoTracking()
                .Where(f => f.UserId == userId
                            && !f.IsArchived
                            && f.MenuItem != null
                            && !f.MenuItem.IsArchived)
                .Include(f => f.MenuItem)
                .OrderByDescending(f => f.CreatedDateTime)
                .ThenBy(f => f.ID)
                .PaginateAsync(pageNumber, pageSize, cancellationToken);

            var baseUrl = _currentRequest.BaseUrl;

            var items = page.Items
                .Select(f => FavoriteResponse.From(f, f.MenuItem!, baseUrl))
                .ToList();

            return Response<PaginatedList<FavoriteResponse>>.Success(
                new PaginatedList<FavoriteResponse>(items, page.TotalCount, pageNumber, pageSize),
                "تم جلب المفضلة بنجاح");
        }
    }
}
