using System;
using System.Collections.Generic;

namespace Luqma.Domain.Models
{
    public class ExternalLogin
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Provider { get; set; } = string.Empty;
        public string ProviderUserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public ApplicationUser User { get; set; } = null!;
    }
}
