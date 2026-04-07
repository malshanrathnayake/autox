using AUTOX.DOMAIN;
using System;
using System.Collections.Generic;
using System.Text;

namespace AUTOX.BLL.Interfaces
{
    public interface IBookiongService
    {
        Task<(bool, long)> UpdateBooking(Booking booking);

        Task<Booking> GetBooking(long boolingId);
        Task<IEnumerable<Booking>> GetAllBooking();
        Task<bool> CompleteBooking(long bookingId);
    }
}
