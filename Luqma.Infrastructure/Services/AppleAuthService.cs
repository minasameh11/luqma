using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Luqma.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Luqma.Infrastructure.Services
{
    public class AppleAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AppleAuthService> _logger;

        private const string AppleIssuer = "https://appleid.apple.com";
        private const string AppleKeysUrl = "https://appleid.apple.com/auth/keys";

        public AppleAuthService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<AppleAuthService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ExternalUserInfo?> ValidateAppleTokenAsync(string identityToken)
        {
            try
            {
                var clientId = _configuration["Apple:ClientId"];
                var keys = await GetApplePublicKeysAsync();

                var handler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = AppleIssuer,
                    ValidateAudience = true,
                    ValidAudience = clientId,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                    {
                        return keys.Where(k => k.KeyId == kid);
                    }
                };

                var principal = handler.ValidateToken(identityToken, validationParameters, out var validatedToken);

                var sub = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(sub))
                {
                    _logger.LogWarning("Apple token missing 'sub' claim");
                    return null;
                }

                var email = principal.FindFirst(ClaimTypes.Email)?.Value;
                var emailVerified = principal.FindFirst("email_verified")?.Value;
                bool isEmailVerified = string.Equals(emailVerified, "true", StringComparison.OrdinalIgnoreCase);

                string? fullName = null;
                if (principal.FindFirst(ClaimTypes.GivenName) is Claim givenName &&
                    principal.FindFirst(ClaimTypes.Surname) is Claim surname)
                {
                    fullName = $"{givenName.Value} {surname.Value}".Trim();
                }

                return new ExternalUserInfo
                {
                    ProviderUserId = sub,
                    Email = email,
                    FullName = fullName,
                    EmailVerified = isEmailVerified
                };
            }
            catch (SecurityTokenExpiredException)
            {
                _logger.LogWarning("Apple identity token has expired");
                return null;
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning(ex, "Apple token validation failed: {Message}", ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating Apple token");
                return null;
            }
        }

        private async Task<List<RsaSecurityKey>> GetApplePublicKeysAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<JsonElement>(AppleKeysUrl);
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
