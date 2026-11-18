using Luqma.Application.Features.Dtos.Responses.CategoryResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Queries.CategoryQueries.GetMainCategoriesQuery
{
    public class GetMainCategoriesQuery : IRequest<Response<List<MainCategoryResponse>>>
    {
    }
}
