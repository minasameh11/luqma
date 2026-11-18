using Luqma.Application.Features.Dtos.Requests.AddressRequests;
using Luqma.Application.Features.Dtos.Responses.AddressResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.AddressCommands.CreateAddressCommand
{
    public class CreateAddressCommand : IRequest<Response<AddressResponse>>
    {
        public SaveAddressRequest Request { get; set; } = new();
    }
}
