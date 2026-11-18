using Luqma.Application.Features.Dtos.Responses.AddressResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Queries.AddressQueries.GetAddressByIdQuery
{
    public class GetAddressByIdQueryHandler : IRequestHandler<GetAddressByIdQuery, Response<AddressResponse>>
    {
        private readonly IGenericRepository<Address> _addresses;
        private readonly ICurrentUserService _currentUser;

        public GetAddressByIdQueryHandler(IGenericRepository<Address> addresses, ICurrentUserService currentUser)
        {
            _addresses = addresses;
            _currentUser = currentUser;
        }

        public async Task<Response<AddressResponse>> Handle(GetAddressByIdQuery request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUser.UserId, out var userId))
                return Response<AddressResponse>.Fail("غير مصرح");

            var item = await _addresses.GetAll()
                .AsNoTracking()
                .Where(a => a.ID == request.Id && a.UserId == userId && !a.IsArchived)
                .Select(AddressResponse.Projection)
                .FirstOrDefaultAsync(cancellationToken);

            if (item == null)
                return Response<AddressResponse>.Fail("العنوان غير موجود");

            return Response<AddressResponse>.Success(item, "تم جلب العنوان بنجاح");
        }
    }
}
