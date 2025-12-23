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
        

        public HomeController(DataAccessLoanApplication data)
        {
            _data = data;
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
                LogEvents.LogToFile("Error in Home Index", ex.Message);
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

                LogEvents.LogToFile("Error in Home Create", ex.Message);
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        [AllowAnonymous]
        public IActionResult Create(LoanApplicationModel objLoanApplication)
        {
            List<LoanTypesModel> loanTypes = _data.GetLoanType();
            bool result = false;
            if (!ModelState.IsValid)
            {
               TempData["errorMessage"] = "Form Validation Failes";
               return View();
            }
            objLoanApplication.LoanTypeID = loanTypes.Find(x => x.LoanTypeName == objLoanApplication.LoanTypeName).LoanTypeid;
            result = _data.AddLoanapplication(objLoanApplication);
            if (!result)
            {
                TempData["errorMessage"] = "Error in loan application creation";
                return View();
            }
            TempData["success"] = "Loan Application has been Created Successfully";
            return RedirectToAction("Index");

        }



        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        public IActionResult UpdateStatus(int loanId)
        {
            try
            {
                LoanApplicationModel loanApplication = _data.GetLoanApplicationbyId(loanId);
                if (loanApplication.LoanID == 0)
                {
                    TempData["errorMessage"] = "Loan Application not found with Loan ID : " + loanId;
                    return RedirectToAction("Index");
                }
                return View(loanApplication);
            }
            catch ( Exception Ex)
            {
                LogEvents.LogToFile("Error in Home UpdateStatus", Ex.Message);
                return View();
            }
        }

        [HttpPost]
        [Authorize]
        [AllowAnonymous]
        public IActionResult UpdateStatus(LoanApplicationModel loanApplication)
        {
            bool result = false;
            result = _data.UpdateLoanapplication(loanApplication);
            if (!result)
            {   TempData["errorMessage"] = "Error in updating loan application status";
                return View();
            }
            TempData["success"] = "Loan Application status has been updated Successfully";
            return RedirectToAction("Index");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
