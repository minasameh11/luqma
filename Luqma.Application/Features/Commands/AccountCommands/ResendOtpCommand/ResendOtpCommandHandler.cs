using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using MediatR;

namespace Luqma.Application.Features.Commands.AccountCommands.ResendOtpCommand
{
    public class ResendOtpCommandHandler : IRequestHandler<ResendOtpCommand, Response<string>>
    {
        private readonly IAccountManager _accountManager;

        public ResendOtpCommandHandler(IAccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        public async Task<Response<string>> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
        {
            var result = await _accountManager.ResendOtpAsync(request.ResendOtpRequest.Email);

            if (result != "تم إعادة إرسال رمز التحقق بنجاح")
                return Response<string>.Fail(result);

            return Response<string>.Success(string.Empty, "تم إعادة إرسال رمز التحقق بنجاح");
        }
    }
}
