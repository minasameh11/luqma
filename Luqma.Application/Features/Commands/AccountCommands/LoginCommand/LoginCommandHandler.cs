using Luqma.Application.Features.Dtos.Responses.AccountResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using MediatR;

namespace Luqma.Application.Features.Commands.AccountCommands.LoginCommand
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Response<AuthResponse>>
    {
        private readonly IAccountManager _accountManager;

        public LoginCommandHandler(IAccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        public async Task<Response<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var result = await _accountManager.LoginAsync(new Domain.AccountsDtos.LoginDto
            {
                Email = request.LoginRequest.Email,
                Password = request.LoginRequest.Password
            });

            if (result == null)
                return Response<AuthResponse>.Fail("البريد الإلكتروني أو كلمة المرور غير صحيحة");

            var authResponse = new AuthResponse
            {
                UserId = result.UserId,
                UserName = result.UserName,
                Role = result.Role ?? string.Empty,
                Token = result.Token,
                ExpiresAt = result.ExpiresAt
            };

            return Response<AuthResponse>.Success(authResponse, "تم تسجيل الدخول بنجاح");
        }
    }
}
