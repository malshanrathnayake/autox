using System;
using System.Collections.Generic;
using System.Text;

namespace AUTOX.BLL.Interfaces
{
    public interface IGoogleCalendarService
    {
        Task<bool> CreateBookingEvent(long bookingId);
        Task<bool> MarkBookingCompleted(long bookingId);
    }
}
