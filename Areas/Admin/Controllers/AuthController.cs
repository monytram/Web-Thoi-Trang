using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebThoiTrang.Areas.Admin.Models;
using WebThoiTrang.Services;

namespace WebThoiTrang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            _logger.LogInformation($"Login GET - IsAuthenticated: {User.Identity.IsAuthenticated}");
            if (User.Identity.IsAuthenticated && User.IsInRole("ADMIN"))
            {
                _logger.LogInformation("User already authenticated as admin, redirecting to dashboard");
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginViewModel model, string returnUrl = null)
        {
            _logger.LogInformation($"Login POST - Email: {model.Email}");
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                _logger.LogInformation("Model is valid, validating user...");
                var isValid = await _authService.ValidateUserAsync(model.Email, model.Password);
                _logger.LogInformation($"User validation result: {isValid}");

                if (isValid)
                {
                    var user = await _authService.GetUserByEmailAsync(model.Email);
                    _logger.LogInformation($"Found user: {user?.Email}, UserRoles count: {user?.UserRoles?.Count}");

                    if (user.UserRoles?.Any(ur => ur.Role.Name == "ADMIN") == true)
                    {
                        _logger.LogInformation("User has ADMIN role, creating claims...");
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim("FullName", user.FullName)
                        };

                        foreach (var userRole in user.UserRoles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
                            _logger.LogInformation($"Added role claim: {userRole.Role.Name}");
                        }

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe
                        };

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);

                        _logger.LogInformation("Authentication completed, redirecting...");

                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            _logger.LogInformation($"Redirecting to return URL: {returnUrl}");
                            return Redirect(returnUrl);
                        }

                        _logger.LogInformation("Redirecting to dashboard");
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    }
                    else
                    {
                        _logger.LogWarning("User does not have ADMIN role");
                        ModelState.AddModelError(string.Empty, "Bạn không có quyền truy cập vào trang quản trị.");
                    }
                }
                else
                {
                    _logger.LogWarning("Invalid login attempt");
                    ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
                }
            }
            else
            {
                _logger.LogWarning("Model state is invalid");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning($"Model error: {error.ErrorMessage}");
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("Processing logout...");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("User logged out");
            return RedirectToAction("Login");
        }
    }
}