using Luqma.Application.Features.Dtos.Responses.CuisineResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Queries.CuisineQueries.GetCuisinesQuery
{
    public class GetCuisinesQueryHandler : IRequestHandler<GetCuisinesQuery, Response<List<CuisineResponse>>>
    {
        private readonly IGenericRepository<Cuisine> _cuisines;
        private readonly ICurrentRequestService _currentRequest;

        public GetCuisinesQueryHandler(IGenericRepository<Cuisine> cuisines, ICurrentRequestService currentRequest)
        {
            _cuisines = cuisines;
            _currentRequest = currentRequest;
        }

        public async Task<Response<List<CuisineResponse>>> Handle(GetCuisinesQuery request, CancellationToken cancellationToken)
        {
            var query = _cuisines.GetAll()
                .AsNoTracking()
                .Where(c => !c.IsArchived);

            if (request.OnlyActive)
                query = query.Where(c => c.IsActive);

            // Intentionally not paginated: the list of kitchen types is small and fixed
            var items = await query
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.NameAr)
                .Select(CuisineResponse.Projection)
                .ToListAsync(cancellationToken);

            var baseUrl = _currentRequest.BaseUrl;
            foreach (var item in items)
            {
                item.WithAbsoluteImageUrl(baseUrl);
            }

            return Response<List<CuisineResponse>>.Success(items, "تم جلب أنواع الأكل بنجاح");
        }
    }
}
