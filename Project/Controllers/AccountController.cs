using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.Services;

namespace Project.Controllers
{
    public class AccountController : Controller
    {
        private const long MaxProfilePictureSize = 5 * 1024 * 1024;
        private static readonly HashSet<string> AllowedProfilePictureExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".gif",
            ".webp"
        };

        private readonly IAccountService _accountService;
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _environment;

        public AccountController(IAccountService accountService, IUserService userService, IWebHostEnvironment environment)
        {
            _accountService = accountService;
            _userService = userService;
            _environment = environment;
        }

        public IActionResult Profile()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
                return RedirectToAction("Login");

            var user = _accountService.GetByEmail(userEmail);
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

            var user = _accountService.GetByEmail(userEmail);
            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }

            return View(new EditProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                ProfilePictureUrl = user.ProfilePictureUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
                return RedirectToAction("Login");

            var user = _accountService.GetByEmail(userEmail);
            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }

            model.ProfilePictureUrl = user.ProfilePictureUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? uploadedProfilePictureUrl = null;

            try
            {
                var profilePictureUrl = user.ProfilePictureUrl;
                var previousProfilePictureUrl = user.ProfilePictureUrl;

                if (model.ProfilePictureFile != null)
                {
                    var profilePictureFile = model.ProfilePictureFile;
                    var validationError = ValidateProfilePicture(profilePictureFile);
                    if (validationError != null)
                    {
                        ModelState.AddModelError(nameof(model.ProfilePictureFile), validationError);
                        return View(model);
                    }

                    profilePictureUrl = await SaveProfilePictureAsync(profilePictureFile);
                    uploadedProfilePictureUrl = profilePictureUrl;
                    model.ProfilePictureUrl = profilePictureUrl;
                }

                _accountService.UpdateProfile(
                    user,
                    model.FirstName.Trim(),
                    model.LastName.Trim(),
                    model.Address?.Trim(),
                    profilePictureUrl);

                if (profilePictureUrl != previousProfilePictureUrl)
                {
                    DeletePreviousProfilePicture(previousProfilePictureUrl);
                }

                HttpContext.Session.SetString("UserName", user.FirstName);
                TempData["ProfileSuccess"] = "Your profile was updated successfully.";
                return RedirectToAction("Profile");
            }
            catch (Exception)
            {
                DeletePreviousProfilePicture(uploadedProfilePictureUrl);
                ModelState.AddModelError(string.Empty, "The profile could not be saved right now. Please try again.");
                return View(model);
            }
        }

        private static string? ValidateProfilePicture(IFormFile file)
        {
            if (file.Length == 0)
                return "Please choose an image file.";

            if (file.Length > MaxProfilePictureSize)
                return "The profile picture must be 5 MB or smaller.";

            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(extension) || !AllowedProfilePictureExtensions.Contains(extension))
                return "Only JPG, PNG, GIF, or WEBP images are accepted.";

            if (string.IsNullOrWhiteSpace(file.ContentType) || !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                return "Only image files are accepted.";

            using var stream = file.OpenReadStream();
            Span<byte> header = stackalloc byte[12];
            var bytesRead = stream.Read(header);

            return IsRecognizedImageHeader(header[..bytesRead], extension)
                ? null
                : "The selected file does not appear to be a valid image.";
        }

        private static bool IsRecognizedImageHeader(ReadOnlySpan<byte> header, string extension)
        {
            if (extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                return header.Length >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF;
            }

            if (extension.Equals(".png", StringComparison.OrdinalIgnoreCase))
            {
                return header.Length >= 8 &&
                    header[0] == 0x89 &&
                    header[1] == 0x50 &&
                    header[2] == 0x4E &&
                    header[3] == 0x47 &&
                    header[4] == 0x0D &&
                    header[5] == 0x0A &&
                    header[6] == 0x1A &&
                    header[7] == 0x0A;
            }

            if (extension.Equals(".gif", StringComparison.OrdinalIgnoreCase))
            {
                return header.Length >= 6 &&
                    header[0] == 0x47 &&
                    header[1] == 0x49 &&
                    header[2] == 0x46 &&
                    header[3] == 0x38 &&
                    (header[4] == 0x37 || header[4] == 0x39) &&
                    header[5] == 0x61;
            }

            if (extension.Equals(".webp", StringComparison.OrdinalIgnoreCase))
            {
                return header.Length >= 12 &&
                    header[0] == 0x52 &&
                    header[1] == 0x49 &&
                    header[2] == 0x46 &&
                    header[3] == 0x46 &&
                    header[8] == 0x57 &&
                    header[9] == 0x45 &&
                    header[10] == 0x42 &&
                    header[11] == 0x50;
            }

            return false;
        }

        private async Task<string> SaveProfilePictureAsync(IFormFile file)
        {
            var uploadsDirectory = Path.Combine(_environment.WebRootPath, "images", "users");
            Directory.CreateDirectory(uploadsDirectory);

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(uploadsDirectory, fileName);

            await using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            return $"/images/users/{fileName}";
        }

        private void DeletePreviousProfilePicture(string? previousProfilePictureUrl)
        {
            if (string.IsNullOrWhiteSpace(previousProfilePictureUrl) ||
                previousProfilePictureUrl.EndsWith("default-avatar.svg", StringComparison.OrdinalIgnoreCase) ||
                !previousProfilePictureUrl.StartsWith("/images/users/", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var fileName = Path.GetFileName(previousProfilePictureUrl);
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            var filePath = Path.Combine(_environment.WebRootPath, "images", "users", fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            model.Email = (model.Email?.Trim() ?? string.Empty).ToLower();
            if (!ModelState.IsValid)
                return View(model);

            var user = _userService.GetByEmail(model.Email);
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

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

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
            model.LastName = model.LastName?.Trim() ?? string.Empty;
            model.Address = model.Address?.Trim();
            if (!ModelState.IsValid)
                return View(model);

            if (_userService.GetByEmail(model.Email) != null)
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

            _userService.Add(user);
            _userService.SaveChanges();

            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
