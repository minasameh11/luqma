using Luqma.Application.Features.Dtos.Responses.AccountResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Infrastructure.Services;
using MediatR;

namespace Luqma.Application.Features.Commands.AccountCommands.LoginWithGoogleCommand
{
    public class LoginWithGoogleCommandHandler : IRequestHandler<LoginWithGoogleCommand, Response<AuthResponse>>
    {
        private readonly GoogleAuthService _googleAuthService;
        private readonly IAccountManager _accountManager;

        public LoginWithGoogleCommandHandler(
            GoogleAuthService googleAuthService,
            IAccountManager accountManager)
        {
            _googleAuthService = googleAuthService;
            _accountManager = accountManager;
        }

        public async Task<Response<AuthResponse>> Handle(LoginWithGoogleCommand request, CancellationToken cancellationToken)
        {
            var userInfo = await _googleAuthService.ValidateGoogleTokenAsync(request.GoogleLoginRequest.IdToken);

            if (userInfo == null)
                return Response<AuthResponse>.Fail("رمز Google غير صالح أو منتهي الصلاحية");

            var result = await _accountManager.ExternalLoginAsync(
                "Google",
                userInfo.ProviderUserId,
                userInfo.Email,
                userInfo.FullName);

            if (result == null)
                return Response<AuthResponse>.Fail("فشل في تسجيل الدخول عبر Google");

            return Response<AuthResponse>.Success(new AuthResponse
            {
                UserId = result.UserId,
                UserName = result.UserName,
                Role = result.Role ?? string.Empty,
                Token = result.Token,
                ExpiresAt = result.ExpiresAt
            }, "تم تسجيل الدخول بنجاح عبر Google");
        }
    }
}
