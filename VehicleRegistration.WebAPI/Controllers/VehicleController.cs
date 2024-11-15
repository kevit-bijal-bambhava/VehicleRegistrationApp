using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleRegistration.Infrastructure.DataBaseModels;
using VehicleRegistration.Infrastructure;
using VehicleRegistration.Core.ServiceContracts;
using VehicleRegistration.WebAPI.Models;
using System.Security.Claims;

namespace VehicleRegistration.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<VehicleController> _logger;

        public VehicleController(IVehicleService vehicleService, ApplicationDbContext db, ILogger<VehicleController> logger)
        {
            _vehicleService = vehicleService;
            _db = db;
            _logger = logger;
        }

        [HttpPost("Add")]
        [Authorize(Roles = "Admin, Clerk")]
        public async Task<IActionResult> AddVehicle(Vehicle vehicle)
        {
            try
            {
                _logger.LogInformation($"WebAPI_VehicleController_AddVehicle: {vehicle}");
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                string userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                VehicleModel newVehicle = new VehicleModel
                {
                    VehicleId = Guid.NewGuid(),
                    VehicleNumber = vehicle.VehicleNumber,
                    Description = vehicle.Description,
                    VehicleOwnerName = vehicle.VehicleOwnerName,
                    OwnerAddress = vehicle.OwnerAddress,
                    OwnerContactNumber = vehicle.OwnerContactNumber,
                    Email = vehicle.Email,
                    VehicleClass = vehicle.VehicleClass,
                    FuelType = vehicle.FuelType,
                    UserId = int.Parse(userIdClaim)
                };

                var addedVehicle = await _vehicleService.AddVehicle(newVehicle);
                return Ok(addedVehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError("WebAPI_VehicleController_AddVehicle : Got an error while adding vehicle. ", ex);
                throw;
            }
        }

        [HttpPut("Edit/{id}")]
        [Authorize(Roles = "Admin, Clerk")]
        public async Task<IActionResult> EditVehicle(Vehicle vehicle, Guid id)
        {
            try
            {
                _logger.LogInformation($"WebAPI_VehicleController_EditVehicle: {id}");
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var existingVehicle = await _vehicleService.GetVehicleByIdAsync(id);
                if (existingVehicle == null)
                {
                    _logger.LogInformation("Vehicle doesn't Exists with Id: " + id);
                    return NotFound();
                }

                existingVehicle.VehicleNumber = vehicle.VehicleNumber;
                existingVehicle.Description = vehicle.Description;
                existingVehicle.VehicleOwnerName = vehicle.VehicleOwnerName;
                existingVehicle.OwnerAddress = vehicle.OwnerAddress;
                existingVehicle.OwnerContactNumber = vehicle.OwnerContactNumber;
                existingVehicle.Email = vehicle.Email;
                existingVehicle.VehicleClass = vehicle.VehicleClass;
                existingVehicle.FuelType = vehicle.FuelType;

                var updatedVehicle = await _vehicleService.EditVehicle(existingVehicle);

                return Ok(updatedVehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError("WebAPI_VehicleController_EditVehicle : Got an error while updating vehicle. ", ex);
                throw;
            }
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteVehicle(Guid id)
        {
            try
            {
                _logger.LogInformation($"WebAPI_VehicleController_DeleteVehicle: {id}");
                var existingVehicle = await _vehicleService.GetVehicleByIdAsync(id);
                if (existingVehicle == null)
                {
                    _logger.LogInformation("Vehicle Not Found with Id: " + id);
                    return NotFound();
                }
                _logger.LogInformation("Vehicle Exists with Id: " + id);
                await _vehicleService.DeleteVehicle(id);

                return Ok("Deleted Successfully..");
            }
            catch (Exception ex)
            {
                _logger.LogError("WebAPI_VehicleController_DeleteVehicle : Got an error while deleting vehicle. ", ex);
                throw;
            }
        }

        [HttpGet("Get/{id}")]
        [Authorize(Roles = "Admin, Clerk, User")]
        public async Task<IActionResult> GetVehicleById(Guid id)
        {
            try
            {
                _logger.LogInformation($"WebAPI_VehicleController_GetVehicleById: {id}");
                var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
                if (vehicle == null)
                {
                    _logger.LogInformation("Vehicle Not Found with Id: " + id);
                    return NotFound();
                }
                _logger.LogInformation("Vehicle Found with Id: " + id);
                return Ok(vehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError("WebAPI_VehicleController_GetVehicleById : Got an error while fetching a vehicle. ", ex);
                throw;
            }
        }

        [HttpGet("GetAllVehicle")]
        [Authorize(Roles = "Admin, Clerk, User")]
        public async Task<IActionResult> GetAllVehicle()
        {
            try
            {
                _logger.LogInformation("WebAPI_VehicleController_GetAllVehicle");
                string userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var vehicles = await _vehicleService.GetAllVehicles(userIdClaim);
                _logger.LogInformation("All Vehicle details fetched.");
                return Ok(vehicles);
            }
            catch (Exception ex)
            {
                _logger.LogError("WebAPI_VehicleController_GetAllVehicle : Got an error while fetching vehicles. ", ex);
                throw;
            }
        }
    }
}
