using Luqma.Application.Features.Dtos.Requests.AccountRequests;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.AccountCommands.ResendOtpCommand
{
    public class ResendOtpCommand : IRequest<Response<string>>
    {
        public ResendOtpRequest ResendOtpRequest { get; set; } = new();
    }
}
