using Luqma.Application.Features.Dtos.Responses.CuisineResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Commands.CuisineCommands.SetCuisineActiveCommand
{
    public class SetCuisineActiveCommandHandler : IRequestHandler<SetCuisineActiveCommand, Response<CuisineResponse>>
    {
        private readonly IGenericRepository<Cuisine> _cuisines;
        private readonly ICurrentRequestService _currentRequest;

        public SetCuisineActiveCommandHandler(IGenericRepository<Cuisine> cuisines, ICurrentRequestService currentRequest)
        {
            _cuisines = cuisines;
            _currentRequest = currentRequest;
        }

        public async Task<Response<CuisineResponse>> Handle(SetCuisineActiveCommand command, CancellationToken cancellationToken)
        {
            var cuisine = await _cuisines.GetAll()
                .FirstOrDefaultAsync(c => c.ID == command.Id && !c.IsArchived, cancellationToken);

            if (cuisine == null)
                return Response<CuisineResponse>.Fail("نوع الأكل غير موجود");

            var message = command.IsActive ? "تم تفعيل نوع الأكل" : "تم إيقاف نوع الأكل";

            if (cuisine.IsActive != command.IsActive)
            {
                cuisine.IsActive = command.IsActive;
                _cuisines.Update(cuisine);

                if (!await _cuisines.SaveChangesAsync())
                    return Response<CuisineResponse>.Fail("فشل في تحديث حالة نوع الأكل");
            }
            else
            {
                message = command.IsActive ? "نوع الأكل مفعل بالفعل" : "نوع الأكل غير مفعل بالفعل";
            }

            var response = CuisineResponse.From(cuisine).WithAbsoluteImageUrl(_currentRequest.BaseUrl);
            return Response<CuisineResponse>.Success(response, message);
        }
    }
}
