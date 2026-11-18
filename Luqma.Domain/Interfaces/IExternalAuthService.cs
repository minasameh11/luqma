using Luqma.Domain.Models;

namespace Luqma.Domain.Interfaces
{
    public interface IExternalAuthService
    {
        Task<ExternalUserInfo?> ValidateFacebookTokenAsync(string accessToken);
        Task<ExternalUserInfo?> ValidateAppleTokenAsync(string identityToken);
    }

    public class ExternalUserInfo
    {
        public string ProviderUserId { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public bool EmailVerified { get; set; }
    }
}
