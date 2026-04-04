using Microsoft.AspNetCore.Mvc;
using Project.Data;
using Project.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Http;

namespace Project.Controllers
{
    public class BookingController : Controller
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult MyBookings()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            var bookings = _context.Bookings
                .Include(b => b.Accommodation)
                .Where(b => b.UserId == user.Id)
                .OrderByDescending(b => b.CreatedAt)
                .ToList();

            return View(bookings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Book(int id, string? checkIn, string? checkOut, int? guests)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return Json(new { success = false, loginRedirect = Url.Action("Login", "Account") });
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null)
            {
                HttpContext.Session.Clear();
                return Json(new { success = false, loginRedirect = Url.Action("Login", "Account") });
            }

            var accommodation = _context.Accommodations.FirstOrDefault(a => a.Id == id);
            if (accommodation == null)
            {
                return Json(new { success = false, message = "Accommodation not found." });
            }
            // validate dates
            if (string.IsNullOrEmpty(checkIn) || string.IsNullOrEmpty(checkOut) || !DateTime.TryParse(checkIn, out var requestedCheckIn) || !DateTime.TryParse(checkOut, out var requestedCheckOut))
            {
                return Json(new { success = false, message = "Please provide valid check-in and check-out dates." });
            }

            if (requestedCheckIn.Date >= requestedCheckOut.Date)
            {
                return Json(new { success = false, message = "Check-out must be after check-in." });
            }

            if (requestedCheckIn.Date < DateTime.Today)
            {
                return Json(new { success = false, message = "Check-in cannot be in the past." });
            }

            var numGuests = guests ?? 1;
            if (numGuests < 1 || numGuests > accommodation.Capacity)
            {
                return Json(new { success = false, message = $"Number of guests must be between 1 and {accommodation.Capacity}." });
            }

            // check availability: overlapping bookings or unavailable periods
            var hasOverlap = _context.Bookings.Any(b => b.AccommodationId == accommodation.Id && b.CheckInDate < requestedCheckOut && b.CheckOutDate > requestedCheckIn)
                             || _context.UnavailablePeriods.Any(up => up.AccommodationId == accommodation.Id && up.StartDate < requestedCheckOut && up.EndDate > requestedCheckIn);
            if (hasOverlap)
            {
                return Json(new { success = false, message = "The accommodation is not available for the selected dates." });
            }

            var nights = (requestedCheckOut.Date - requestedCheckIn.Date).Days;
            if (nights <= 0) nights = 1;

            var totalPrice = accommodation.PricePerNight * nights;

            var booking = new Booking
            {
                UserId = user.Id,
                AccommodationId = accommodation.Id,
                CheckInDate = requestedCheckIn.Date,
                CheckOutDate = requestedCheckOut.Date,
                Guests = numGuests,
                TotalPrice = totalPrice,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            return Json(new { success = true, redirect = Url.Action("MyBookings", "Booking") });
        }
    }
}
