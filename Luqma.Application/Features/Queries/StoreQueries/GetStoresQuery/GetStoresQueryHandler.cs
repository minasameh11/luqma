using Luqma.Application.Extentions.Pagination;
using Luqma.Application.Features.Dtos.Responses.CuisineResponses;
using Luqma.Application.Features.Dtos.Responses.StoreResponses;
using Luqma.Application.Helpers;
using Luqma.Application.Wapper;
using Luqma.Domain.Helpers;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Queries.StoreQueries.GetStoresQuery
{
    public class GetStoresQueryHandler : IRequestHandler<GetStoresQuery, Response<PaginatedList<StoreResponse>>>
    {
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 50;

        private readonly IGenericRepository<Store> _stores;
        private readonly IGenericRepository<StoreCuisine> _storeCuisines;
        private readonly IGenericRepository<StoreImage> _storeImages;
        private readonly ICurrentRequestService _currentRequest;

        public GetStoresQueryHandler(
            IGenericRepository<Store> stores,
            IGenericRepository<StoreCuisine> storeCuisines,
            IGenericRepository<StoreImage> storeImages,
            ICurrentRequestService currentRequest)
        {
            _stores = stores;
            _storeCuisines = storeCuisines;
            _storeImages = storeImages;
            _currentRequest = currentRequest;
        }

        public async Task<Response<PaginatedList<StoreResponse>>> Handle(GetStoresQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize is < 1 or > MaxPageSize
                ? (request.PageSize is < 1 ? DefaultPageSize : MaxPageSize)
                : request.PageSize;

            var now = TimeOnly.FromDateTime(EgyptTime.NowLocal);

            var query = _stores.GetAll()
                .AsNoTracking()
                .Where(s => !s.IsArchived);

            if (request.MainCategoryId.HasValue && request.MainCategoryId.Value != Guid.Empty)
                query = query.Where(s => s.MainCategoryId == request.MainCategoryId.Value);

            if (request.CuisineId.HasValue && request.CuisineId.Value != Guid.Empty)
            {
                var cuisineId = request.CuisineId.Value;
                query = query.Where(s => _storeCuisines.GetAll()
                    .Any(sc => sc.StoreId == s.ID && sc.CuisineId == cuisineId && !sc.IsArchived));
            }

            if (request.IsActive.HasValue)
                query = query.Where(s => s.IsActive == request.IsActive.Value);

            if (request.IsOpenNow.HasValue)
            {
                // Mirrors WorkingHours.IsOpen so the filter runs in the database
                var isOpenNow = request.IsOpenNow.Value;
                query = isOpenNow
                    ? query.Where(s => s.IsActive && (
                        s.OpenTime == s.CloseTime ||
                        (s.OpenTime < s.CloseTime
                            ? s.OpenTime <= now && now < s.CloseTime
                            : now >= s.OpenTime || now < s.CloseTime)))
                    : query.Where(s => !s.IsActive || !(
                        s.OpenTime == s.CloseTime ||
                        (s.OpenTime < s.CloseTime
                            ? s.OpenTime <= now && now < s.CloseTime
                            : now >= s.OpenTime || now < s.CloseTime)));
            }

            var page = await query
                .OrderBy(s => s.NameAr)
                .Select(StoreResponse.Projection)
                .PaginateAsync(pageNumber, pageSize, cancellationToken);

            var baseUrl = _currentRequest.BaseUrl;
            var storeIds = page.Items.Select(i => i.Id).ToList();

            var cuisinesByStore = storeIds.Count == 0
                ? new Dictionary<Guid, List<Cuisine>>()
                : (await _storeCuisines.GetAll()
                    .AsNoTracking()
                    .Where(sc => storeIds.Contains(sc.StoreId) && !sc.IsArchived && sc.Cuisine != null)
                    .Select(sc => new { sc.StoreId, Cuisine = sc.Cuisine! })
                    .ToListAsync(cancellationToken))
                  .GroupBy(x => x.StoreId)
                  .ToDictionary(g => g.Key, g => g.Select(x => x.Cuisine).OrderBy(c => c.DisplayOrder).ToList());

            var imagesByStore = storeIds.Count == 0
                ? new Dictionary<Guid, List<string>>()
                : (await _storeImages.GetAll()
                    .AsNoTracking()
                    .Where(i => storeIds.Contains(i.StoreId) && !i.IsArchived)
                    .Select(i => new { i.StoreId, i.ImageUrl, i.DisplayOrder })
                    .ToListAsync(cancellationToken))
                  .GroupBy(x => x.StoreId)
                  .ToDictionary(
                      g => g.Key,
                      g => g.OrderBy(x => x.DisplayOrder)
                            .Select(x => ImageUrlResolver.ToAbsoluteUrl(baseUrl, x.ImageUrl))
                            .ToList());

            foreach (var item in page.Items)
            {
                item.WithAbsoluteIconUrl(baseUrl);
                item.WithDistanceFrom(request.Latitude, request.Longitude);
                item.IsOpenNow = item.IsActive && WorkingHours.IsOpen(item.OpenTime, item.CloseTime, now);

                if (cuisinesByStore.TryGetValue(item.Id, out var cuisines))
                {
                    item.Cuisines = cuisines
                        .Select(c => CuisineResponse.From(c).WithAbsoluteImageUrl(baseUrl))
                        .ToList();
                }

                if (imagesByStore.TryGetValue(item.Id, out var images))
                    item.Images = images;
            }

            return Response<PaginatedList<StoreResponse>>.Success(page, "تم جلب المحلات بنجاح");
        }
    }
}
