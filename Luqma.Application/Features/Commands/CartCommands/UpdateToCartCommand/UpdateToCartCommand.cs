using Luqma.Application.Features.Dtos.Requests.CartRequests;
using Luqma.Application.Features.Dtos.Responses.CartResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.CartCommands.UpdateToCartCommand
{
    public class UpdateToCartCommand : IRequest<Response<CartItemResponse>>
    {
        public UpdateCartRequest Request { get; set; } = new();
    }
}
