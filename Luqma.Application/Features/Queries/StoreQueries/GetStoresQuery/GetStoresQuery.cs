using Luqma.Application.Extentions.Pagination;
using Luqma.Application.Features.Dtos.Responses.StoreResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Queries.StoreQueries.GetStoresQuery
{
    public class GetStoresQuery : IRequest<Response<PaginatedList<StoreResponse>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public Guid? MainCategoryId { get; set; }

        // Returns only the stores that serve this kitchen type
        public Guid? CuisineId { get; set; }

        // true: active stores currently inside working hours, false: everything else
        public bool? IsOpenNow { get; set; }

        public bool? IsActive { get; set; }

        // Client map location used to fill DistanceKm for every returned store
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
