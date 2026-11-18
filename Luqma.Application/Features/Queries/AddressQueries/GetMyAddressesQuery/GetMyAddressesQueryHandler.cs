using Luqma.Application.Features.Dtos.Responses.AddressResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Queries.AddressQueries.GetMyAddressesQuery
{
    public class GetMyAddressesQueryHandler : IRequestHandler<GetMyAddressesQuery, Response<List<AddressResponse>>>
    {
        private readonly IGenericRepository<Address> _addresses;
        private readonly ICurrentUserService _currentUser;

        public GetMyAddressesQueryHandler(IGenericRepository<Address> addresses, ICurrentUserService currentUser)
        {
            _addresses = addresses;
            _currentUser = currentUser;
        }

        public async Task<Response<List<AddressResponse>>> Handle(GetMyAddressesQuery request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUser.UserId, out var userId))
                return Response<List<AddressResponse>>.Fail("غير مصرح");

            // Intentionally not paginated: a user can hold at most MaxAddressesPerUser addresses
            var items = await _addresses.GetAll()
                .AsNoTracking()
                .Where(a => a.UserId == userId && !a.IsArchived)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.CreatedDateTime)
                .Select(AddressResponse.Projection)
                .ToListAsync(cancellationToken);

            return Response<List<AddressResponse>>.Success(items, "تم جلب العناوين بنجاح");
        }
    }
}
