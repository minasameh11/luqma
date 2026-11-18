using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using MediatR;

namespace Luqma.Application.Features.Commands.AccountCommands.ResetPasswordCommand
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Response<string>>
    {
        private readonly IAccountManager _accountManager;

        public ResetPasswordCommandHandler(IAccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        public async Task<Response<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var result = await _accountManager.ResetPasswordAsync(
                request.ResetPasswordRequest.Email,
                request.ResetPasswordRequest.Otp,
                request.ResetPasswordRequest.NewPassword);

            if (!result)
                return Response<string>.Fail("رمز التحقق غير صحيح أو منتهي الصلاحية");

            return Response<string>.Success(string.Empty, "تم إعادة تعيين كلمة المرور بنجاح");
        }
    }
}
