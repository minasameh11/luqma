using Luqma.Application.Features.Dtos.Responses.MenuResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Commands.MenuCommands.UpdateMenuCategoryCommand
{
    public class UpdateMenuCategoryCommandHandler : IRequestHandler<UpdateMenuCategoryCommand, Response<MenuCategoryResponse>>
    {
        private readonly IGenericRepository<MenuCategory> _categories;
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentRequestService _currentRequest;

        public UpdateMenuCategoryCommandHandler(
            IGenericRepository<MenuCategory> categories,
            ICurrentUserService currentUser,
            ICurrentRequestService currentRequest)
        {
            _categories = categories;
            _currentUser = currentUser;
            _currentRequest = currentRequest;
        }

        public async Task<Response<MenuCategoryResponse>> Handle(UpdateMenuCategoryCommand command, CancellationToken cancellationToken)
        {
            var category = await _categories.GetAll()
                .Include(c => c.Store)
                .FirstOrDefaultAsync(c => c.ID == command.Id && !c.IsArchived, cancellationToken);

            if (category == null)
                return Response<MenuCategoryResponse>.Fail("القسم غير موجود");

            var accessError = MenuAccessGuard.ValidateStoreAccess(category.Store?.OwnerId, _currentUser);
            if (accessError != null)
                return Response<MenuCategoryResponse>.Fail(accessError);

            var request = command.Request;
            var error = MenuRequestValidator.ValidateCategory(request);
            if (error != null)
                return Response<MenuCategoryResponse>.Fail(error);

            var nameAr = request.NameAr.Trim();
            var nameEn = request.NameEn.Trim();

            var exists = await _categories.GetAll()
                .AnyAsync(c => c.ID != category.ID && c.StoreId == category.StoreId && !c.IsArchived &&
                               (c.NameAr == nameAr || c.NameEn == nameEn), cancellationToken);

            if (exists)
                return Response<MenuCategoryResponse>.Fail("هذا القسم موجود بالفعل في المحل");

            category.NameAr = nameAr;
            category.NameEn = nameEn;
            category.ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim();
            category.DisplayOrder = request.DisplayOrder is > 0 ? request.DisplayOrder.Value : category.DisplayOrder;
            category.IsActive = request.IsActive;

            _categories.Update(category);

            if (!await _categories.SaveChangesAsync())
                return Response<MenuCategoryResponse>.Fail("فشل في تحديث القسم");

            var response = MenuCategoryResponse.From(category).WithAbsoluteImageUrl(_currentRequest.BaseUrl);
            return Response<MenuCategoryResponse>.Success(response, "تم تحديث القسم بنجاح");
        }
    }
}
