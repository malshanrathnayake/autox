using AUTOX.BLL.Interfaces;
using AUTOX.DOMAIN;
using Microsoft.AspNetCore.Mvc;

namespace AUTOX.WEB.Controllers
{
    public class BookingController : Controller
    {
        readonly IBookiongService _bookiongService;
        readonly IGoogleCalendarService _googleCalendarService;

        public BookingController(IBookiongService bookiongService, IGoogleCalendarService googleCalendarService)
        {
            _bookiongService = bookiongService;
            _googleCalendarService = googleCalendarService;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookingRequestViewModel bookingViewModel)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");

            Booking booking = new Booking()
            {
                CustomerName = bookingViewModel.CustomerName,
                ContactNumber = bookingViewModel.ContactNumber,
                Email = bookingViewModel.Email,
                VehicleType = bookingViewModel.VehicleType,
                ServiceType = bookingViewModel.ServiceType,
                BookingDate = bookingViewModel.BookingDate,
                Notes = bookingViewModel.Notes,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,

            };

            var (saved, bookingId) = await _bookiongService.UpdateBooking(booking);

            if (!saved)
            {
                TempData["Error"] = "Booking could not be saved.";
                return RedirectToAction("Index");
            }

            // 2. create calendar event
            var calendarCreated = await _googleCalendarService.CreateBookingEvent(bookingId);

            if (calendarCreated)
            {
                TempData["Success"] = "Booking saved successfully.";
            }
            else
            {
                TempData["Error"] = "Booking saved, but calendar sync failed.";
            }

            return RedirectToAction("Success", "Booking", new
            {
                title = "Booking Successful",
                message = "Your booking has been created successfully.",
                buttonText = "Book Another",
                redirectUrl = Url.Action("Index", "Booking")
            });
        }

        [HttpGet("Booking/Complete/{bookingId}")]
        public async Task<IActionResult> Complete(long bookingId)
        {

            var status = await _bookiongService.CompleteBooking(bookingId);
            var calendarCreated = await _googleCalendarService.MarkBookingCompleted(bookingId);

            if (status)
            {
                TempData["Success"] = "Completed";
            }
            else
            {
                TempData["Error"] = "Complete Failed";
            }

            return RedirectToAction("Success", "Booking", new
            {
                title = "Job Completed",
                message = "Your calendar event has been completed successfully.",
                buttonText = "Check Calendar",
                redirectUrl = "https://calendar.google.com/calendar"
            });
        }

        [HttpGet]
        public IActionResult Success(string title, string message, string buttonText, string redirectUrl)
        {
            var model = new StatusPageViewModel
            {
                Title = title,
                Message = message,
                ButtonText = buttonText,
                RedirectUrl = redirectUrl
            };

            TempData["Success"] = message;

            return View(model);
        }
    }
}

