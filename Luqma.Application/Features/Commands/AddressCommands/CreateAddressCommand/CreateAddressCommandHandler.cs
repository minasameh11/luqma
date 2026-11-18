using Luqma.Application.Features.Dtos.Responses.AddressResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Commands.AddressCommands.CreateAddressCommand
{
    public class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, Response<AddressResponse>>
    {
        private readonly IGenericRepository<Address> _addresses;
        private readonly ICurrentUserService _currentUser;

        public CreateAddressCommandHandler(IGenericRepository<Address> addresses, ICurrentUserService currentUser)
        {
            _addresses = addresses;
            _currentUser = currentUser;
        }

        public async Task<Response<AddressResponse>> Handle(CreateAddressCommand command, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUser.UserId, out var userId))
                return Response<AddressResponse>.Fail("غير مصرح");

            var request = command.Request;
            var error = AddressRequestValidator.Validate(request);
            if (error != null)
                return Response<AddressResponse>.Fail(error);

            var activeCount = await _addresses.GetAll()
                .CountAsync(a => a.UserId == userId && !a.IsArchived, cancellationToken);

            if (activeCount >= AddressRequestValidator.MaxAddressesPerUser)
                return Response<AddressResponse>.Fail($"لا يمكن حفظ أكثر من {AddressRequestValidator.MaxAddressesPerUser} عناوين");

            // The first address is always the default one
            var makeDefault = request.IsDefault || activeCount == 0;
            if (makeDefault)
            {
                var currentDefaults = await _addresses.GetAll()
                    .Where(a => a.UserId == userId && !a.IsArchived && a.IsDefault)
                    .ToListAsync(cancellationToken);

                foreach (var item in currentDefaults)
                {
                    item.IsDefault = false;
                    _addresses.Update(item);
                }
            }

            var address = new Address
            {
                UserId = userId,
                Label = request.Label,
                Title = request.Title.Trim(),
                District = request.District?.Trim(),
                City = request.City.Trim(),
                Details = request.Details?.Trim(),
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                IsDefault = makeDefault
            };

            _addresses.Add(address);

            if (!await _addresses.SaveChangesAsync())
                return Response<AddressResponse>.Fail("فشل في حفظ العنوان");

            return Response<AddressResponse>.Success(AddressResponse.From(address), "تم حفظ العنوان بنجاح");
        }
    }
}
