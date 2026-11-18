using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Commands.AddressCommands.SetDefaultAddressCommand
{
    public class SetDefaultAddressCommandHandler : IRequestHandler<SetDefaultAddressCommand, Response<string>>
    {
        private readonly IGenericRepository<Address> _addresses;
        private readonly ICurrentUserService _currentUser;

        public SetDefaultAddressCommandHandler(IGenericRepository<Address> addresses, ICurrentUserService currentUser)
        {
            _addresses = addresses;
            _currentUser = currentUser;
        }

        public async Task<Response<string>> Handle(SetDefaultAddressCommand command, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUser.UserId, out var userId))
                return Response<string>.Fail("غير مصرح");

            var userAddresses = await _addresses.GetAll()
                .Where(a => a.UserId == userId && !a.IsArchived)
                .ToListAsync(cancellationToken);

            var target = userAddresses.FirstOrDefault(a => a.ID == command.Id);
            if (target == null)
                return Response<string>.Fail("العنوان غير موجود");

            if (target.IsDefault)
                return Response<string>.Success(string.Empty, "هذا هو العنوان الافتراضي بالفعل");

            foreach (var item in userAddresses.Where(a => a.IsDefault))
            {
                item.IsDefault = false;
                _addresses.Update(item);
            }

            target.IsDefault = true;
            _addresses.Update(target);

            if (!await _addresses.SaveChangesAsync())
                return Response<string>.Fail("فشل في تعيين العنوان الافتراضي");

            return Response<string>.Success(string.Empty, "تم تعيين العنوان الافتراضي بنجاح");
        }
    }
}
