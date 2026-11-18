using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.AccountCommands.LogoutCommand
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Response<string>>
    {
        public Task<Response<string>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Response<string>.Success(string.Empty, "تم تسجيل الخروج بنجاح"));
        }
    }
}
