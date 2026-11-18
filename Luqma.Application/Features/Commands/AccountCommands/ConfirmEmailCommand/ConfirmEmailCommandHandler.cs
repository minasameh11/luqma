using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using MediatR;

namespace Luqma.Application.Features.Commands.AccountCommands.ConfirmEmailCommand
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Response<string>>
    {
        private readonly IAccountManager _accountManager;

        public ConfirmEmailCommandHandler(IAccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        public async Task<Response<string>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var result = await _accountManager.ConfirmEmailAsync(
                request.ConfirmEmailRequest.Email,
                request.ConfirmEmailRequest.Otp);

            if (!result)
                return Response<string>.Fail("رمز التحقق غير صحيح أو منتهي الصلاحية");

            return Response<string>.Success(string.Empty, "تم تأكيد البريد الإلكتروني بنجاح");
        }
    }
}
