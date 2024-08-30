﻿using Microsoft.AspNetCore.Authorization;
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
        private readonly ILogger<VehicleController> _logger;
        public VehicleController(IHttpClientFactory httpClientFactory, VehicleService vehicleService, ILogger<VehicleController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _vehicleService = vehicleService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetVehiclesDetails()
        {
            _logger.LogInformation("MVC_VehicleController_GetVehicleDetails.");
            string jwtToken = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(jwtToken))
            {
                _logger.LogInformation("JWT Token is NULL");
                return RedirectToAction("Login", "Account");
            }

            List<VehicleViewModel> vehicles = await _vehicleService.GetVehicles(jwtToken);

            return View(vehicles);
        }

        [HttpGet]
        public IActionResult AddVehicleDetails()
        {
            _logger.LogInformation("MVC_VehicleController_AddVehicleDetailsGet.");
            string jwtToken = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(jwtToken))
            {
                _logger.LogInformation("JWT Token is NULL");
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddVehicleDetails(VehicleViewModel model)
        {
            _logger.LogInformation("MVC_VehicleController_AddVehicleDetailsPost.");
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string jwtToken = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(jwtToken))
            {
                _logger.LogInformation("JWT Token is NULL");
                return RedirectToAction("Login", "Home");
            }

            var result = await _vehicleService.AddVehicle(model, jwtToken);
            TempData["SuccessMessage"] = "Vehicle details added successfully!";

            return RedirectToAction("GetVehiclesDetails");
        }

        [HttpGet]
        public async Task<IActionResult> EditVehicleDetails(Guid id)
        {
            _logger.LogInformation("MVC_VehicleController_EditVehicleDetailsGet with VehicleId: " + id);
            string jwtToken = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(jwtToken))
            {
                _logger.LogInformation("JWT Token is NULL");
                return RedirectToAction("Login", "Home");
            }

            var vehicle = await _vehicleService.GetVehicleById(id, jwtToken);
            if (vehicle == null)
            {
                _logger.LogInformation("Vehicle Not Found.");
                return NotFound();
            }

            return View(vehicle);
        }

        [HttpPost]
        public async Task<IActionResult> EditVehicleDetails(VehicleViewModel model, Guid id)
        {
            _logger.LogInformation("MVC_VehicleController_EditVehicleDetailsPost with VehicleId: " + id);
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string jwtToken = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(jwtToken))
            {
                _logger.LogInformation("JWT Token is NULL");
                return RedirectToAction("Login", "Home");
            }

            var result = await _vehicleService.UpdateVehicles(model, id, jwtToken);
            TempData["Edited"] = "Details Updated Successfully!!";
            return RedirectToAction("GetVehiclesDetails");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteVehicle(Guid id)
        {
            _logger.LogInformation("MVC_VehicleController_DeleteVehicle with VehicleId: " + id);
            string jwtToken = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(jwtToken))
            {
                _logger.LogInformation("JWT Token is NULL");
                return RedirectToAction("Login", "Home");
            }

            var result = await _vehicleService.DeleteVehicle(id, jwtToken);
            TempData["Deleted"] = "Vehicle Deleted Successfully!!";
            return RedirectToAction("GetVehiclesDetails");
        }

        [HttpGet]
        public async Task<IActionResult> GetVehicleById(Guid id)
        {
            _logger.LogInformation("MVC_VehicleController_GetVehicleById with VehicleId: " + id);
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
