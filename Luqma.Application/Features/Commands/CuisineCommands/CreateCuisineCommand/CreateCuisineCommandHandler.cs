using Luqma.Application.Features.Dtos.Responses.CuisineResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Commands.CuisineCommands.CreateCuisineCommand
{
    public class CreateCuisineCommandHandler : IRequestHandler<CreateCuisineCommand, Response<CuisineResponse>>
    {
        private readonly IGenericRepository<Cuisine> _cuisines;
        private readonly ICurrentRequestService _currentRequest;

        public CreateCuisineCommandHandler(IGenericRepository<Cuisine> cuisines, ICurrentRequestService currentRequest)
        {
            _cuisines = cuisines;
            _currentRequest = currentRequest;
        }

        public async Task<Response<CuisineResponse>> Handle(CreateCuisineCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var error = CuisineRequestValidator.Validate(request);
            if (error != null)
                return Response<CuisineResponse>.Fail(error);

            var nameAr = request.NameAr.Trim();
            var nameEn = request.NameEn.Trim();

            var exists = await _cuisines.GetAll()
                .AnyAsync(c => !c.IsArchived && (c.NameAr == nameAr || c.NameEn == nameEn), cancellationToken);

            if (exists)
                return Response<CuisineResponse>.Fail("نوع الأكل هذا موجود بالفعل");

            var displayOrder = request.DisplayOrder is > 0
                ? request.DisplayOrder.Value
                : await GetNextDisplayOrderAsync(cancellationToken);

            var cuisine = new Cuisine
            {
                NameAr = nameAr,
                NameEn = nameEn,
                ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim(),
                DisplayOrder = displayOrder,
                IsActive = request.IsActive
            };

            _cuisines.Add(cuisine);

            if (!await _cuisines.SaveChangesAsync())
                return Response<CuisineResponse>.Fail("فشل في حفظ نوع الأكل");

            var response = CuisineResponse.From(cuisine).WithAbsoluteImageUrl(_currentRequest.BaseUrl);
            return Response<CuisineResponse>.Success(response, "تم إضافة نوع الأكل بنجاح");
        }

        private async Task<int> GetNextDisplayOrderAsync(CancellationToken cancellationToken)
        {
            var lastOrder = await _cuisines.GetAll()
                .Where(c => !c.IsArchived)
                .OrderByDescending(c => c.DisplayOrder)
                .Select(c => (int?)c.DisplayOrder)
                .FirstOrDefaultAsync(cancellationToken);

            return (lastOrder ?? 0) + 1;
        }
    }
}
