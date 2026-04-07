using AUTOX.BLL.Interfaces;
using AUTOX.DAL;
using AUTOX.DOMAIN;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AUTOX.BLL
{
    public class BookiongServiceImpl : IBookiongService
    {
        private readonly IDatabaseService _databaseService;

        public BookiongServiceImpl(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<(bool, long)> UpdateBooking(Booking booking) {

            string jsonString = JsonConvert.SerializeObject(booking);
            DataTransactionManager dataTransactionManager = new DataTransactionManager(_databaseService.GetConnectionString());
            var (status, primaryKey) = await dataTransactionManager.BookingDataManager.UpdateDataReturnPrimaryKey("UpdateBooking", jsonString);
            return (status, primaryKey);
        }

        public async Task<Booking> GetBooking(long boolingId)
        {
            DataTransactionManager dataTransactionManager = new DataTransactionManager(_databaseService.GetConnectionString());
            var booling = await dataTransactionManager.BookingDataManager.RetrieveData("GetBooking", new SqlParameter[] {
                new SqlParameter("@boolingId", boolingId)
            });
            return booling.FirstOrDefault();
        }

        public async Task<IEnumerable<Booking>> GetAllBooking()
        {
            DataTransactionManager dataTransactionManager = new DataTransactionManager(_databaseService.GetConnectionString());
            var boolings = await dataTransactionManager.BookingDataManager.RetrieveData("GetBooking");
            return boolings;
        }

        public async Task<bool> CompleteBooking(long bookingId)
        {
            DataTransactionManager dataTransactionManager = new DataTransactionManager(_databaseService.GetConnectionString());
            var status = await dataTransactionManager.BookingDataManager.ExecuteProcedure("CompleteBooking", new SqlParameter[]
            {
                new SqlParameter("@bookingId", bookingId)
            });
            return status;
        }
    }
}
