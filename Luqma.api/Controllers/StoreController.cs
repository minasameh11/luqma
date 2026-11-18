using Luqma.Application.Features.Commands.ImageCommands.UploadImageCommand;
using Luqma.Application.Features.Commands.StoreCommands.CreateStoreCommand;
using Luqma.Application.Features.Commands.StoreCommands.SetStoreActiveCommand;
using Luqma.Application.Features.Commands.StoreCommands.UpdateStoreCommand;
using Luqma.Application.Features.Dtos.Requests.CategoryRequests;
using Luqma.Application.Features.Dtos.Requests.StoreRequests;
using Luqma.Application.Features.Queries.StoreQueries.GetStoresQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luqma.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StoreController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(
            [FromQuery] Guid? mainCategoryId,
            [FromQuery] Guid? cuisineId,
            [FromQuery] bool? isOpenNow,
            [FromQuery] bool? isActive,
            [FromQuery] double? latitude,
            [FromQuery] double? longitude,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var query = new GetStoresQuery
            {
                MainCategoryId = mainCategoryId,
                CuisineId = cuisineId,
                IsOpenNow = isOpenNow,
                IsActive = isActive,
                Latitude = latitude,
                Longitude = longitude,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("UploadIcon")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadIcon([FromForm] UploadMainCategoryIconRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new UploadImageCommand
            {
                File = request.File,
                FolderName = UploadImageCommandHandler.StoresFolder
            }, cancellationToken);

            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] SaveStoreRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CreateStoreCommand { Request = request }, cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("Update/{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStoreRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new UpdateStoreCommand { Id = id, Request = request }, cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("SetActive/{id:guid}")]
        public async Task<IActionResult> SetActive(Guid id, [FromBody] SetStoreActiveRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new SetStoreActiveCommand { Id = id, IsActive = request.IsActive }, cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }
    }
}
