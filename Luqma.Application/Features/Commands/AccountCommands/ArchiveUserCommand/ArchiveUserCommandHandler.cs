using Luqma.Application.Exceptions;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using MediatR;

namespace Luqma.Application.Features.Commands.AccountCommands.ArchiveUserCommand
{
    public class ArchiveUserCommandHandler : IRequestHandler<ArchiveUserCommand, Response<string>>
    {
        private readonly IAccountManager _accountManager;

        public ArchiveUserCommandHandler(IAccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        public async Task<Response<string>> Handle(ArchiveUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _accountManager.GetUserByIdAsync(request.Id);
            if (user == null)
                throw new NotFoundException("المستخدم غير موجود");

            var result = await _accountManager.ArchiveAsync(request.Id);
            if (!result)
                return Response<string>.Fail("فشل في أرشفة المستخدم");

            return Response<string>.Success(string.Empty, "تم أرشفة المستخدم بنجاح");
        }
    }
}
