using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleRegistrationMVC.Services;
using VehicleRegistrationMVC.Models;
using System.Threading.Tasks;

namespace VehicleRegistrationMVC.Controllers
{
    public class VehicleController : Controller
    {
        private readonly VehicleService _vehicleService;
        private readonly IHttpClientFactory _httpClientFactory;

        public VehicleController(IHttpClientFactory httpClientFactory, VehicleService vehicleService)
        {
            _httpClientFactory = httpClientFactory;
            _vehicleService = vehicleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetVehiclesDetails()
        {
            string jwtToken = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(jwtToken))
            {
                return RedirectToAction("Login", "Account");
            }

            List<VehicleViewModel> vehicles = await _vehicleService.GetVehicles(jwtToken);

            return View(vehicles);
        }

        [HttpGet]
        public IActionResult AddVehicleDetails()
        {
            string jwtToken = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(jwtToken))
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddVehicleDetails(VehicleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string jwtToken = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(jwtToken))
            {
                return RedirectToAction("Login", "Home");
            }
           
            var result = await _vehicleService.AddVehicle(model, jwtToken);
            TempData["SuccessMessage"] = "Vehicle details added successfully!";

            return RedirectToAction("GetVehiclesDetails");
        }

        [HttpGet]
        public async Task<IActionResult> EditVehicleDetails(Guid id)
        {
            string jwtToken = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(jwtToken))
            {
                return RedirectToAction("Login", "Home");
            }

            var vehicle = await _vehicleService.GetVehicleById(id, jwtToken);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        [HttpPost]
        public async Task<IActionResult> EditVehicleDetails(VehicleViewModel model, Guid Id)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string jwtToken = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(jwtToken))
            {
                return RedirectToAction("Login", "Home");
            }

            var result = await _vehicleService.UpdateVehicles(model,Id, jwtToken);
            TempData["Edited"] = "Details Updated Successfully!!";
            return RedirectToAction("GetVehiclesDetails");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteVehicle(Guid id)
        {
            string jwtToken = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(jwtToken))
            {
                return RedirectToAction("Login", "Home");
            }

            var result = await _vehicleService.DeleteVehicle(id, jwtToken);
            TempData["Deleted"] = "Vehicle Deleted Successfully!!";
            return RedirectToAction("GetVehiclesDetails");
        }

        [HttpGet]
        public async Task<IActionResult> GetVehicleById(Guid id)
        {
            string jwtToken = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(jwtToken))
            {
                return RedirectToAction("Login", "Home");
            }

            var vehicle = await _vehicleService.GetVehicleById(id, jwtToken);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }
    }
}
