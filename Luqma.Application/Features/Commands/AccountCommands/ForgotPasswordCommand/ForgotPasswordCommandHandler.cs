using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using MediatR;

namespace Luqma.Application.Features.Commands.AccountCommands.ForgotPasswordCommand
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Response<string>>
    {
        private readonly IAccountManager _accountManager;

        public ForgotPasswordCommandHandler(IAccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        public async Task<Response<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var result = await _accountManager.RequestPasswordResetAsync(request.ForgotPasswordRequest.Email);

            if (!result)
                return Response<string>.Fail("البريد الإلكتروني غير موجود");

            return Response<string>.Success(string.Empty, "تم إرسال رمز إعادة تعيين كلمة المرور إلى بريدك الإلكتروني");
        }
    }
}
