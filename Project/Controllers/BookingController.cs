using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.Services;

namespace Project.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IUserService _userService;

        public BookingController(IBookingService bookingService, IUserService userService)
        {
            _bookingService = bookingService;
            _userService = userService;
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
    }
}
