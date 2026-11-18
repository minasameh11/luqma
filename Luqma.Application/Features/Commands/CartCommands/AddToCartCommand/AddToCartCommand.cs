using Luqma.Application.Features.Dtos.Requests.CartRequests;
using Luqma.Application.Features.Dtos.Responses.CartResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.CartCommands.AddToCartCommand
{
    public class AddToCartCommand : IRequest<Response<CartItemResponse>>
    {
        public AddToCartRequest Request { get; set; } = new();
    }
}
