using Microsoft.AspNetCore.Mvc;
using Project.Data;
using Project.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Project.Filters;
using System;
using Microsoft.AspNetCore.Http;

namespace Project.Controllers
{
    public class AccommodationController : Controller
    {
        private readonly AppDbContext _context;

        public AccommodationController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult CreateAccommodation()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
                return RedirectToAction("Login", "Account");

            return View(new Accommodation());
        }

        [HttpPost]
        public IActionResult CreateAccommodation(Accommodation model)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
                return RedirectToAction("Login", "Account");

            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            model.CreatedAt = DateTime.Now;
            model.OwnerId = user.Id;
            _context.Accommodations.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Accommodations");
        }

        [RequireAdmin]
        public IActionResult AdminAccommodations()
        {
            var accommodations = _context.Accommodations.OrderByDescending(a => a.CreatedAt).ToList();
            return View(accommodations);
        }

        [RequireAdmin]
        public IActionResult EditAccommodation(int id)
        {
            var acc = _context.Accommodations.FirstOrDefault(a => a.Id == id);
            if (acc == null) return NotFound();
            return View(acc);
        }

        [HttpPost]
        [RequireAdmin]
        public IActionResult EditAccommodation(Accommodation model)
        {
            var acc = _context.Accommodations.FirstOrDefault(a => a.Id == model.Id);
            if (acc == null) return NotFound();

            acc.Title = model.Title;
            acc.Location = model.Location;
            acc.Address = model.Address;
            acc.Description = model.Description;
            acc.PricePerNight = model.PricePerNight;
            acc.Capacity = model.Capacity;
            acc.Bedrooms = model.Bedrooms;
            acc.Bathrooms = model.Bathrooms;
            acc.Type = model.Type;
            acc.ImageUrl = model.ImageUrl;

            _context.SaveChanges();
            return RedirectToAction("AdminAccommodations");
        }

        [RequireAdmin]
        public IActionResult DeleteAccommodation(int id)
        {
            var acc = _context.Accommodations.FirstOrDefault(a => a.Id == id);
            if (acc == null) return NotFound();
            _context.Accommodations.Remove(acc);
            _context.SaveChanges();
            return RedirectToAction("AdminAccommodations");
        }

        public IActionResult Accommodations(string? location, string? type, decimal? maxPrice, int? guests, string? checkIn, string? checkOut)
        {
            var query = _context.Accommodations.AsQueryable();

            if (!string.IsNullOrWhiteSpace(location))
            {
                location = location.Trim().ToLower();
                query = query.Where(a => a.Location.ToLower().Contains(location));
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                query = query.Where(a => a.Type.ToLower() == type.ToLower());
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(a => a.PricePerNight <= maxPrice.Value);
            }

            if (guests.HasValue)
            {
                query = query.Where(a => a.Capacity >= guests.Value);
            }

            DateTime requestedCheckIn = DateTime.MinValue, requestedCheckOut = DateTime.MinValue;
            var hasDates = DateTime.TryParse(checkIn, out requestedCheckIn) && DateTime.TryParse(checkOut, out requestedCheckOut) && requestedCheckIn < requestedCheckOut;

            if (hasDates)
            {
                // filter out accommodations that have overlapping bookings or unavailable periods in the requested period
                query = query.Where(a => !_context.Bookings.Any(b => b.AccommodationId == a.Id && b.CheckInDate < requestedCheckOut && b.CheckOutDate > requestedCheckIn)
                                         && !_context.UnavailablePeriods.Any(up => up.AccommodationId == a.Id && up.StartDate < requestedCheckOut && up.EndDate > requestedCheckIn));
            }

            ViewBag.CheckIn = checkIn;
            ViewBag.CheckOut = checkOut;
            ViewBag.Guests = guests;

            var stays = query
                .OrderByDescending(a => a.Rating)
                .ToList();

            return View(stays);
        }

        public IActionResult AccommodationDetails(int id)
        {
            var accommodation = _context.Accommodations.FirstOrDefault(a => a.Id == id);

            if (accommodation == null)
            {
                return NotFound();
            }

            var amenities = _context.AccommodationAmenities
                .Where(aa => aa.AccommodationId == id)
                .Join(
                    _context.Amenities,
                    aa => aa.AmenityId,
                    a => a.Id,
                    (aa, a) => a
                )
                .ToList();

            var images = _context.AccommodationImages
                .Where(i => i.AccommodationId == id)
                .OrderBy(i => i.DisplayOrder)
                .ToList();

            var viewModel = new AccommodationDetailsViewModel
            {
                Accommodation = accommodation,
                Amenities = amenities,
                Images = images
            };

            return View(viewModel);
        }
    }
}
