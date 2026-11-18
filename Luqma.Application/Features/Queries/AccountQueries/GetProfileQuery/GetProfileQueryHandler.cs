using Luqma.Application.Features.Dtos.Responses.AccountResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using MediatR;

namespace Luqma.Application.Features.Queries.AccountQueries.GetProfileQuery
{
    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, Response<UserResponse>>
    {
        private readonly IAccountManager _accountManager;
        private readonly ICurrentUserService _currentUserService;

        public GetProfileQueryHandler(
            IAccountManager accountManager,
            ICurrentUserService currentUserService)
        {
            _accountManager = accountManager;
            _currentUserService = currentUserService;
        }

        public async Task<Response<UserResponse>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
                return Response<UserResponse>.Fail("غير مصرح");

            var user = await _accountManager.GetProfileAsync(userId);
            if (user == null)
                return Response<UserResponse>.Fail("المستخدم غير موجود");

            var response = new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Role = user.Role,
                EmailConfirmed = user.EmailConfirmed,
                IsArchived = user.IsArchived,
                CreateAt = user.CreateAt,
                UpdateAt = user.UpdateAt
            };

            return Response<UserResponse>.Success(response, "تم جلب الملف الشخصي بنجاح");
        }
    }
}
