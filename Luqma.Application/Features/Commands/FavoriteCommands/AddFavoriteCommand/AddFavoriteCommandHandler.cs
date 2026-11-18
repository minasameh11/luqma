using Luqma.Application.Features.Dtos.Responses.FavoriteResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Commands.FavoriteCommands.AddFavoriteCommand
{
    public class AddFavoriteCommandHandler : IRequestHandler<AddFavoriteCommand, Response<FavoriteResponse>>
    {
        private readonly IGenericRepository<Favorite> _favorites;
        private readonly IGenericRepository<MenuItem> _menuItems;
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentRequestService _currentRequest;

        public AddFavoriteCommandHandler(
            IGenericRepository<Favorite> favorites,
            IGenericRepository<MenuItem> menuItems,
            ICurrentUserService currentUser,
            ICurrentRequestService currentRequest)
        {
            _favorites = favorites;
            _menuItems = menuItems;
            _currentUser = currentUser;
            _currentRequest = currentRequest;
        }

        public async Task<Response<FavoriteResponse>> Handle(AddFavoriteCommand command, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUser.UserId, out var userId))
                return Response<FavoriteResponse>.Fail("غير مصرح");

            if (command.MenuItemId == Guid.Empty)
                return Response<FavoriteResponse>.Fail("معرف الصنف مطلوب");

            var menuItem = await _menuItems.GetAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.ID == command.MenuItemId, cancellationToken);

            if (menuItem is null || menuItem.IsArchived || !menuItem.IsActive)
                return Response<FavoriteResponse>.Fail("الصنف غير متاح");

            var favorite = await _favorites.GetAll()
                .FirstOrDefaultAsync(
                    f => f.UserId == userId && f.MenuItemId == menuItem.ID,
                    cancellationToken);

            var baseUrl = _currentRequest.BaseUrl;

            if (favorite is not null && !favorite.IsArchived)
                return Response<FavoriteResponse>.Success(
                    FavoriteResponse.From(favorite, menuItem, baseUrl), "الصنف موجود بالفعل في المفضلة");

            // Removing a favorite archives it; adding it again restores the same row
            if (favorite is not null)
            {
                _favorites.UnArchived(favorite);

                if (!await _favorites.SaveChangesAsync())
                    return Response<FavoriteResponse>.Fail("فشل في إضافة الصنف إلى المفضلة");

                return Response<FavoriteResponse>.Success(
                    FavoriteResponse.From(favorite, menuItem, baseUrl), "تم إضافة الصنف إلى المفضلة");
            }

            favorite = new Favorite
            {
                UserId = userId,
                MenuItemId = menuItem.ID
            };

            _favorites.Add(favorite);

            if (!await _favorites.SaveChangesAsync())
                return Response<FavoriteResponse>.Fail("فشل في إضافة الصنف إلى المفضلة");

            return Response<FavoriteResponse>.Success(
                FavoriteResponse.From(favorite, menuItem, baseUrl), "تم إضافة الصنف إلى المفضلة");
        }
    }
}
