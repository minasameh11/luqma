using Luqma.Application.Features.Dtos.Responses.MenuResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Commands.MenuCommands.UpdateMenuItemCommand
{
    public class UpdateMenuItemCommandHandler : IRequestHandler<UpdateMenuItemCommand, Response<MenuItemResponse>>
    {
        private readonly IGenericRepository<MenuItem> _items;
        private readonly IGenericRepository<MenuCategory> _categories;
        private readonly IGenericRepository<MenuItemOption> _options;
        private readonly IGenericRepository<MenuItemAddon> _addons;
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentRequestService _currentRequest;

        public UpdateMenuItemCommandHandler(
            IGenericRepository<MenuItem> items,
            IGenericRepository<MenuCategory> categories,
            IGenericRepository<MenuItemOption> options,
            IGenericRepository<MenuItemAddon> addons,
            ICurrentUserService currentUser,
            ICurrentRequestService currentRequest)
        {
            _items = items;
            _categories = categories;
            _options = options;
            _addons = addons;
            _currentUser = currentUser;
            _currentRequest = currentRequest;
        }

        public async Task<Response<MenuItemResponse>> Handle(UpdateMenuItemCommand command, CancellationToken cancellationToken)
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

            var request = command.Request;
            var error = MenuRequestValidator.ValidateItem(request);
            if (error != null)
                return Response<MenuItemResponse>.Fail(error);

            // The item can move between categories of the same store only
            if (request.MenuCategoryId != item.MenuCategoryId)
            {
                var targetCategory = await _categories.GetAll()
                    .Include(c => c.Store)
                    .FirstOrDefaultAsync(c => c.ID == request.MenuCategoryId && !c.IsArchived, cancellationToken);

                if (targetCategory == null)
                    return Response<MenuItemResponse>.Fail("القسم غير موجود");

                if (targetCategory.StoreId != item.MenuCategory?.StoreId)
                    return Response<MenuItemResponse>.Fail("لا يمكن نقل الصنف إلى قسم في محل آخر");

                item.MenuCategoryId = targetCategory.ID;
                item.MenuCategory = targetCategory;
            }

            item.NameAr = request.NameAr.Trim();
            item.NameEn = request.NameEn.Trim();
            item.DescriptionAr = string.IsNullOrWhiteSpace(request.DescriptionAr) ? null : request.DescriptionAr.Trim();
            item.DescriptionEn = string.IsNullOrWhiteSpace(request.DescriptionEn) ? null : request.DescriptionEn.Trim();
            item.Price = request.Price;
            item.CompareAtPrice = request.CompareAtPrice;
            item.ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim();
            item.IsSoldByWeight = request.IsSoldByWeight;
            item.WeightMin = request.IsSoldByWeight ? request.WeightMin : null;
            item.WeightMax = request.IsSoldByWeight ? request.WeightMax : null;
            item.WeightStep = request.IsSoldByWeight ? request.WeightStep : null;
            item.IsActive = request.IsActive;

            // Options and addons are replaced as a whole (existing rows are archived, not deleted)
            foreach (var option in item.Options.Where(o => !o.IsArchived).ToList())
            {
                _options.Archived(option);
            }

            foreach (var addon in item.Addons.Where(a => !a.IsArchived).ToList())
            {
                _addons.Archived(addon);
            }

            item.Options = request.Options
                .Select(o => new MenuItemOption
                {
                    MenuItemId = item.ID,
                    NameAr = o.NameAr.Trim(),
                    NameEn = o.NameEn.Trim(),
                    Price = o.Price
                })
                .ToList();

            item.Addons = request.Addons
                .Select(a => new MenuItemAddon
                {
                    MenuItemId = item.ID,
                    NameAr = a.NameAr.Trim(),
                    NameEn = a.NameEn.Trim(),
                    Price = a.Price,
                    ImageUrl = string.IsNullOrWhiteSpace(a.ImageUrl) ? null : a.ImageUrl.Trim()
                })
                .ToList();

            _items.Update(item);

            if (!await _items.SaveChangesAsync())
                return Response<MenuItemResponse>.Fail("فشل في تحديث الصنف");

            var response = MenuItemResponse.From(item).WithAbsoluteImageUrls(_currentRequest.BaseUrl);
            return Response<MenuItemResponse>.Success(response, "تم تحديث الصنف بنجاح");
        }
    }
}
