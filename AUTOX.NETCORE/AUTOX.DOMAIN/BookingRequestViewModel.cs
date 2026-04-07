using System;
using System.Collections.Generic;
using System.Text;

namespace AUTOX.DOMAIN
{
    public class BookingRequestViewModel
    {
        public long BookingId { get; set; }

        public string CustomerName { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string? Email { get; set; }

        public string VehicleType { get; set; } = string.Empty;
        public string ServiceType { get; set; } = string.Empty;

        public DateTime BookingDate { get; set; }

        public string? Notes { get; set; }
    }
}
