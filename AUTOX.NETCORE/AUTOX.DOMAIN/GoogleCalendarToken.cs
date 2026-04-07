using System;
using System.Collections.Generic;
using System.Text;

namespace AUTOX.DOMAIN
{
    public class GoogleCalendarToken
    {
        public int GoogleCalendarTokenId { get; set; }

        public string GoogleEmail { get; set; } = string.Empty;
        public string? AccessToken { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime? TokenExpiryUtc { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
