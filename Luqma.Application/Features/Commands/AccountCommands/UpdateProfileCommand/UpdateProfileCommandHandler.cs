using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using MediatR;

namespace Luqma.Application.Features.Commands.AccountCommands.UpdateProfileCommand
{
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Response<string>>
    {
        private readonly IAccountManager _accountManager;
        private readonly ICurrentUserService _currentUserService;

        public UpdateProfileCommandHandler(
            IAccountManager accountManager,
            ICurrentUserService currentUserService)
        {
            _accountManager = accountManager;
            _currentUserService = currentUserService;
        }

        public async Task<Response<string>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
                return Response<string>.Fail("غير مصرح");

            var result = await _accountManager.UpdateProfileAsync(userId, new Domain.AccountsDtos.UpdateAccountDto
            {
                Id = userId,
                UserName = request.UpdateProfileRequest.UserName,
                Email = request.UpdateProfileRequest.Email,
                PhoneNumber = request.UpdateProfileRequest.PhoneNumber,
                Address = request.UpdateProfileRequest.Address
            });

            if (!result)
                return Response<string>.Fail("فشل في تحديث الملف الشخصي");

            return Response<string>.Success(string.Empty, "تم تحديث الملف الشخصي بنجاح");
        }
    }
}
