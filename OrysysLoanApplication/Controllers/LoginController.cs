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

        public LoginController(DataAccessLoanApplication data)
        {
            _data = data;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel loginmodel)
        {
            bool isValidUser = _data.ValidateUser(loginmodel.Username, loginmodel.Password);

            if (!isValidUser)
            {
                loginmodel.ErrorMessage = "Invalid username or password";
                _data.LogLoginAttempt(loginmodel.Username, false);
                return View(loginmodel);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, loginmodel.Username)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            try
            {
                
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = false
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity),
                    authProperties);
            }
            catch (Exception ex)
            {
                LogEvents.LogToFile("Login Error", $"SignInAsync failed for user {loginmodel.Username}. Exception: {ex}");
            }

            _data.LogLoginAttempt(loginmodel.Username, true);

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

        // Best-effort sign-out invoked by navigator.sendBeacon on unload
        [HttpPost]
        [AllowAnonymous]
        public IActionResult LogoutBeacon()
        {
            
            HttpContext.SignOutAsync().GetAwaiter().GetResult();
            return Ok();
        }
    }
}
