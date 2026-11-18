using Luqma.Application.Features.Dtos.Responses.CategoryResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Queries.CategoryQueries.GetMainCategoriesQuery
{
    public class GetMainCategoriesQueryHandler : IRequestHandler<GetMainCategoriesQuery, Response<List<MainCategoryResponse>>>
    {
        private readonly IGenericRepository<MainCategory> _categories;
        private readonly ICurrentRequestService _currentRequest;

        public GetMainCategoriesQueryHandler(IGenericRepository<MainCategory> categories, ICurrentRequestService currentRequest)
        {
            _categories = categories;
            _currentRequest = currentRequest;
        }

        public async Task<Response<List<MainCategoryResponse>>> Handle(GetMainCategoriesQuery request, CancellationToken cancellationToken)
        {
            // Intentionally not paginated: main categories are a small, fixed list
            var items = await _categories.GetAll()
                .AsNoTracking()
                .Where(c => !c.IsArchived)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.NameAr)
                .Select(MainCategoryResponse.Projection)
                .ToListAsync(cancellationToken);

            var baseUrl = _currentRequest.BaseUrl;
            foreach (var item in items)
            {
                item.WithAbsoluteIconUrl(baseUrl);
            }

            return Response<List<MainCategoryResponse>>.Success(items, "تم جلب الأصناف بنجاح");
        }
    }
}
