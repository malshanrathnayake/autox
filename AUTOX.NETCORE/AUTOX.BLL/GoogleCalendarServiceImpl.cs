using AUTOX.BLL.Interfaces;
using AUTOX.DAL;
using AUTOX.DOMAIN;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2.Responses;
using Newtonsoft.Json;
using Google.Apis.Auth.OAuth2.Flows;
namespace AUTOX.BLL
{
    public class GoogleCalendarServiceImpl : IGoogleCalendarService
    {
        private readonly IDatabaseService _databaseService;
        private readonly IConfigurationService _configurationService;

        public GoogleCalendarServiceImpl(
            IDatabaseService databaseService,
            IConfigurationService configurationService)
        {
            _databaseService = databaseService;
            _configurationService = configurationService;
        }

        public async Task<bool> CreateBookingEvent(long bookingId)
        {
            try
            {
                using var dataTransactionManager = new DataTransactionManager(_databaseService.GetConnectionString());

                var bookingList = await dataTransactionManager.BookingDataManager
                    .RetrieveData("GetBooking", new[]
                    {
                        new Microsoft.Data.SqlClient.SqlParameter("@BookingId", bookingId)
                    });

                var booking = bookingList.FirstOrDefault();
                if (booking == null)
                    return false;

                var tokenList = await dataTransactionManager.GoogleCalendarTokenDataManager
                    .RetrieveData("GetGoogleCalendarToken");

                var token = tokenList.FirstOrDefault();
                if (token == null)
                    return false;

                var calendarService = await BuildCalendarService(token);

                var calendarEvent = new Event
                {
                    Summary = $"{booking.VehicleType} - {booking.CustomerName} - {booking.ContactNumber}",
                    Description =
                        $@"Customer Name: {booking.CustomerName}
                        Contact Number: {booking.ContactNumber}
                        Email: {booking.Email}
                        Vehicle Type: {booking.VehicleType}
                        Service Type: {booking.ServiceType}
                        Booking Date: {booking.BookingDate:yyyy-MM-dd}
                        Notes: {booking.Notes}
                        BookingId: {booking.BookingId}

                        Complete URL:
                        {_configurationService.GetAppBaseUrl()}/Booking/Complete/{booking.BookingId}",
                    Start = new EventDateTime
                    {
                        Date = booking.BookingDate.ToString("yyyy-MM-dd"),
                        TimeZone = "Asia/Colombo"
                    },
                    End = new EventDateTime
                    {
                        Date = booking.BookingDate.AddDays(1).ToString("yyyy-MM-dd"),
                        TimeZone = "Asia/Colombo"
                    },
                    ExtendedProperties = new Event.ExtendedPropertiesData
                    {
                        Private__ = new Dictionary<string, string>
                        {
                            { "BookingId", booking.BookingId.ToString() },
                            { "Status", booking.Status ?? "Pending" }
                        }
                    }
                };

                var insertRequest = calendarService.Events.Insert(calendarEvent, "primary");
                var createdEvent = await insertRequest.ExecuteAsync();

                if (createdEvent == null || string.IsNullOrWhiteSpace(createdEvent.Id))
                    return false;

                booking.GoogleEventId = createdEvent.Id;
                booking.Status = "CalendarCreated";
                booking.UpdatedAt = DateTime.Now;

                string bookingJson = JsonConvert.SerializeObject(booking);
                var (status, _) = await dataTransactionManager.BookingDataManager
                    .UpdateDataReturnPrimaryKey("UpdateBooking", bookingJson);

                return status;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> MarkBookingCompleted(long bookingId)
        {
            try
            {
                using var dataTransactionManager = new DataTransactionManager(_databaseService.GetConnectionString());

                var bookingList = await dataTransactionManager.BookingDataManager
                    .RetrieveData("GetBooking", new[]
                    {
                        new Microsoft.Data.SqlClient.SqlParameter("@BookingId", bookingId)
                    });

                var booking = bookingList.FirstOrDefault();
                if (booking == null)
                    return false;

                booking.Status = "Completed";
                booking.UpdatedAt = DateTime.Now;

                if (!string.IsNullOrWhiteSpace(booking.GoogleEventId))
                {
                    var tokenList = await dataTransactionManager.GoogleCalendarTokenDataManager
                        .RetrieveData("GetGoogleCalendarToken");

                    var token = tokenList.FirstOrDefault();
                    if (token != null)
                    {
                        var calendarService = await BuildCalendarService(token);

                        var existingEvent = await calendarService.Events.Get("primary", booking.GoogleEventId).ExecuteAsync();
                        if (existingEvent != null)
                        {
                            existingEvent.Summary = $"[COMPLETED] {booking.VehicleType} - {booking.CustomerName} - {booking.ContactNumber}";
                            existingEvent.Description =
                                                        $@"Customer Name: {booking.CustomerName}
                                                        Contact Number: {booking.ContactNumber}
                                                        Email: {booking.Email}
                                                        Vehicle Type: {booking.VehicleType}
                                                        Service Type: {booking.ServiceType}
                                                        Booking Date: {booking.BookingDate:yyyy-MM-dd}
                                                        Notes: {booking.Notes}
                                                        BookingId: {booking.BookingId}
                                                        Status: Completed";

                            existingEvent.ExtendedProperties ??= new Event.ExtendedPropertiesData();
                            existingEvent.ExtendedProperties.Private__ ??= new Dictionary<string, string>();
                            existingEvent.ExtendedProperties.Private__["BookingId"] = booking.BookingId.ToString();
                            existingEvent.ExtendedProperties.Private__["Status"] = "Completed";

                            await calendarService.Events.Update(existingEvent, "primary", booking.GoogleEventId).ExecuteAsync();
                        }
                    }
                }

                string bookingJson = JsonConvert.SerializeObject(booking);
                var (status, _) = await dataTransactionManager.BookingDataManager
                    .UpdateDataReturnPrimaryKey("UpdateBooking", bookingJson);

                return status;
            }
            catch
            {
                return false;
            }
        }

        private async Task<CalendarService> BuildCalendarService(GoogleCalendarToken token)
        {
            var clientId = _configurationService.GetGoogleClientId();
            var clientSecret = _configurationService.GetGoogleClientSecret();

            var tokenResponse = new TokenResponse
            {
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken
            };

            var secrets = new ClientSecrets
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            };

            var credential = new UserCredential(
                new GoogleAuthorizationCodeFlow(
                    new GoogleAuthorizationCodeFlow.Initializer
                    {
                        ClientSecrets = secrets,
                        Scopes = new[] { CalendarService.Scope.Calendar }
                    }),
                token.GoogleEmail,
                tokenResponse);

            await credential.RefreshTokenAsync(CancellationToken.None);

            return new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "AUTOX Booking Calendar"
            });
        }
    }
}
