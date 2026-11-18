using Luqma.Application.Extentions.Pagination;
using Luqma.Application.Features.Dtos.Responses.MenuResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Queries.MenuQueries.GetMenuItemsQuery
{
    public class GetMenuItemsQuery : IRequest<Response<PaginatedList<MenuItemResponse>>>
    {
        public Guid? StoreId { get; set; }
        public Guid? MenuCategoryId { get; set; }

        // Searched in the Arabic/English name and description
        public string? Search { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        // null returns items regardless of their active state
        public bool? IsActive { get; set; }

        // Only items currently on sale (compare-at price above the selling price)
        public bool? OnSale { get; set; }

        // Allow-listed: price_asc, price_desc, newest, name (default)
        public string? SortBy { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
