using Luqma.Domain.Enums;

namespace Luqma.Application.Features.Dtos.Requests.AccountRequests
{
    public class RegisterRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;
    }
}
