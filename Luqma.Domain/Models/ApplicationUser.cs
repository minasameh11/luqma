using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luqma.Domain.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? FullName { get; set; }
        public DateTime? TokenExpiresAt { get; set; }
        public string? JwtToken { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public string? Address { get; set; }
        public bool? IsArchived { get; set; } = false;
        public string? VerificationCode { get; set; }
        public DateTime? VerificationCodeExpiresAt { get; set; }
    }
}
