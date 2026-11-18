using Luqma.Application.Features.Dtos.Responses.AddressResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Commands.AddressCommands.UpdateAddressCommand
{
    public class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, Response<AddressResponse>>
    {
        private readonly IGenericRepository<Address> _addresses;
        private readonly ICurrentUserService _currentUser;

        public UpdateAddressCommandHandler(IGenericRepository<Address> addresses, ICurrentUserService currentUser)
        {
            _addresses = addresses;
            _currentUser = currentUser;
        }

        public async Task<Response<AddressResponse>> Handle(UpdateAddressCommand command, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUser.UserId, out var userId))
                return Response<AddressResponse>.Fail("غير مصرح");

            var request = command.Request;
            var error = AddressRequestValidator.Validate(request);
            if (error != null)
                return Response<AddressResponse>.Fail(error);

            // Ownership is enforced in the query itself
            var address = await _addresses.GetAll()
                .FirstOrDefaultAsync(a => a.ID == command.Id && a.UserId == userId && !a.IsArchived, cancellationToken);

            if (address == null)
                return Response<AddressResponse>.Fail("العنوان غير موجود");

            // Promote to default, but never leave the user without a default address
            if (request.IsDefault && !address.IsDefault)
            {
                var currentDefaults = await _addresses.GetAll()
                    .Where(a => a.UserId == userId && !a.IsArchived && a.IsDefault && a.ID != address.ID)
                    .ToListAsync(cancellationToken);

                foreach (var item in currentDefaults)
                {
                    item.IsDefault = false;
                    _addresses.Update(item);
                }
                address.IsDefault = true;
            }

            address.Label = request.Label;
            address.Title = request.Title.Trim();
            address.District = request.District?.Trim();
            address.City = request.City.Trim();
            address.Details = request.Details?.Trim();
            address.Latitude = request.Latitude;
            address.Longitude = request.Longitude;

            _addresses.Update(address);

            if (!await _addresses.SaveChangesAsync())
                return Response<AddressResponse>.Fail("فشل في تحديث العنوان");

            return Response<AddressResponse>.Success(AddressResponse.From(address), "تم تحديث العنوان بنجاح");
        }
    }
}
