using Luqma.Application.Features.Dtos.Responses.CategoryResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Commands.CategoryCommands.CreateMainCategoryCommand
{
    public class CreateMainCategoryCommandHandler : IRequestHandler<CreateMainCategoryCommand, Response<MainCategoryResponse>>
    {
        private readonly IGenericRepository<MainCategory> _categories;
        private readonly ICurrentRequestService _currentRequest;

        public CreateMainCategoryCommandHandler(IGenericRepository<MainCategory> categories, ICurrentRequestService currentRequest)
        {
            _categories = categories;
            _currentRequest = currentRequest;
        }

        public async Task<Response<MainCategoryResponse>> Handle(CreateMainCategoryCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var error = MainCategoryRequestValidator.Validate(request);
            if (error != null)
                return Response<MainCategoryResponse>.Fail(error);

            var nameAr = request.NameAr.Trim();
            var nameEn = request.NameEn.Trim();

            var alreadyExists = await _categories.GetAll()
                .AnyAsync(c => !c.IsArchived &&
                               (c.NameAr == nameAr || c.NameEn == nameEn), cancellationToken);

            if (alreadyExists)
                return Response<MainCategoryResponse>.Fail("هذا الصنف موجود بالفعل");

            var displayOrder = request.DisplayOrder is > 0
                ? request.DisplayOrder.Value
                : await GetNextDisplayOrderAsync(cancellationToken);

            var category = new MainCategory
            {
                NameAr = nameAr,
                NameEn = nameEn,
                IconUrl = request.IconUrl.Trim(),
                DisplayOrder = displayOrder,
                IsActive = request.IsActive
            };

            _categories.Add(category);

            if (!await _categories.SaveChangesAsync())
                return Response<MainCategoryResponse>.Fail("فشل في حفظ الصنف");

            var response = MainCategoryResponse.From(category).WithAbsoluteIconUrl(_currentRequest.BaseUrl);
            return Response<MainCategoryResponse>.Success(response, "تم إضافة الصنف بنجاح");
        }

        private async Task<int> GetNextDisplayOrderAsync(CancellationToken cancellationToken)
        {
            var lastOrder = await _categories.GetAll()
                .Where(c => !c.IsArchived)
                .OrderByDescending(c => c.DisplayOrder)
                .Select(c => (int?)c.DisplayOrder)
                .FirstOrDefaultAsync(cancellationToken);

            return (lastOrder ?? 0) + 1;
        }
    }
}
