using Luqma.Application.Features.Dtos.Responses.AccountResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Infrastructure.Services;
using MediatR;

namespace Luqma.Application.Features.Commands.AccountCommands.LoginWithFacebookCommand
{
    public class LoginWithFacebookCommandHandler : IRequestHandler<LoginWithFacebookCommand, Response<AuthResponse>>
    {
        private readonly FacebookAuthService _facebookAuthService;
        private readonly IAccountManager _accountManager;

        public LoginWithFacebookCommandHandler(
            FacebookAuthService facebookAuthService,
            IAccountManager accountManager)
        {
            _facebookAuthService = facebookAuthService;
            _accountManager = accountManager;
        }

        public async Task<Response<AuthResponse>> Handle(LoginWithFacebookCommand request, CancellationToken cancellationToken)
        {
            var userInfo = await _facebookAuthService.ValidateFacebookTokenAsync(request.FacebookLoginRequest.AccessToken);

            if (userInfo == null)
                return Response<AuthResponse>.Fail("رمز Facebook غير صالح أو منتهي الصلاحية");

            var result = await _accountManager.ExternalLoginAsync(
                "Facebook",
                userInfo.ProviderUserId,
                userInfo.Email,
                userInfo.FullName);

            if (result == null)
                return Response<AuthResponse>.Fail("فشل في تسجيل الدخول عبر Facebook");

            return Response<AuthResponse>.Success(new AuthResponse
            {
                UserId = result.UserId,
                UserName = result.UserName,
                Role = result.Role ?? string.Empty,
                Token = result.Token,
                ExpiresAt = result.ExpiresAt
            }, "تم تسجيل الدخول بنجاح عبر Facebook");
        }
    }
}
