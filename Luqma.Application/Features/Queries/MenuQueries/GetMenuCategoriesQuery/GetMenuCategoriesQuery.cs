using Luqma.Application.Features.Dtos.Responses.MenuResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Queries.MenuQueries.GetMenuCategoriesQuery
{
    public class GetMenuCategoriesQuery : IRequest<Response<List<MenuCategoryResponse>>>
    {
        public Guid StoreId { get; set; }

        // When true only active sections are returned (customer view)
        public bool OnlyActive { get; set; }
    }
}
