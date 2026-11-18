namespace Luqma.Application.Features.Dtos.Requests.AccountRequests
{
    public class ConfirmEmailRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }
}
