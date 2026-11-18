using Luqma.Application.Features.Commands.CategoryCommands.CreateMainCategoryCommand;
using Luqma.Application.Features.Commands.ImageCommands.UploadImageCommand;
using Luqma.Application.Features.Dtos.Requests.CategoryRequests;
using Luqma.Application.Features.Queries.CategoryQueries.GetMainCategoriesQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luqma.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MainCategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MainCategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMainCategoriesQuery(), cancellationToken);
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
                FolderName = UploadImageCommandHandler.MainCategoriesFolder
            }, cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] SaveMainCategoryRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CreateMainCategoryCommand { Request = request }, cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }
    }
}
