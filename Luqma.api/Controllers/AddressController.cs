using Luqma.Application.Features.Commands.AddressCommands.CreateAddressCommand;
using Luqma.Application.Features.Commands.AddressCommands.DeleteAddressCommand;
using Luqma.Application.Features.Commands.AddressCommands.SetDefaultAddressCommand;
using Luqma.Application.Features.Commands.AddressCommands.UpdateAddressCommand;
using Luqma.Application.Features.Dtos.Requests.AddressRequests;
using Luqma.Application.Features.Queries.AddressQueries.GetAddressByIdQuery;
using Luqma.Application.Features.Queries.AddressQueries.GetMyAddressesQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luqma.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AddressController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AddressController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetMyAddresses")]
        public async Task<IActionResult> GetMyAddresses(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMyAddressesQuery(), cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetById/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAddressByIdQuery { Id = id }, cancellationToken);
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] SaveAddressRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CreateAddressCommand { Request = request }, cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [HttpPut("Update/{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SaveAddressRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new UpdateAddressCommand { Id = id, Request = request }, cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [HttpPost("SetDefault/{id:guid}")]
        public async Task<IActionResult> SetDefault(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new SetDefaultAddressCommand { Id = id }, cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("Delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteAddressCommand { Id = id }, cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }
    }
}
