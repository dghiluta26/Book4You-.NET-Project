using Microsoft.AspNetCore.Mvc;
using Project.Data;
using Project.Filters;
using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Controllers
{
    [RequireAdmin]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Bookings()
        {
            var bookings = _context.Bookings
                .Include(b => b.Accommodation)
                .Include(b => b.User)
                .OrderByDescending(b => b.CreatedAt)
                .ToList();

            return View(bookings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelBooking(int id)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.Id == id);
            if (booking == null) return NotFound();

            booking.Status = "Cancelled";
            _context.SaveChanges();

            return RedirectToAction("Bookings");
        }

        public IActionResult Users()
        {
            var users = _context.Users.OrderBy(u => u.Email).ToList();
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleRole(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();

            user.Role = user.Role == "Admin" ? "User" : "Admin";
            _context.SaveChanges();
            return RedirectToAction("Users");
        }

        public IActionResult UnavailablePeriods()
        {
            var periods = _context.Set<UnavailablePeriod>()
                .OrderByDescending(p => p.StartDate)
                .ToList();

            return View(periods);
        }

        public IActionResult CreateUnavailablePeriod()
        {
            var viewModel = new CreateUnavailablePeriodViewModel
            {
                Accommodations = _context.Accommodations.OrderBy(a => a.Title).ToList()
            };

            ViewData["Accommodations"] = viewModel.Accommodations;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUnavailablePeriod(CreateUnavailablePeriodViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Accommodations = _context.Accommodations.OrderBy(a => a.Title).ToList();
                ViewData["Accommodations"] = viewModel.Accommodations;
                return View(viewModel);
            }

            if (viewModel.UnavailablePeriod.StartDate >= viewModel.UnavailablePeriod.EndDate)
            {
                ModelState.AddModelError(string.Empty, "End date must be after start date.");
                viewModel.Accommodations = _context.Accommodations.OrderBy(a => a.Title).ToList();
                ViewData["Accommodations"] = viewModel.Accommodations;
                return View(viewModel);
            }

            _context.Set<UnavailablePeriod>().Add(viewModel.UnavailablePeriod);
            _context.SaveChanges();
            return RedirectToAction("UnavailablePeriods");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUnavailablePeriod(int id)
        {
            var p = _context.Set<UnavailablePeriod>().FirstOrDefault(x => x.Id == id);
            if (p == null) return NotFound();
            _context.Set<UnavailablePeriod>().Remove(p);
            _context.SaveChanges();
            return RedirectToAction("UnavailablePeriods");
        }
    }
}
