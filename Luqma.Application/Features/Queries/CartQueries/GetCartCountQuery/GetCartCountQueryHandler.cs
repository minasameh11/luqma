using Luqma.Application.Features.Dtos.Responses.CartResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Queries.CartQueries.GetCartCountQuery
{
    public class GetCartCountQueryHandler : IRequestHandler<GetCartCountQuery, Response<CartCountResponse>>
    {
        private readonly IGenericRepository<CartItem> _cartItems;
        private readonly ICurrentUserService _currentUser;

        public GetCartCountQueryHandler(IGenericRepository<CartItem> cartItems, ICurrentUserService currentUser)
        {
            _cartItems = cartItems;
            _currentUser = currentUser;
        }

        public async Task<Response<CartCountResponse>> Handle(GetCartCountQuery query, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUser.UserId, out var userId))
                return Response<CartCountResponse>.Fail("غير مصرح");

            var count = await _cartItems.GetAll()
                .AsNoTracking()
                .Where(c => c.UserId == userId && !c.IsArchived)
                .GroupBy(c => 1)
                .Select(g => new CartCountResponse
                {
                    LinesCount = g.Count(),
                    TotalQuantity = g.Sum(c => c.Quantity)
                })
                .FirstOrDefaultAsync(cancellationToken);

            return Response<CartCountResponse>.Success(
                count ?? new CartCountResponse(), "تم جلب عدد أصناف السلة بنجاح");
        }
    }
}
