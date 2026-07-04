using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.Services;

namespace Project.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IUserService _userService;
        
        
        private readonly IWebHostEnvironment _env;

        public BookingController(
            IBookingService bookingService,
            IUserService userService,  
            IWebHostEnvironment env)
        {
            _bookingService = bookingService;
            _userService = userService;
            _env = env;
        }

        public IActionResult MyBookings()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
                return RedirectToAction("Login", "Account");

            var user = _userService.GetByEmail(userEmail);
            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            return View(_bookingService.GetForUser(user.Id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelBooking(int id)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
                return RedirectToAction("Login", "Account");

            var user = _userService.GetByEmail(userEmail);
            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            var result = _bookingService.CancelForUser(id, user.Id);
            if (result.Success)
            {
                TempData["BookingSuccess"] = result.Message;
            }
            else
            {
                TempData["BookingError"] = result.Message;
            }

            return RedirectToAction(nameof(MyBookings));
        }

        [HttpGet]
        public IActionResult Checkout(int id, string? checkIn, string? checkOut, int? guests)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                var returnUrl = Url.Action(nameof(Checkout), new { id, checkIn, checkOut, guests });
                return RedirectToAction("Login", "Account", new { returnUrl });
            }

            var user = _userService.GetByEmail(userEmail);
            if (user == null)
            {
                HttpContext.Session.Clear();
                var returnUrl = Url.Action(nameof(Checkout), new { id, checkIn, checkOut, guests });
                return RedirectToAction("Login", "Account", new { returnUrl });
            }

            var preview = _bookingService.GetCheckoutPreview(id, checkIn, checkOut, guests);
            if (preview == null)
            {
                return NotFound();
            }

            preview.ReturnUrl = Url.Action(nameof(ConfirmCheckout), new { id, checkIn, checkOut, guests });
            return View(preview);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmCheckout(int id, string? checkIn, string? checkOut, int? guests)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action(nameof(Checkout), new { id, checkIn, checkOut, guests }) });

            var user = _userService.GetByEmail(userEmail);
            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action(nameof(Checkout), new { id, checkIn, checkOut, guests }) });
            }

            var result = _bookingService.Book(id, user.Id, checkIn, checkOut, guests);
            if (!result.Success)
            {
                var preview = _bookingService.GetCheckoutPreview(id, checkIn, checkOut, guests);
                if (preview == null)
                {
                    return NotFound();
                }

                preview.ErrorMessage = result.Message;
                preview.ReturnUrl = Url.Action(nameof(Checkout), new { id, checkIn, checkOut, guests });
                return View("Checkout", preview);
            }

            return RedirectToAction("MyBookings", "Booking");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Book(int id, string? checkIn, string? checkOut, int? guests)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
                return Json(new { success = false, loginRedirect = Url.Action("Login", "Account") });

            var user = _userService.GetByEmail(userEmail);
            if (user == null)
            {
                HttpContext.Session.Clear();
                return Json(new { success = false, loginRedirect = Url.Action("Login", "Account") });
            }

            var result = _bookingService.Book(id, user.Id, checkIn, checkOut, guests);
            if (!result.Success)
                return Json(new { success = false, message = result.Message });

            return Json(new { success = true, redirect = Url.Action("MyBookings", "Booking") });
        }

        // Serves the confirmation PDF for a booking owned by the logged-in user.
        public IActionResult DownloadPdf(int id)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
                return RedirectToAction("Login", "Account");

            var user = _userService.GetByEmail(userEmail);
            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            var booking = _bookingService.GetBookingForDownload(id);
            if (booking == null || booking.UserId != user.Id || string.IsNullOrEmpty(booking.PdfPath))
            return NotFound();

            var physicalPath = Path.Combine(
                _env.WebRootPath,
                booking.PdfPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (!System.IO.File.Exists(physicalPath))
                return NotFound();

            return PhysicalFile(physicalPath, "application/pdf", $"booking-{booking.Id:D6}.pdf");
        }
    }
}
