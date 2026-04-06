using Microsoft.AspNetCore.Mvc;
using Project.Data;
using Project.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Project.Filters;
using System;
using System.Collections.Generic;
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

        private User? GetCurrentUser()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return null;
            }

            return _context.Users.FirstOrDefault(u => u.Email == userEmail);
        }

        private Dictionary<int, decimal> GetAverageRatings(IEnumerable<int> accommodationIds)
        {
            var idList = accommodationIds.Distinct().ToList();
            if (idList.Count == 0)
            {
                return new Dictionary<int, decimal>();
            }

            return _context.Reviews
                .Where(r => idList.Contains(r.AccommodationId))
                .GroupBy(r => r.AccommodationId)
                .Select(g => new
                {
                    AccommodationId = g.Key,
                    AverageRating = g.Average(r => (decimal)r.Rating)
                })
                .AsEnumerable()
                .ToDictionary(x => x.AccommodationId, x => decimal.Round(x.AverageRating, 2));
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
                query = query.Where(a => !_context.Bookings.Any(b => b.AccommodationId == a.Id && b.CheckInDate < requestedCheckOut && b.CheckOutDate > requestedCheckIn)
                                         && !_context.UnavailablePeriods.Any(up => up.AccommodationId == a.Id && up.StartDate < requestedCheckOut && up.EndDate > requestedCheckIn));
            }

            ViewBag.CheckIn = checkIn;
            ViewBag.CheckOut = checkOut;
            ViewBag.Guests = guests;

            var stays = query.ToList();
            var ratingsByAccommodationId = GetAverageRatings(stays.Select(a => a.Id));

            foreach (var stay in stays)
            {
                if (ratingsByAccommodationId.TryGetValue(stay.Id, out var averageRating))
                {
                    stay.Rating = averageRating;
                }
            }

            stays = stays
                .OrderByDescending(a => a.Rating)
                .ThenByDescending(a => a.CreatedAt)
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

            var reviews = _context.Reviews
                .Include(r => r.User)
                .Where(r => r.AccommodationId == id)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            var ratingsByAccommodationId = GetAverageRatings(new[] { id });
            if (ratingsByAccommodationId.TryGetValue(id, out var averageRating))
            {
                accommodation.Rating = averageRating;
            }

            var currentUser = GetCurrentUser();
            var canLeaveReview = false;
            var hasReviewed = false;

            if (currentUser != null)
            {
                hasReviewed = reviews.Any(r => r.UserId == currentUser.Id);
                canLeaveReview = !hasReviewed && _context.Bookings.Any(b =>
                    b.UserId == currentUser.Id &&
                    b.AccommodationId == id &&
                    b.Status != "Cancelled" &&
                    b.CheckOutDate.Date <= DateTime.Today);
            }

            var viewModel = new AccommodationDetailsViewModel
            {
                Accommodation = accommodation,
                Amenities = amenities,
                Images = images,
                Reviews = reviews,
                CanLeaveReview = canLeaveReview,
                HasReviewed = hasReviewed
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddReview(int accommodationId, int rating, string comment)
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var accommodation = _context.Accommodations.FirstOrDefault(a => a.Id == accommodationId);
            if (accommodation == null)
            {
                return NotFound();
            }

            var hasStayed = _context.Bookings.Any(b =>
                b.UserId == currentUser.Id &&
                b.AccommodationId == accommodationId &&
                b.Status != "Cancelled" &&
                b.CheckOutDate.Date <= DateTime.Today);

            if (!hasStayed)
            {
                TempData["ReviewError"] = "You can leave a review after your stay.";
                return RedirectToAction("AccommodationDetails", new { id = accommodationId });
            }

            var hasReviewed = _context.Reviews.Any(r => r.UserId == currentUser.Id && r.AccommodationId == accommodationId);
            if (hasReviewed)
            {
                TempData["ReviewError"] = "You have already reviewed this accommodation.";
                return RedirectToAction("AccommodationDetails", new { id = accommodationId });
            }

            comment = (comment ?? string.Empty).Trim();
            if (rating < 1 || rating > 5)
            {
                TempData["ReviewError"] = "Please choose a rating from 1 to 5.";
                return RedirectToAction("AccommodationDetails", new { id = accommodationId });
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                TempData["ReviewError"] = "Please write a comment for your review.";
                return RedirectToAction("AccommodationDetails", new { id = accommodationId });
            }

            if (comment.Length > 1000)
            {
                TempData["ReviewError"] = "Your review comment must be 1000 characters or less.";
                return RedirectToAction("AccommodationDetails", new { id = accommodationId });
            }

            var review = new Review
            {
                UserId = currentUser.Id,
                AccommodationId = accommodationId,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.Now
            };

            _context.Reviews.Add(review);
            _context.SaveChanges();

            TempData["ReviewSuccess"] = "Thanks for sharing your review.";
            return RedirectToAction("AccommodationDetails", new { id = accommodationId });
        }
    }
}
