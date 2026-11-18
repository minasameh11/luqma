using Luqma.Application.Exceptions;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using MediatR;

namespace Luqma.Application.Features.Commands.AccountCommands.UnarchiveUserCommand
{
    public class UnarchiveUserCommandHandler : IRequestHandler<UnarchiveUserCommand, Response<string>>
    {
        private readonly IAccountManager _accountManager;

        public UnarchiveUserCommandHandler(IAccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        public async Task<Response<string>> Handle(UnarchiveUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _accountManager.GetUserByIdAsync(request.Id);
            if (user == null)
                throw new NotFoundException("المستخدم غير موجود");

            var result = await _accountManager.UnarchiveAsync(request.Id);
            if (!result)
                return Response<string>.Fail("فشل في إلغاء أرشفة المستخدم");

            return Response<string>.Success(string.Empty, "تم إلغاء أرشفة المستخدم بنجاح");
        }
    }
}
