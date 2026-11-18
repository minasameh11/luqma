using Luqma.Application.Features.Commands.CartCommands.AddToCartCommand;
using Luqma.Application.Features.Commands.CartCommands.ClearCartCommand;
using Luqma.Application.Features.Commands.CartCommands.UpdateToCartCommand;
using Luqma.Application.Features.Dtos.Requests.CartRequests;
using Luqma.Application.Features.Queries.CartQueries.GetCartCountQuery;
using Luqma.Application.Features.Queries.CartQueries.GetMyCartQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luqma.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CartController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Adds a new line to the cart only; an existing line is never modified here
        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new AddToCartCommand { Request = request }, cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        // Changes the quantity of an existing line; Quantity 0 removes the line
        [HttpPut("UpdateToCart")]
        public async Task<IActionResult> UpdateToCart([FromBody] UpdateCartRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new UpdateToCartCommand { Request = request }, cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetMyCart")]
        public async Task<IActionResult> GetMyCart(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMyCartQuery(), cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        // Lightweight totals for the cart badge
        [HttpGet("GetCount")]
        public async Task<IActionResult> GetCount(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCartCountQuery(), cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("Clear")]
        public async Task<IActionResult> Clear(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ClearCartCommand(), cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }
    }
}
