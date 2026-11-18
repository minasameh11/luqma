using Luqma.Application.Features.Dtos.Responses.MenuResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Commands.MenuCommands.CreateMenuItemCommand
{
    public class CreateMenuItemCommandHandler : IRequestHandler<CreateMenuItemCommand, Response<MenuItemResponse>>
    {
        private readonly IGenericRepository<MenuItem> _items;
        private readonly IGenericRepository<MenuCategory> _categories;
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentRequestService _currentRequest;

        public CreateMenuItemCommandHandler(
            IGenericRepository<MenuItem> items,
            IGenericRepository<MenuCategory> categories,
            ICurrentUserService currentUser,
            ICurrentRequestService currentRequest)
        {
            _items = items;
            _categories = categories;
            _currentUser = currentUser;
            _currentRequest = currentRequest;
        }

        public async Task<Response<MenuItemResponse>> Handle(CreateMenuItemCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var error = MenuRequestValidator.ValidateItem(request);
            if (error != null)
                return Response<MenuItemResponse>.Fail(error);

            var category = await _categories.GetAll()
                .Include(c => c.Store)
                .FirstOrDefaultAsync(c => c.ID == request.MenuCategoryId && !c.IsArchived, cancellationToken);

            if (category == null)
                return Response<MenuItemResponse>.Fail("القسم غير موجود");

            var accessError = MenuAccessGuard.ValidateStoreAccess(category.Store?.OwnerId, _currentUser);
            if (accessError != null)
                return Response<MenuItemResponse>.Fail(accessError);

            var item = new MenuItem
            {
                MenuCategoryId = category.ID,
                NameAr = request.NameAr.Trim(),
                NameEn = request.NameEn.Trim(),
                DescriptionAr = string.IsNullOrWhiteSpace(request.DescriptionAr) ? null : request.DescriptionAr.Trim(),
                DescriptionEn = string.IsNullOrWhiteSpace(request.DescriptionEn) ? null : request.DescriptionEn.Trim(),
                Price = request.Price,
                CompareAtPrice = request.CompareAtPrice,
                ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim(),
                IsSoldByWeight = request.IsSoldByWeight,
                WeightMin = request.IsSoldByWeight ? request.WeightMin : null,
                WeightMax = request.IsSoldByWeight ? request.WeightMax : null,
                WeightStep = request.IsSoldByWeight ? request.WeightStep : null,
                IsActive = request.IsActive,
                MenuCategory = category,
                Options = request.Options
                    .Select(o => new MenuItemOption
                    {
                        NameAr = o.NameAr.Trim(),
                        NameEn = o.NameEn.Trim(),
                        Price = o.Price
                    })
                    .ToList(),
                Addons = request.Addons
                    .Select(a => new MenuItemAddon
                    {
                        NameAr = a.NameAr.Trim(),
                        NameEn = a.NameEn.Trim(),
                        Price = a.Price,
                        ImageUrl = string.IsNullOrWhiteSpace(a.ImageUrl) ? null : a.ImageUrl.Trim()
                    })
                    .ToList()
            };

            _items.Add(item);

            if (!await _items.SaveChangesAsync())
                return Response<MenuItemResponse>.Fail("فشل في حفظ الصنف");

            var response = MenuItemResponse.From(item).WithAbsoluteImageUrls(_currentRequest.BaseUrl);
            return Response<MenuItemResponse>.Success(response, "تم إضافة الصنف بنجاح");
        }
    }
}
