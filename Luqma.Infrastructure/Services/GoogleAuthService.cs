using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using Luqma.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Luqma.Infrastructure.Services
{
    public class GoogleAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GoogleAuthService> _logger;

        private const string GoogleIssuer = "https://accounts.google.com";
        private const string GoogleKeysUrl = "https://www.googleapis.com/oauth2/v3/certs";

        public GoogleAuthService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<GoogleAuthService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ExternalUserInfo?> ValidateGoogleTokenAsync(string idToken)
        {
            try
            {
                var clientId = _configuration["Google:ClientId"];
                var keys = await GetGooglePublicKeysAsync();

                var handler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = GoogleIssuer,
                    ValidateAudience = true,
                    ValidAudience = clientId,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                    {
                        return keys.Where(k => k.KeyId == kid);
                    }
                };

                var principal = handler.ValidateToken(idToken, validationParameters, out var validatedToken);

                var sub = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(sub))
                {
                    _logger.LogWarning("Google token missing 'sub' claim");
                    return null;
                }

                var email = principal.FindFirst(ClaimTypes.Email)?.Value;
                var name = principal.FindFirst(ClaimTypes.Name)?.Value;
                var emailVerified = principal.FindFirst("email_verified")?.Value;
                bool isEmailVerified = string.Equals(emailVerified, "true", StringComparison.OrdinalIgnoreCase);

                return new ExternalUserInfo
                {
                    ProviderUserId = sub,
                    Email = email,
                    FullName = name,
                    EmailVerified = isEmailVerified
                };
            }
            catch (SecurityTokenExpiredException)
            {
                _logger.LogWarning("Google identity token has expired");
                return null;
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning(ex, "Google token validation failed: {Message}", ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating Google token");
                return null;
            }
        }

        private async Task<List<RsaSecurityKey>> GetGooglePublicKeysAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<JsonElement>(GoogleKeysUrl);
            var keys = new List<RsaSecurityKey>();

            foreach (var keyElement in response.GetProperty("keys").EnumerateArray())
            {
                var kid = keyElement.GetProperty("kid").GetString();
                var n = keyElement.GetProperty("n").GetString();
                var e = keyElement.GetProperty("e").GetString();

                var rsa = RSA.Create();
                rsa.ImportParameters(new RSAParameters
                {
                    Modulus = Base64UrlEncoder.DecodeBytes(n!),
                    Exponent = Base64UrlEncoder.DecodeBytes(e!)
                });

                var securityKey = new RsaSecurityKey(rsa) { KeyId = kid };
                keys.Add(securityKey);
            }

            return keys;
        }
    }
}
