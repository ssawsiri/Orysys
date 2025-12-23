using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OrysysLoanApplication.DataAccess;
using OrysysLoanApplication.Models;

namespace OrysysLoanApplication.Controllers
{
    public class LoginController : Controller
    {
        private readonly DataAccessLoanApplication _data;
        private readonly ILogger<LoginController> _logger;

        public LoginController(DataAccessLoanApplication data, ILogger<LoginController> logger)
        {
            _data = data;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginmodel)
        {
            bool isValidUser = _data.ValidateUser(loginmodel.Username, loginmodel.Password);

            if (!isValidUser)
            {
                loginmodel.ErrorMessage = "Invalid username or password";
                return View(loginmodel);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, loginmodel.Username)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            try
            {
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SignInAsync failed for user {User}. Scheme: {Scheme}", loginmodel.Username, CookieAuthenticationDefaults.AuthenticationScheme);
                throw;
            }

            return RedirectToAction("Index", "Home");
        }

       
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
