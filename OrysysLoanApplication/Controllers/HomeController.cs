using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrysysLoanApplication.DataAccess;
using OrysysLoanApplication.Models;
using System.Diagnostics;

namespace OrysysLoanApplication.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private readonly DataAccessLoanApplication _data;
        private readonly ILogger<LoginController> _logger;

        public HomeController(DataAccessLoanApplication data, ILogger<LoginController> logger)
        {
            _data = data;
            _logger = logger;
        }

        public IActionResult Index()
        {
            List<LoanApplicationModel> loanApplications = new List<LoanApplicationModel>();
            try
            {
                 loanApplications = _data.GetAllLoanApplication();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in : " + ex.Message);
                TempData["ErrorMessage"] = "An error occurred while fetching loan types." + ex.Message;
            }  
            return View(loanApplications);
        }


        public IActionResult Create()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
