using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.FavoriteCommands.RemoveFavoriteCommand
{
    public class RemoveFavoriteCommand : IRequest<Response<Guid?>>
    {
        public Guid MenuItemId { get; set; }
    }
}
