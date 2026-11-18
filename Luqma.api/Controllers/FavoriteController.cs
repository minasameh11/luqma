using Luqma.Application.Features.Commands.FavoriteCommands.AddFavoriteCommand;
using Luqma.Application.Features.Commands.FavoriteCommands.RemoveFavoriteCommand;
using Luqma.Application.Features.Dtos.Requests.FavoriteRequests;
using Luqma.Application.Features.Queries.FavoriteQueries.GetFavoritesCountQuery;
using Luqma.Application.Features.Queries.FavoriteQueries.GetMyFavoritesQuery;
using Luqma.Application.Features.Queries.FavoriteQueries.IsFavoriteQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luqma.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FavoriteController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FavoriteController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("AddToFavorites")]
        public async Task<IActionResult> AddToFavorites([FromBody] AddFavoriteRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new AddFavoriteCommand { MenuItemId = request.MenuItemId }, cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("Remove/{menuItemId:guid}")]
        public async Task<IActionResult> Remove(Guid menuItemId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RemoveFavoriteCommand { MenuItemId = menuItemId }, cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetMyFavorites")]
        public async Task<IActionResult> GetMyFavorites(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetMyFavoritesQuery { PageNumber = pageNumber, PageSize = pageSize }, cancellationToken);

            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        // Used by the UI to mark the heart state of a single item
        [HttpGet("IsFavorite/{menuItemId:guid}")]
        public async Task<IActionResult> IsFavorite(Guid menuItemId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new IsFavoriteQuery { MenuItemId = menuItemId }, cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetCount")]
        public async Task<IActionResult> GetCount(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetFavoritesCountQuery(), cancellationToken);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }
    }
}
