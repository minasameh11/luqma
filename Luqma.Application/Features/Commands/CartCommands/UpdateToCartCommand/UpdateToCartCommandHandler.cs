using Luqma.Application.Features.Dtos.Responses.CartResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Commands.CartCommands.UpdateToCartCommand
{
    public class UpdateToCartCommandHandler : IRequestHandler<UpdateToCartCommand, Response<CartItemResponse>>
    {
        private readonly IGenericRepository<CartItem> _cartItems;
        private readonly IGenericRepository<MenuItem> _menuItems;
        private readonly IGenericRepository<MenuItemOption> _options;
        private readonly IGenericRepository<MenuItemAddon> _addons;
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentRequestService _currentRequest;

        public UpdateToCartCommandHandler(
            IGenericRepository<CartItem> cartItems,
            IGenericRepository<MenuItem> menuItems,
            IGenericRepository<MenuItemOption> options,
            IGenericRepository<MenuItemAddon> addons,
            ICurrentUserService currentUser,
            ICurrentRequestService currentRequest)
        {
            _cartItems = cartItems;
            _menuItems = menuItems;
            _options = options;
            _addons = addons;
            _currentUser = currentUser;
            _currentRequest = currentRequest;
        }

        public async Task<Response<CartItemResponse>> Handle(UpdateToCartCommand command, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUser.UserId, out var userId))
                return Response<CartItemResponse>.Fail("غير مصرح");

            var request = command.Request;
            var error = CartRequestValidator.ValidateUpdate(request);
            if (error != null)
                return Response<CartItemResponse>.Fail(error);

            var selection = await CartLineResolver.ResolveAsync(
                request.MenuItemId,
                request.MenuItemOptionId,
                request.AddonIds,
                _menuItems,
                _options,
                _addons,
                cancellationToken);

            if (!selection.IsResolved)
                return Response<CartItemResponse>.Fail(selection.Error!);

            var menuItem = selection.MenuItem!;
            var menuItemOption = selection.Option;
            var addons = selection.Addons;

            var cartItem = await _cartItems.GetAll()
                .FirstOrDefaultAsync(
                    c => c.UserId == userId
                         && c.MenuItemId == menuItem.ID
                         && c.ConfigurationSignature == selection.Signature
                         && !c.IsArchived,
                    cancellationToken);

            if (cartItem is null)
                return Response<CartItemResponse>.Fail("الصنف غير موجود في السلة، أضفه أولاً");

            var baseUrl = _currentRequest.BaseUrl;

            // Updating the quantity to zero removes the line from the cart
            if (request.Quantity == 0)
            {
                _cartItems.Archived(cartItem);

                if (!await _cartItems.SaveChangesAsync())
                    return Response<CartItemResponse>.Fail("فشل في حذف الصنف من السلة");

                return Response<CartItemResponse>.Success(
                    CartItemResponse.From(cartItem, menuItem, menuItemOption, addons, baseUrl), "تم حذف الصنف من السلة");
            }

            var quantityChanged = cartItem.Quantity != request.Quantity;
            cartItem.Quantity = request.Quantity;

            // Asking for the quantity the line already has is not a failure; there is nothing to save
            if (quantityChanged && !await _cartItems.SaveChangesAsync())
                return Response<CartItemResponse>.Fail("فشل في تحديث السلة");

            return Response<CartItemResponse>.Success(
                CartItemResponse.From(cartItem, menuItem, menuItemOption, addons, baseUrl),
                "تم تحديث كمية الصنف في السلة");
        }
    }
}
