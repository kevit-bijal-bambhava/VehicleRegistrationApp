using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VehicleRegistrationMVC.Models;
using VehicleRegistrationMVC.Services;

namespace VehicleRegistrationMVC.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;
        private readonly IHttpClientFactory _httpClientFactory;
        public AccountController(AccountService accountService, IHttpClientFactory httpClientFactory)
        {
            _accountService = accountService;
            _httpClientFactory = httpClientFactory;
        }
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel signUpmodel)
        {
            if (!ModelState.IsValid)
            {
                return View(signUpmodel);
            }
            string response = await _accountService.SignUpAsync(signUpmodel);
            ModelState.AddModelError(string.Empty, response);
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _accountService.LoginAsync(model, HttpContext);
            if (!string.IsNullOrEmpty(result.Message))
            {
                if(result.Message == "Login Successful")
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