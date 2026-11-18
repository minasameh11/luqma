using Luqma.Application.Features.Commands.AccountCommands.ArchiveUserCommand;
using Luqma.Application.Features.Commands.AccountCommands.ConfirmEmailCommand;
using Luqma.Application.Features.Commands.AccountCommands.ForgotPasswordCommand;
using Luqma.Application.Features.Commands.AccountCommands.LoginCommand;
using Luqma.Application.Features.Commands.AccountCommands.LoginWithAppleCommand;
using Luqma.Application.Features.Commands.AccountCommands.LoginWithFacebookCommand;
using Luqma.Application.Features.Commands.AccountCommands.LoginWithGoogleCommand;
using Luqma.Application.Features.Commands.AccountCommands.LogoutCommand;
using Luqma.Application.Features.Commands.AccountCommands.RegisterCommand;
using Luqma.Application.Features.Commands.AccountCommands.ResendOtpCommand;
using Luqma.Application.Features.Commands.AccountCommands.ResetPasswordCommand;
using Luqma.Application.Features.Commands.AccountCommands.UnarchiveUserCommand;
using Luqma.Application.Features.Commands.AccountCommands.UpdateProfileCommand;
using Luqma.Application.Features.Dtos.Requests.AccountRequests;
using Luqma.Application.Features.Queries.AccountQueries.GetAllUsersQuery;
using Luqma.Application.Features.Queries.AccountQueries.GetProfileQuery;
using Luqma.Application.Features.Queries.AccountQueries.GetUserByIdQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luqma.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var command = new RegisterCommand { RegisterRequest = request };
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var command = new LoginCommand { LoginRequest = request };
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
                return Unauthorized(result);

            return Ok(result);
        }

        [HttpPost("LoginWithFacebook")]
        public async Task<IActionResult> LoginWithFacebook([FromBody] FacebookLoginRequest request)
        {
            var command = new LoginWithFacebookCommand { FacebookLoginRequest = request };
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("LoginWithApple")]
        public async Task<IActionResult> LoginWithApple([FromBody] AppleLoginRequest request)
        {
            var command = new LoginWithAppleCommand { AppleLoginRequest = request };
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("LoginWithGoogle")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] GoogleLoginRequest request)
        {
            var command = new LoginWithGoogleCommand { GoogleLoginRequest = request };
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await _mediator.Send(new LogoutCommand());
            return Ok(result);
        }

        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
        {
            var command = new ConfirmEmailCommand { ConfirmEmailRequest = request };
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("ResendOtp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequest request)
        {
            var command = new ResendOtpCommand { ResendOtpRequest = request };
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var command = new ForgotPasswordCommand { ForgotPasswordRequest = request };
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var command = new ResetPasswordCommand { ResetPasswordRequest = request };
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _mediator.Send(new GetAllUsersQuery());
            return Ok(result);
        }

        [Authorize]
        [HttpGet("GetUserById/{id:guid}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery { Id = id });
            return Ok(result);
        }

        [Authorize]
        [HttpGet("GetProfile")]
        public async Task<IActionResult> GetProfile()
        {
            var result = await _mediator.Send(new GetProfileQuery());
            return Ok(result);
        }

        [Authorize]
        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var command = new UpdateProfileCommand { UpdateProfileRequest = request };
            var result = await _mediator.Send(command);

            if (!result.IsSucceeded)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("ArchiveUser/{id:guid}")]
        public async Task<IActionResult> ArchiveUser(Guid id)
        {
            var result = await _mediator.Send(new ArchiveUserCommand { Id = id });

            if (!result.IsSucceeded)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("UnarchiveUser/{id:guid}")]
        public async Task<IActionResult> UnarchiveUser(Guid id)
        {
            var result = await _mediator.Send(new UnarchiveUserCommand { Id = id });

            if (!result.IsSucceeded)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
