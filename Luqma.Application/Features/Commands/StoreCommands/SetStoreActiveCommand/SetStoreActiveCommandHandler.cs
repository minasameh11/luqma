using Luqma.Application.Features.Dtos.Responses.StoreResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Helpers;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Commands.StoreCommands.SetStoreActiveCommand
{
    public class SetStoreActiveCommandHandler : IRequestHandler<SetStoreActiveCommand, Response<StoreResponse>>
    {
        private readonly IGenericRepository<Store> _stores;
        private readonly ICurrentRequestService _currentRequest;

        public SetStoreActiveCommandHandler(IGenericRepository<Store> stores, ICurrentRequestService currentRequest)
        {
            _stores = stores;
            _currentRequest = currentRequest;
        }

        public async Task<Response<StoreResponse>> Handle(SetStoreActiveCommand command, CancellationToken cancellationToken)
        {
            var store = await _stores.GetAll()
                .Include(s => s.MainCategory)
                .FirstOrDefaultAsync(s => s.ID == command.Id, cancellationToken);

            if (store == null || store.IsArchived)
                return Response<StoreResponse>.Fail("المحل غير موجود");

            if (store.IsActive == command.IsActive)
            {
                var unchanged = StoreResponse.From(store)
                    .WithAbsoluteIconUrl(_currentRequest.BaseUrl);
                unchanged.IsOpenNow = store.IsOpenAt(TimeOnly.FromDateTime(EgyptTime.NowLocal));
                return Response<StoreResponse>.Success(unchanged,
                    command.IsActive ? "المحل مفعل بالفعل" : "المحل غير مفعل بالفعل");
            }

            store.IsActive = command.IsActive;
            _stores.Update(store);

            if (!await _stores.SaveChangesAsync())
                return Response<StoreResponse>.Fail("فشل في تحديث حالة المحل");

            var response = StoreResponse.From(store);
            response.WithAbsoluteIconUrl(_currentRequest.BaseUrl);
            response.IsOpenNow = store.IsOpenAt(TimeOnly.FromDateTime(EgyptTime.NowLocal));

            return Response<StoreResponse>.Success(response,
                command.IsActive ? "تم تفعيل المحل" : "تم إيقاف المحل");
        }
    }
}
