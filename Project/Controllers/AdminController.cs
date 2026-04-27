using Microsoft.AspNetCore.Mvc;
using Project.Filters;
using Project.Models;
using Project.Services;

namespace Project.Controllers
{
    [RequireAdmin]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Bookings()
        {
            return View(_adminService.GetBookings());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelBooking(int id)
        {
            try
            {
                _adminService.CancelBooking(id);
                return RedirectToAction("Bookings");
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        public IActionResult Users()
        {
            return View(_adminService.GetUsers());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleRole(int id)
        {
            try
            {
                _adminService.ToggleRole(id);
                return RedirectToAction("Users");
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        public IActionResult UnavailablePeriods()
        {
            return View(_adminService.GetUnavailablePeriods());
        }

        public IActionResult CreateUnavailablePeriod()
        {
            var viewModel = _adminService.GetCreateUnavailablePeriodViewModel();
            ViewData["Accommodations"] = viewModel.Accommodations;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUnavailablePeriod(CreateUnavailablePeriodViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var reloadModel = _adminService.GetCreateUnavailablePeriodViewModel();
                reloadModel.UnavailablePeriod = viewModel.UnavailablePeriod;
                ViewData["Accommodations"] = reloadModel.Accommodations;
                return View(reloadModel);
            }

            try
            {
                _adminService.CreateUnavailablePeriod(viewModel.UnavailablePeriod, "Created from admin panel");
                return RedirectToAction("UnavailablePeriods");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var reloadModel = _adminService.GetCreateUnavailablePeriodViewModel();
                reloadModel.UnavailablePeriod = viewModel.UnavailablePeriod;
                ViewData["Accommodations"] = reloadModel.Accommodations;
                return View(reloadModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUnavailablePeriod(int id)
        {
            try
            {
                _adminService.DeleteUnavailablePeriod(id);
                return RedirectToAction("UnavailablePeriods");
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }
    }
}
