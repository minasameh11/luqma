using Luqma.Application.Features.Dtos.Responses.FavoriteResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.FavoriteCommands.AddFavoriteCommand
{
    public class AddFavoriteCommand : IRequest<Response<FavoriteResponse>>
    {
        public Guid MenuItemId { get; set; }
    }
}
