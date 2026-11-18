using Luqma.Application.Features.Dtos.Responses.AccountResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Infrastructure.Services;
using MediatR;

namespace Luqma.Application.Features.Commands.AccountCommands.LoginWithAppleCommand
{
    public class LoginWithAppleCommandHandler : IRequestHandler<LoginWithAppleCommand, Response<AuthResponse>>
    {
        private readonly AppleAuthService _appleAuthService;
        private readonly IAccountManager _accountManager;

        public LoginWithAppleCommandHandler(
            AppleAuthService appleAuthService,
            IAccountManager accountManager)
        {
            _appleAuthService = appleAuthService;
            _accountManager = accountManager;
        }

        public async Task<Response<AuthResponse>> Handle(LoginWithAppleCommand request, CancellationToken cancellationToken)
        {
            var userInfo = await _appleAuthService.ValidateAppleTokenAsync(request.AppleLoginRequest.IdentityToken);

            if (userInfo == null)
                return Response<AuthResponse>.Fail("رمز Apple غير صالح أو منتهي الصلاحية");

            var result = await _accountManager.ExternalLoginAsync(
                "Apple",
                userInfo.ProviderUserId,
                userInfo.Email,
                userInfo.FullName);

            if (result == null)
                return Response<AuthResponse>.Fail("فشل في تسجيل الدخول عبر Apple");

            return Response<AuthResponse>.Success(new AuthResponse
            {
                UserId = result.UserId,
                UserName = result.UserName,
                Role = result.Role ?? string.Empty,
                Token = result.Token,
                ExpiresAt = result.ExpiresAt
            }, "تم تسجيل الدخول بنجاح عبر Apple");
        }
    }
}
