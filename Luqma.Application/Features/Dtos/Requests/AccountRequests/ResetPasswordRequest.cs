namespace Luqma.Application.Features.Dtos.Requests.AccountRequests
{
    public class ResetPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
