using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using OrysysLoanApplication.DataAccess;
using OrysysLoanApplication.Models;
using System.Data;
using System.Security.Claims;

namespace OrysysLoanApplication.Controllers
{
    public class LoginController : Controller
    {
        private readonly DataAccessLoanApplication _data;

        public LoginController(DataAccessLoanApplication data)
        {
            _data = data;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginmodel)
        {
            if (!ModelState.IsValid)
                return View(loginmodel);

            bool isValidUser = _data.ValidateUser(loginmodel.Username, loginmodel.Password);

            
            if (!isValidUser)
            {
                loginmodel.ErrorMessage = "Invalid username or password";
                return View(loginmodel);
            }

            // Create Authentication Cookie
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, loginmodel.Username)
        };

            var identity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
