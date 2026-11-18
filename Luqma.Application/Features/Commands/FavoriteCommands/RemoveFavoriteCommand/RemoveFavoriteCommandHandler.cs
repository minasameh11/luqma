using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Commands.FavoriteCommands.RemoveFavoriteCommand
{
    public class RemoveFavoriteCommandHandler : IRequestHandler<RemoveFavoriteCommand, Response<Guid?>>
    {
        private readonly IGenericRepository<Favorite> _favorites;
        private readonly ICurrentUserService _currentUser;

        public RemoveFavoriteCommandHandler(IGenericRepository<Favorite> favorites, ICurrentUserService currentUser)
        {
            _favorites = favorites;
            _currentUser = currentUser;
        }

        public async Task<Response<Guid?>> Handle(RemoveFavoriteCommand command, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUser.UserId, out var userId))
                return Response<Guid?>.Fail("غير مصرح");

            if (command.MenuItemId == Guid.Empty)
                return Response<Guid?>.Fail("معرف الصنف مطلوب");

            var favorite = await _favorites.GetAll()
                .FirstOrDefaultAsync(
                    f => f.UserId == userId && f.MenuItemId == command.MenuItemId && !f.IsArchived,
                    cancellationToken);

            if (favorite is null)
                return Response<Guid?>.Fail("الصنف غير موجود في المفضلة");

            _favorites.Archived(favorite);

            if (!await _favorites.SaveChangesAsync())
                return Response<Guid?>.Fail("فشل في إزالة الصنف من المفضلة");

            return Response<Guid?>.Success(favorite.MenuItemId, "تم إزالة الصنف من المفضلة");
        }
    }
}
