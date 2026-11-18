using Luqma.Application.Features.Dtos.Requests.AccountRequests;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.AccountCommands.UpdateProfileCommand
{
    public class UpdateProfileCommand : IRequest<Response<string>>
    {
        public UpdateProfileRequest UpdateProfileRequest { get; set; } = new();
    }
}
