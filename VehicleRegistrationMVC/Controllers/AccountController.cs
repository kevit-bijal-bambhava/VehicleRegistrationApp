using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using VehicleRegistrationMVC.Filters.ActionFilters;
using VehicleRegistrationMVC.Models;
using VehicleRegistrationMVC.Services;

namespace VehicleRegistrationMVC.Controllers
{
    [AllowAnonymous]
    [ServiceFilter(typeof(ModelStateValidationFilter))]
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;
        private readonly ILogger<AccountController> _logger;
        public AccountController(AccountService accountService, ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }
        [HttpGet]
        public IActionResult SignUp()
        {
            _logger.LogInformation("MVC_AccountController_SignUpGet");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel signUpmodel)
        {
            _logger.LogInformation("MVC_AccountController_SignUpPost");
            string response = await _accountService.SignUpAsync(signUpmodel);
            ModelState.AddModelError(string.Empty, response);
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult Login()
        {
            _logger.LogInformation("MVC_AccountController_LoginGet");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            _logger.LogInformation("MVC_AccountController_LoginPost");
            var result = await _accountService.LoginAsync(model, HttpContext);
           
           if(result != null)
            {
                if (result.Message == "Login Successful")
                {
                    HttpContext.Session.SetString("Token", result.JwtToken);
                    return RedirectToAction("GetVehiclesDetails", "Vehicle");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            _logger.LogInformation("MVC_AccountController_LogOut");
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}