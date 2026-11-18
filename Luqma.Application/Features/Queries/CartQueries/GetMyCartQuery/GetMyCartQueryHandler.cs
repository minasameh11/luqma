using Luqma.Application.Features.Dtos.Responses.CartResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Queries.CartQueries.GetMyCartQuery
{
    public class GetMyCartQueryHandler : IRequestHandler<GetMyCartQuery, Response<CartResponse>>
    {
        private readonly IGenericRepository<CartItem> _cartItems;
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentRequestService _currentRequest;

        public GetMyCartQueryHandler(
            IGenericRepository<CartItem> cartItems,
            ICurrentUserService currentUser,
            ICurrentRequestService currentRequest)
        {
            _cartItems = cartItems;
            _currentUser = currentUser;
            _currentRequest = currentRequest;
        }

        public async Task<Response<CartResponse>> Handle(GetMyCartQuery query, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUser.UserId, out var userId))
                return Response<CartResponse>.Fail("غير مصرح");

            var lines = await _cartItems.GetAll()
                .AsNoTracking()
                .Where(c => c.UserId == userId && !c.IsArchived)
                .Include(c => c.MenuItem)
                .Include(c => c.MenuItemOption)
                .Include(c => c.Addons)
                    .ThenInclude(a => a.MenuItemAddon)
                .OrderBy(c => c.CreatedDateTime)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            var baseUrl = _currentRequest.BaseUrl;

            var items = lines
                .Where(c => c.MenuItem is not null)
                .Select(c => CartItemResponse.From(
                    c,
                    c.MenuItem!,
                    c.MenuItemOption,
                    c.Addons
                        .Where(a => !a.IsArchived && a.MenuItemAddon is not null)
                        .Select(a => a.MenuItemAddon!),
                    baseUrl))
                .ToList();

            var cart = new CartResponse
            {
                Items = items,
                TotalQuantity = items.Sum(i => i.Quantity),
                SubTotal = items.Sum(i => i.LineTotal)
            };

            return Response<CartResponse>.Success(cart, "تم جلب السلة بنجاح");
        }
    }
}
