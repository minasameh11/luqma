using Luqma.Application.Features.Dtos.Responses.MenuResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Commands.MenuCommands.SetMenuItemActiveCommand
{
    public class SetMenuItemActiveCommandHandler : IRequestHandler<SetMenuItemActiveCommand, Response<MenuItemResponse>>
    {
        private readonly IGenericRepository<MenuItem> _items;
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentRequestService _currentRequest;

        public SetMenuItemActiveCommandHandler(
            IGenericRepository<MenuItem> items,
            ICurrentUserService currentUser,
            ICurrentRequestService currentRequest)
        {
            _items = items;
            _currentUser = currentUser;
            _currentRequest = currentRequest;
        }

        public async Task<Response<MenuItemResponse>> Handle(SetMenuItemActiveCommand command, CancellationToken cancellationToken)
        {
            var item = await _items.GetAll()
                .Include(i => i.MenuCategory)!.ThenInclude(c => c!.Store)
                .Include(i => i.Options)
                .Include(i => i.Addons)
                .FirstOrDefaultAsync(i => i.ID == command.Id && !i.IsArchived, cancellationToken);

            if (item == null)
                return Response<MenuItemResponse>.Fail("الصنف غير موجود");

            var accessError = MenuAccessGuard.ValidateStoreAccess(item.MenuCategory?.Store?.OwnerId, _currentUser);
            if (accessError != null)
                return Response<MenuItemResponse>.Fail(accessError);

            var message = command.IsActive ? "تم تفعيل الصنف" : "تم إيقاف الصنف";

            if (item.IsActive != command.IsActive)
            {
                item.IsActive = command.IsActive;
                _items.Update(item);

                if (!await _items.SaveChangesAsync())
                    return Response<MenuItemResponse>.Fail("فشل في تحديث حالة الصنف");
            }
            else
            {
                message = command.IsActive ? "الصنف مفعل بالفعل" : "الصنف غير مفعل بالفعل";
            }

            var response = MenuItemResponse.From(item).WithAbsoluteImageUrls(_currentRequest.BaseUrl);
            return Response<MenuItemResponse>.Success(response, message);
        }
    }
}
