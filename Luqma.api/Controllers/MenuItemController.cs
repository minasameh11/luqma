using Luqma.Application.Features.Commands.ImageCommands.UploadImageCommand;
using Luqma.Application.Features.Commands.MenuCommands.CreateMenuItemCommand;
using Luqma.Application.Features.Commands.MenuCommands.SetMenuItemActiveCommand;
using Luqma.Application.Features.Commands.MenuCommands.UpdateMenuItemCommand;
using Luqma.Application.Features.Dtos.Requests.CategoryRequests;
using Luqma.Application.Features.Dtos.Requests.MenuRequests;
using Luqma.Application.Features.Queries.MenuQueries.GetMenuItemsQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luqma.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuItemController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MenuItemController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(
            [FromQuery] Guid? storeId,
            [FromQuery] Guid? menuCategoryId,
            [FromQuery] string? search,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] bool? isActive,
            [FromQuery] bool? onSale,
            [FromQuery] string? sortBy,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var query = new GetMenuItemsQuery
            {
                StoreId = storeId,
                MenuCategoryId = menuCategoryId,
                Search = search,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                IsActive = isActive,
                OnSale = onSale,
                SortBy = sortBy,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query, cancellationToken);
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
                FolderName = UploadImageCommandHandler.MenuItemsFolder
            }, cancellationToken);

            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "Admin,SuperAdmin,Owner")]
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] SaveMenuItemRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CreateMenuItemCommand { Request = request }, cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "Admin,SuperAdmin,Owner")]
        [HttpPut("Update/{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SaveMenuItemRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new UpdateMenuItemCommand { Id = id, Request = request }, cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "Admin,SuperAdmin,Owner")]
        [HttpPut("SetActive/{id:guid}")]
        public async Task<IActionResult> SetActive(Guid id, [FromBody] SetMenuActiveRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new SetMenuItemActiveCommand { Id = id, IsActive = request.IsActive }, cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }
    }
}
