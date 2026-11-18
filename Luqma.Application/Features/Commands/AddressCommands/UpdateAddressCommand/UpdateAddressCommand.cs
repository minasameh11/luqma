using Luqma.Application.Features.Dtos.Requests.AddressRequests;
using Luqma.Application.Features.Dtos.Responses.AddressResponses;
using Luqma.Application.Wapper;
using MediatR;

namespace Luqma.Application.Features.Commands.AddressCommands.UpdateAddressCommand
{
    public class UpdateAddressCommand : IRequest<Response<AddressResponse>>
    {
        public Guid Id { get; set; }
        public SaveAddressRequest Request { get; set; } = new();
    }
}
