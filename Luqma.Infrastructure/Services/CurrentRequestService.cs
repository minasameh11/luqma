using Luqma.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Luqma.Infrastructure.Services
{
    public class CurrentRequestService : ICurrentRequestService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly string? _configuredBaseUrl;

        public CurrentRequestService(IHttpContextAccessor contextAccessor, IConfiguration configuration)
        {
            _contextAccessor = contextAccessor;
            _configuredBaseUrl = configuration["App:BaseUrl"];
        }

        public string BaseUrl
        {
            get
            {
                // Explicit configuration wins (needed when the app runs behind a proxy with a different public URL)
                if (!string.IsNullOrWhiteSpace(_configuredBaseUrl))
                    return _configuredBaseUrl.TrimEnd('/');

                var request = _contextAccessor.HttpContext?.Request;
                if (request == null || !request.Host.HasValue)
                    return string.Empty;

                return $"{request.Scheme}://{request.Host}{request.PathBase}".TrimEnd('/');
            }
        }
    }
}
