using Luqma.Application.Extentions.Pagination;
using Luqma.Application.Features.Dtos.Responses.MenuResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Queries.MenuQueries.GetMenuItemsQuery
{
    public class GetMenuItemsQueryHandler : IRequestHandler<GetMenuItemsQuery, Response<PaginatedList<MenuItemResponse>>>
    {
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 50;

        private readonly IGenericRepository<MenuItem> _items;
        private readonly ICurrentRequestService _currentRequest;

        public GetMenuItemsQueryHandler(IGenericRepository<MenuItem> items, ICurrentRequestService currentRequest)
        {
            _items = items;
            _currentRequest = currentRequest;
        }

        public async Task<Response<PaginatedList<MenuItemResponse>>> Handle(GetMenuItemsQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize is < 1 or > MaxPageSize
                ? (request.PageSize is < 1 ? DefaultPageSize : MaxPageSize)
                : request.PageSize;

            var query = _items.GetAll()
                .AsNoTracking()
                .Where(i => !i.IsArchived);

            if (request.MenuCategoryId.HasValue && request.MenuCategoryId.Value != Guid.Empty)
                query = query.Where(i => i.MenuCategoryId == request.MenuCategoryId.Value);

            if (request.StoreId.HasValue && request.StoreId.Value != Guid.Empty)
                query = query.Where(i => i.MenuCategory != null && i.MenuCategory.StoreId == request.StoreId.Value);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(i =>
                    i.NameAr.ToLower().Contains(search) ||
                    i.NameEn.ToLower().Contains(search) ||
                    (i.DescriptionAr != null && i.DescriptionAr.ToLower().Contains(search)) ||
                    (i.DescriptionEn != null && i.DescriptionEn.ToLower().Contains(search)));
            }

            if (request.MinPrice.HasValue)
                query = query.Where(i => i.Price >= request.MinPrice.Value);

            if (request.MaxPrice.HasValue)
                query = query.Where(i => i.Price <= request.MaxPrice.Value);

            if (request.IsActive.HasValue)
                query = query.Where(i => i.IsActive == request.IsActive.Value);

            if (request.OnSale.HasValue)
                query = request.OnSale.Value
                    ? query.Where(i => i.CompareAtPrice != null && i.CompareAtPrice > i.Price)
                    : query.Where(i => i.CompareAtPrice == null || i.CompareAtPrice <= i.Price);

            var ordered = request.SortBy?.Trim().ToLowerInvariant() switch
            {
                "price_asc" => query.OrderBy(i => i.Price).ThenBy(i => i.NameAr),
                "price_desc" => query.OrderByDescending(i => i.Price).ThenBy(i => i.NameAr),
                "newest" => query.OrderByDescending(i => i.CreatedDateTime),
                _ => query.OrderBy(i => i.NameAr)
            };

            var page = await ordered
                .Include(i => i.MenuCategory)
                .Include(i => i.Options)
                .Include(i => i.Addons)
                .AsSplitQuery()
                .PaginateAsync(pageNumber, pageSize, cancellationToken);

            var baseUrl = _currentRequest.BaseUrl;
            var items = page.Items
                .Select(i => MenuItemResponse.From(i).WithAbsoluteImageUrls(baseUrl))
                .ToList();

            return Response<PaginatedList<MenuItemResponse>>.Success(
                new PaginatedList<MenuItemResponse>(items, page.TotalCount, pageNumber, pageSize),
                "تم جلب أصناف المينيو بنجاح");
        }
    }
}
