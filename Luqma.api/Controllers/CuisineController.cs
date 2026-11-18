using Luqma.Application.Features.Commands.CuisineCommands.CreateCuisineCommand;
using Luqma.Application.Features.Commands.CuisineCommands.SetCuisineActiveCommand;
using Luqma.Application.Features.Commands.CuisineCommands.UpdateCuisineCommand;
using Luqma.Application.Features.Dtos.Requests.CuisineRequests;
using Luqma.Application.Features.Queries.CuisineQueries.GetCuisinesQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luqma.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CuisineController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CuisineController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] bool onlyActive = false, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetCuisinesQuery { OnlyActive = onlyActive }, cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] SaveCuisineRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CreateCuisineCommand { Request = request }, cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("Update/{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SaveCuisineRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new UpdateCuisineCommand { Id = id, Request = request }, cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("SetActive/{id:guid}")]
        public async Task<IActionResult> SetActive(Guid id, [FromBody] SetCuisineActiveRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new SetCuisineActiveCommand { Id = id, IsActive = request.IsActive }, cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }
    }
}
