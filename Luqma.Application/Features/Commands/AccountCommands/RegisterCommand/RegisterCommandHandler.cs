using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using MediatR;

namespace Luqma.Application.Features.Commands.AccountCommands.RegisterCommand
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Response<string>>
    {
        private readonly IAccountManager _accountManager;

        public RegisterCommandHandler(IAccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        public async Task<Response<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var result = await _accountManager.RegisterAsync(new Domain.AccountsDtos.RegisterDto
            {
                UserName = request.RegisterRequest.UserName,
                Address = request.RegisterRequest.Address,
                Password = request.RegisterRequest.Password,
                ConfirmPassword = request.RegisterRequest.ConfirmPassword,
                Email = request.RegisterRequest.Email,
                PhoneNumber = request.RegisterRequest.PhoneNumber,
                Role = request.RegisterRequest.Role
            });

            if (result == "تم التسجيل بنجاح، برجاء تأكيد البريد الإلكتروني")
                return Response<string>.Success(result, result);

            return Response<string>.Fail(result);
        }
    }
}
