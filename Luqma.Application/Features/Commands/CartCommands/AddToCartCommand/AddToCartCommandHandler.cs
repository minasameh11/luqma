using Luqma.Application.Features.Dtos.Responses.CartResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Commands.CartCommands.AddToCartCommand
{
    public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, Response<CartItemResponse>>
    {
        private readonly IGenericRepository<CartItem> _cartItems;
        private readonly IGenericRepository<MenuItem> _menuItems;
        private readonly IGenericRepository<MenuItemOption> _options;
        private readonly IGenericRepository<MenuItemAddon> _addons;
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentRequestService _currentRequest;

        public AddToCartCommandHandler(
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

        public async Task<Response<CartItemResponse>> Handle(AddToCartCommand command, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUser.UserId, out var userId))
                return Response<CartItemResponse>.Fail("غير مصرح");

            var request = command.Request;
            var error = CartRequestValidator.ValidateAdd(request);
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

            var existingLine = await _cartItems.GetAll()
                .FirstOrDefaultAsync(
                    c => c.UserId == userId
                         && c.MenuItemId == menuItem.ID
                         && c.ConfigurationSignature == selection.Signature,
                    cancellationToken);

            var baseUrl = _currentRequest.BaseUrl;

            // Adding only creates lines; changing a quantity is the update use case's job
            if (existingLine is not null && !existingLine.IsArchived)
                return Response<CartItemResponse>.Fail("الصنف موجود بالفعل في السلة، استخدم تحديث الكمية");

            // A previously removed line is restored instead of duplicated
            if (existingLine is not null)
            {
                _cartItems.UnArchived(existingLine);
                existingLine.Quantity = request.Quantity;

                if (!await _cartItems.SaveChangesAsync())
                    return Response<CartItemResponse>.Fail("فشل في إضافة الصنف إلى السلة");

                return Response<CartItemResponse>.Success(
                    CartItemResponse.From(existingLine, menuItem, menuItemOption, addons, baseUrl),
                    "تم إضافة الصنف إلى السلة");
            }

            var cartItem = new CartItem
            {
                UserId = userId,
                MenuItemId = menuItem.ID,
                MenuItemOptionId = menuItemOption?.ID,
                ConfigurationSignature = selection.Signature,
                Quantity = request.Quantity,
                Addons = addons.Select(a => new CartItemAddon { MenuItemAddonId = a.ID }).ToList()
            };

            _cartItems.Add(cartItem);

            if (!await _cartItems.SaveChangesAsync())
                return Response<CartItemResponse>.Fail("فشل في إضافة الصنف إلى السلة");

            return Response<CartItemResponse>.Success(
                CartItemResponse.From(cartItem, menuItem, menuItemOption, addons, baseUrl),
                "تم إضافة الصنف إلى السلة");
        }
    }
}
