using Luqma.Application.Features.Dtos.Responses.AccountResponses;
using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using MediatR;

namespace Luqma.Application.Features.Queries.AccountQueries.GetAllUsersQuery
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Response<List<UserResponse>>>
    {
        private readonly IAccountManager _accountManager;

        public GetAllUsersQueryHandler(IAccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        public async Task<Response<List<UserResponse>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = (await _accountManager.GetAllUsers())
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Address = u.Address,
                    Role = u.Role,
                    EmailConfirmed = u.EmailConfirmed,
                    IsArchived = u.IsArchived,
                    CreateAt = u.CreateAt,
                    UpdateAt = u.UpdateAt
                })
                .ToList();

            return Response<List<UserResponse>>.Success(users, "تم جلب المستخدمين بنجاح");
        }
    }
}
