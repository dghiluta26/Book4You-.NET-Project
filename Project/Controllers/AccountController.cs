using Microsoft.AspNetCore.Mvc;
using Project.Data;
using Microsoft.AspNetCore.Identity;
using Project.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Profile()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
                return RedirectToAction("Login");

            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }

            return View(user);
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
                return RedirectToAction("Login");

            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult EditProfile(User model)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
                return RedirectToAction("Login");

            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Address = model.Address;

            try
            {
                if (model.ProfilePictureFile != null && model.ProfilePictureFile.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var extension = Path.GetExtension(model.ProfilePictureFile.FileName).ToLowerInvariant();
                    var contentType = model.ProfilePictureFile.ContentType?.Trim() ?? string.Empty;

                    if (!allowedExtensions.Contains(extension) || !contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError(nameof(model.ProfilePictureFile), "Only JPG, PNG, GIF, and WEBP images are allowed.");
                        return View(user);
                    }

                    if (model.ProfilePictureFile.Length > 2 * 1024 * 1024)
                    {
                        ModelState.AddModelError(nameof(model.ProfilePictureFile), "Profile pictures must be 2 MB or smaller.");
                        return View(user);
                    }

                    using var memoryStream = new MemoryStream();
                    model.ProfilePictureFile.CopyTo(memoryStream);
                    user.ProfilePictureUrl = $"data:{contentType};base64,{Convert.ToBase64String(memoryStream.ToArray())}";
                }
                _context.SaveChanges();

                HttpContext.Session.SetString("UserName", user.FirstName);

                return RedirectToAction("Profile");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "The profile could not be saved right now. Please try again.");
                return View(user);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            model.Email = (model.Email?.Trim() ?? string.Empty).ToLower();
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email sau parola incorecta.");
                return View(model);
            }

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.Password, model.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "Email sau parola incorecta";
                return View(model);
            }

            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", user.FirstName);
            HttpContext.Session.SetString("UserRole", user.Role ?? "User");

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Signup(SignupViewModel model)
        {
            model.Email = (model.Email?.Trim() ?? string.Empty).ToLower();
            model.FirstName = model.FirstName?.Trim() ?? string.Empty;
            model.LastName = (model.LastName?.Trim() ?? string.Empty);
            model.Address = model.Address?.Trim();
            if (!ModelState.IsValid)
                return View(model);

            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Exista deja un cont cu acest email.");
                return View(model);
            }

            var user = new User
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
            };
            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, model.Password);

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
