using Luqma.Application.Exceptions;
using Luqma.Application.Features.Dtos.Responses.AccountResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using MediatR;

namespace Luqma.Application.Features.Queries.AccountQueries.GetUserByIdQuery
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Response<UserResponse>>
    {
        private readonly IAccountManager _accountManager;

        public GetUserByIdQueryHandler(IAccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        public async Task<Response<UserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _accountManager.GetUserByIdAsync(request.Id);

            if (user == null)
                throw new NotFoundException("المستخدم غير موجود");

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

            return Response<UserResponse>.Success(response, "تم جلب المستخدم بنجاح");
        }
    }
}
