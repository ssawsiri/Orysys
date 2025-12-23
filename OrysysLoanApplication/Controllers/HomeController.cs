using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        [Authorize]
        [AllowAnonymous]
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
                TempData["ErrorMessage"] = "An error occurred while fetching loan Application Details." + ex.Message;
            }  
            return View(loanApplications);
        }

        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        public IActionResult Create()
        {
            List<LoanTypesModel> loanTypes = _data.GetLoanType();

            try
            {
                ViewBag.LoanTypes = new SelectList(loanTypes, "LoanTypeName", "LoanTypeName");

                ViewBag.LoanTypesJson = System.Text.Json.JsonSerializer.Serialize(loanTypes);
            }
            catch (Exception ex)
            {

                _logger.LogError("Error in : " + ex.Message);
                TempData["ErrorMessage"] = "An error occurred while fetching loan types." + ex.Message;
            }

            return View();
        }

    

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
