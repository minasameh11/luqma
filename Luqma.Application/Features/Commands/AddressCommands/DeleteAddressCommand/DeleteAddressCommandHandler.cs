using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Commands.AddressCommands.DeleteAddressCommand
{
    public class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, Response<string>>
    {
        private readonly IGenericRepository<Address> _addresses;
        private readonly ICurrentUserService _currentUser;

        public DeleteAddressCommandHandler(IGenericRepository<Address> addresses, ICurrentUserService currentUser)
        {
            _addresses = addresses;
            _currentUser = currentUser;
        }

        public async Task<Response<string>> Handle(DeleteAddressCommand command, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUser.UserId, out var userId))
                return Response<string>.Fail("غير مصرح");

            var address = await _addresses.GetAll()
                .FirstOrDefaultAsync(a => a.ID == command.Id && a.UserId == userId && !a.IsArchived, cancellationToken);

            if (address == null)
                return Response<string>.Fail("العنوان غير موجود");

            var wasDefault = address.IsDefault;
            address.IsDefault = false;
            _addresses.Archived(address);

            // Deleting the default address promotes the most recently created one
            if (wasDefault)
            {
                var next = await _addresses.GetAll()
                    .Where(a => a.UserId == userId && !a.IsArchived && a.ID != address.ID)
                    .OrderByDescending(a => a.CreatedDateTime)
                    .FirstOrDefaultAsync(cancellationToken);

                if (next != null)
                {
                    next.IsDefault = true;
                    _addresses.Update(next);
                }
            }

            if (!await _addresses.SaveChangesAsync())
                return Response<string>.Fail("فشل في حذف العنوان");

            return Response<string>.Success(string.Empty, "تم حذف العنوان بنجاح");
        }
    }
}
