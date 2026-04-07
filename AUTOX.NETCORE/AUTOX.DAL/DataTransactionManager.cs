using AUTOX.DOMAIN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUTOX.DAL
{
    public class DataTransactionManager : IDisposable
    {
        private string _connectionString;

        public DataTransactionManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        private DataManager<BookingRequestViewModel> _bookingRequestViewModelDatamanager;
        public DataManager<BookingRequestViewModel> BookingRequestViewModelDataManager
        {
            get
            {
                if (this._bookingRequestViewModelDatamanager == null)
                {
                    this._bookingRequestViewModelDatamanager = new DataManager<BookingRequestViewModel>(_connectionString);
                }

                return this._bookingRequestViewModelDatamanager;
            }
        }

        private DataManager<Booking> _bookingDatamanager;
        public DataManager<Booking> BookingDataManager
        {
            get
            {
                if (this._bookingDatamanager == null)
                {
                    this._bookingDatamanager = new DataManager<Booking>(_connectionString);
                }

                return this._bookingDatamanager;
            }
        }

        private DataManager<GoogleCalendarToken> _googleCalendarTokenDatamanager;
        public DataManager<GoogleCalendarToken> GoogleCalendarTokenDataManager
        {
            get
            {
                if (this._googleCalendarTokenDatamanager == null)
                {
                    this._googleCalendarTokenDatamanager = new DataManager<GoogleCalendarToken>(_connectionString);
                }

                return this._googleCalendarTokenDatamanager;
            }
        }

        private bool _disposed = false;
        protected void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {

                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
