using Luqma.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Luqma.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public CurrentUserService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        public string UserId => _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        public string UserName => _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;

        public bool IsInRole(string role) => _contextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
    }
}
