using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.Filters;
using Project.Models;
using Project.Services;

namespace Project.Controllers
{
    public class AccommodationController : Controller
    {
        private readonly IAccommodationService _accommodationService;
        private readonly IUserService _userService;

        public AccommodationController(IAccommodationService accommodationService, IUserService userService)
        {
            _accommodationService = accommodationService;
            _userService = userService;
        }

        private User? GetCurrentUser()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            return string.IsNullOrEmpty(userEmail) ? null : _userService.GetByEmail(userEmail);
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

            var user = _userService.GetByEmail(userEmail);
            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            _accommodationService.Create(model, user.Id);
            return RedirectToAction("Accommodations");
        }

        [RequireAdmin]
        public IActionResult AdminAccommodations()
        {
            return View(_accommodationService.GetAllForAdmin());
        }

        [RequireAdmin]
        public IActionResult EditAccommodation(int id)
        {
            var acc = _accommodationService.GetById(id);
            return acc == null ? NotFound() : View(acc);
        }

        [HttpPost]
        [RequireAdmin]
        public IActionResult EditAccommodation(Accommodation model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                _accommodationService.Update(model);
                return RedirectToAction("AdminAccommodations");
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        [RequireAdmin]
        public IActionResult DeleteAccommodation(int id)
        {
            try
            {
                _accommodationService.Delete(id);
                return RedirectToAction("AdminAccommodations");
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        public IActionResult Accommodations(string? location, string? type, decimal? maxPrice, int? guests, string? checkIn, string? checkOut)
        {
            ViewBag.CheckIn = checkIn;
            ViewBag.CheckOut = checkOut;
            ViewBag.Guests = guests;
            return View(_accommodationService.Search(location, type, maxPrice, guests, checkIn, checkOut));
        }

        public IActionResult AccommodationDetails(int id)
        {
            var currentUser = GetCurrentUser();
            var viewModel = _accommodationService.GetDetails(id, currentUser?.Id);
            return viewModel == null ? NotFound() : View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddReview(int accommodationId, int rating, string comment)
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
                return RedirectToAction("Login", "Account");

            try
            {
                _accommodationService.AddReview(accommodationId, currentUser.Id, rating, comment);
                TempData["ReviewSuccess"] = "Thanks for sharing your review.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ReviewError"] = ex.Message;
            }
            catch (ArgumentException ex)
            {
                TempData["ReviewError"] = ex.Message;
            }

            return RedirectToAction("AccommodationDetails", new { id = accommodationId });
        }
    }
}
