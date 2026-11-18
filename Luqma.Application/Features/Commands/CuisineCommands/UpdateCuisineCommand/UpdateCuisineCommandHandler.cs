using Luqma.Application.Features.Dtos.Responses.CuisineResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Commands.CuisineCommands.UpdateCuisineCommand
{
    public class UpdateCuisineCommandHandler : IRequestHandler<UpdateCuisineCommand, Response<CuisineResponse>>
    {
        private readonly IGenericRepository<Cuisine> _cuisines;
        private readonly ICurrentRequestService _currentRequest;

        public UpdateCuisineCommandHandler(IGenericRepository<Cuisine> cuisines, ICurrentRequestService currentRequest)
        {
            _cuisines = cuisines;
            _currentRequest = currentRequest;
        }

        public async Task<Response<CuisineResponse>> Handle(UpdateCuisineCommand command, CancellationToken cancellationToken)
        {
            var cuisine = await _cuisines.GetAll()
                .FirstOrDefaultAsync(c => c.ID == command.Id && !c.IsArchived, cancellationToken);

            if (cuisine == null)
                return Response<CuisineResponse>.Fail("نوع الأكل غير موجود");

            var request = command.Request;
            var error = CuisineRequestValidator.Validate(request);
            if (error != null)
                return Response<CuisineResponse>.Fail(error);

            var nameAr = request.NameAr.Trim();
            var nameEn = request.NameEn.Trim();

            var exists = await _cuisines.GetAll()
                .AnyAsync(c => c.ID != cuisine.ID && !c.IsArchived &&
                               (c.NameAr == nameAr || c.NameEn == nameEn), cancellationToken);

            if (exists)
                return Response<CuisineResponse>.Fail("نوع الأكل هذا موجود بالفعل");

            cuisine.NameAr = nameAr;
            cuisine.NameEn = nameEn;
            cuisine.ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim();
            cuisine.DisplayOrder = request.DisplayOrder is > 0 ? request.DisplayOrder.Value : cuisine.DisplayOrder;
            cuisine.IsActive = request.IsActive;

            _cuisines.Update(cuisine);

            if (!await _cuisines.SaveChangesAsync())
                return Response<CuisineResponse>.Fail("فشل في تحديث نوع الأكل");

            var response = CuisineResponse.From(cuisine).WithAbsoluteImageUrl(_currentRequest.BaseUrl);
            return Response<CuisineResponse>.Success(response, "تم تحديث نوع الأكل بنجاح");
        }
    }
}
