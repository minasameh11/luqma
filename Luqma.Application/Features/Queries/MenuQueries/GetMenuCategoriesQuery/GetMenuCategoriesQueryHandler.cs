using Luqma.Application.Features.Dtos.Responses.MenuResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Queries.MenuQueries.GetMenuCategoriesQuery
{
    public class GetMenuCategoriesQueryHandler : IRequestHandler<GetMenuCategoriesQuery, Response<List<MenuCategoryResponse>>>
    {
        private readonly IGenericRepository<MenuCategory> _categories;
        private readonly ICurrentRequestService _currentRequest;

        public GetMenuCategoriesQueryHandler(IGenericRepository<MenuCategory> categories, ICurrentRequestService currentRequest)
        {
            _categories = categories;
            _currentRequest = currentRequest;
        }

        public async Task<Response<List<MenuCategoryResponse>>> Handle(GetMenuCategoriesQuery request, CancellationToken cancellationToken)
        {
            if (request.StoreId == Guid.Empty)
                return Response<List<MenuCategoryResponse>>.Fail("المحل مطلوب");

            var query = _categories.GetAll()
                .AsNoTracking()
                .Where(c => c.StoreId == request.StoreId && !c.IsArchived);

            if (request.OnlyActive)
                query = query.Where(c => c.IsActive);

            // Intentionally not paginated: a single store menu has a small, fixed set of sections
            var items = await query
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.NameAr)
                .Select(MenuCategoryResponse.Projection)
                .ToListAsync(cancellationToken);

            var baseUrl = _currentRequest.BaseUrl;
            foreach (var item in items)
            {
                item.WithAbsoluteImageUrl(baseUrl);
            }

            return Response<List<MenuCategoryResponse>>.Success(items, "تم جلب أقسام المينيو بنجاح");
        }
    }
}
