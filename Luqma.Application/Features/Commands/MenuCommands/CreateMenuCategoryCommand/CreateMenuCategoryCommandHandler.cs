using Luqma.Application.Features.Dtos.Responses.MenuResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Commands.MenuCommands.CreateMenuCategoryCommand
{
    public class CreateMenuCategoryCommandHandler : IRequestHandler<CreateMenuCategoryCommand, Response<MenuCategoryResponse>>
    {
        private readonly IGenericRepository<MenuCategory> _categories;
        private readonly IGenericRepository<Store> _stores;
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentRequestService _currentRequest;

        public CreateMenuCategoryCommandHandler(
            IGenericRepository<MenuCategory> categories,
            IGenericRepository<Store> stores,
            ICurrentUserService currentUser,
            ICurrentRequestService currentRequest)
        {
            _categories = categories;
            _stores = stores;
            _currentUser = currentUser;
            _currentRequest = currentRequest;
        }

        public async Task<Response<MenuCategoryResponse>> Handle(CreateMenuCategoryCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var error = MenuRequestValidator.ValidateCategory(request);
            if (error != null)
                return Response<MenuCategoryResponse>.Fail(error);

            if (!request.StoreId.HasValue || request.StoreId.Value == Guid.Empty)
                return Response<MenuCategoryResponse>.Fail("المحل مطلوب");

            var store = await _stores.GetAll()
                .FirstOrDefaultAsync(s => s.ID == request.StoreId.Value && !s.IsArchived, cancellationToken);

            if (store == null)
                return Response<MenuCategoryResponse>.Fail("المحل غير موجود");

            var accessError = MenuAccessGuard.ValidateStoreAccess(store.OwnerId, _currentUser);
            if (accessError != null)
                return Response<MenuCategoryResponse>.Fail(accessError);

            var nameAr = request.NameAr.Trim();
            var nameEn = request.NameEn.Trim();

            var exists = await _categories.GetAll()
                .AnyAsync(c => c.StoreId == store.ID && !c.IsArchived &&
                               (c.NameAr == nameAr || c.NameEn == nameEn), cancellationToken);

            if (exists)
                return Response<MenuCategoryResponse>.Fail("هذا القسم موجود بالفعل في المحل");

            var displayOrder = request.DisplayOrder is > 0
                ? request.DisplayOrder.Value
                : await GetNextDisplayOrderAsync(store.ID, cancellationToken);

            var category = new MenuCategory
            {
                StoreId = store.ID,
                NameAr = nameAr,
                NameEn = nameEn,
                ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim(),
                DisplayOrder = displayOrder,
                IsActive = request.IsActive
            };

            _categories.Add(category);

            if (!await _categories.SaveChangesAsync())
                return Response<MenuCategoryResponse>.Fail("فشل في حفظ القسم");

            var response = MenuCategoryResponse.From(category).WithAbsoluteImageUrl(_currentRequest.BaseUrl);
            return Response<MenuCategoryResponse>.Success(response, "تم إضافة القسم بنجاح");
        }

        private async Task<int> GetNextDisplayOrderAsync(Guid storeId, CancellationToken cancellationToken)
        {
            var lastOrder = await _categories.GetAll()
                .Where(c => c.StoreId == storeId && !c.IsArchived)
                .OrderByDescending(c => c.DisplayOrder)
                .Select(c => (int?)c.DisplayOrder)
                .FirstOrDefaultAsync(cancellationToken);

            return (lastOrder ?? 0) + 1;
        }
    }
}
