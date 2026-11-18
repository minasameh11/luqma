using Luqma.Application.Features.Dtos.Responses.MenuResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Commands.MenuCommands.SetMenuCategoryActiveCommand
{
    public class SetMenuCategoryActiveCommandHandler : IRequestHandler<SetMenuCategoryActiveCommand, Response<MenuCategoryResponse>>
    {
        private readonly IGenericRepository<MenuCategory> _categories;
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentRequestService _currentRequest;

        public SetMenuCategoryActiveCommandHandler(
            IGenericRepository<MenuCategory> categories,
            ICurrentUserService currentUser,
            ICurrentRequestService currentRequest)
        {
            _categories = categories;
            _currentUser = currentUser;
            _currentRequest = currentRequest;
        }

        public async Task<Response<MenuCategoryResponse>> Handle(SetMenuCategoryActiveCommand command, CancellationToken cancellationToken)
        {
            var category = await _categories.GetAll()
                .Include(c => c.Store)
                .FirstOrDefaultAsync(c => c.ID == command.Id && !c.IsArchived, cancellationToken);

            if (category == null)
                return Response<MenuCategoryResponse>.Fail("القسم غير موجود");

            var accessError = MenuAccessGuard.ValidateStoreAccess(category.Store?.OwnerId, _currentUser);
            if (accessError != null)
                return Response<MenuCategoryResponse>.Fail(accessError);

            var message = command.IsActive ? "تم تفعيل القسم" : "تم إيقاف القسم";

            if (category.IsActive != command.IsActive)
            {
                category.IsActive = command.IsActive;
                _categories.Update(category);

                if (!await _categories.SaveChangesAsync())
                    return Response<MenuCategoryResponse>.Fail("فشل في تحديث حالة القسم");
            }
            else
            {
                message = command.IsActive ? "القسم مفعل بالفعل" : "القسم غير مفعل بالفعل";
            }

            var response = MenuCategoryResponse.From(category).WithAbsoluteImageUrl(_currentRequest.BaseUrl);
            return Response<MenuCategoryResponse>.Success(response, message);
        }
    }
}
