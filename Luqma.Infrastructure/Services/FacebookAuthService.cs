using System.Net.Http.Json;
using System.Text.Json;
using Luqma.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Luqma.Infrastructure.Services
{
    public class FacebookAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FacebookAuthService> _logger;

        public FacebookAuthService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<FacebookAuthService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ExternalUserInfo?> ValidateFacebookTokenAsync(string accessToken)
        {
            try
            {
                var appId = _configuration["Facebook:AppId"];
                var appSecret = _configuration["Facebook:AppSecret"];

                var appAccessToken = $"{appId}|{appSecret}";

                var debugUrl = $"https://graph.facebook.com/debug_token?input_token={Uri.EscapeDataString(accessToken)}&access_token={Uri.EscapeDataString(appAccessToken)}";

                var debugResponse = await _httpClient.GetFromJsonAsync<JsonElement>(debugUrl);

                if (!debugResponse.TryGetProperty("data", out var data))
                {
                    _logger.LogWarning("Facebook token validation failed: no data in response");
                    return null;
                }

                var isValid = data.GetProperty("is_valid").GetBoolean();
                if (!isValid)
                {
                    _logger.LogWarning("Facebook token is not valid");
                    return null;
                }

                var appIdFromToken = data.GetProperty("app_id").GetString();
                if (appIdFromToken != appId)
                {
                    _logger.LogWarning("Facebook token app_id mismatch: expected {Expected}, got {Actual}", appId, appIdFromToken);
                    return null;
                }

                var facebookUserId = data.GetProperty("user_id").GetString();

                var userUrl = $"https://graph.facebook.com/me?fields=id,name,email&access_token={Uri.EscapeDataString(accessToken)}";
                var userResponse = await _httpClient.GetFromJsonAsync<JsonElement>(userUrl);

                var email = userResponse.TryGetProperty("email", out var emailProp) ? emailProp.GetString() : null;
                var name = userResponse.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : null;

                return new ExternalUserInfo
                {
                    ProviderUserId = facebookUserId!,
                    Email = email,
                    FullName = name,
                    EmailVerified = !string.IsNullOrEmpty(email)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating Facebook token");
                return null;
            }
        }
    }
}
