using Luqma.Application.Features.Commands.ImageCommands.UploadImageCommand;
using Luqma.Application.Features.Commands.MenuCommands.CreateMenuCategoryCommand;
using Luqma.Application.Features.Commands.MenuCommands.SetMenuCategoryActiveCommand;
using Luqma.Application.Features.Commands.MenuCommands.UpdateMenuCategoryCommand;
using Luqma.Application.Features.Dtos.Requests.CategoryRequests;
using Luqma.Application.Features.Dtos.Requests.MenuRequests;
using Luqma.Application.Features.Queries.MenuQueries.GetMenuCategoriesQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luqma.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuCategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MenuCategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(
            [FromQuery] Guid storeId,
            [FromQuery] bool onlyActive = false,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetMenuCategoriesQuery { StoreId = storeId, OnlyActive = onlyActive }, cancellationToken);

            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "Admin,SuperAdmin,Owner")]
        [HttpPost("UploadIcon")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadIcon([FromForm] UploadMainCategoryIconRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new UploadImageCommand
            {
                File = request.File,
                FolderName = UploadImageCommandHandler.MenuCategoriesFolder
            }, cancellationToken);

            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "Admin,SuperAdmin,Owner")]
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] SaveMenuCategoryRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CreateMenuCategoryCommand { Request = request }, cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "Admin,SuperAdmin,Owner")]
        [HttpPut("Update/{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SaveMenuCategoryRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new UpdateMenuCategoryCommand { Id = id, Request = request }, cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "Admin,SuperAdmin,Owner")]
        [HttpPut("SetActive/{id:guid}")]
        public async Task<IActionResult> SetActive(Guid id, [FromBody] SetMenuActiveRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new SetMenuCategoryActiveCommand { Id = id, IsActive = request.IsActive }, cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }
    }
}
