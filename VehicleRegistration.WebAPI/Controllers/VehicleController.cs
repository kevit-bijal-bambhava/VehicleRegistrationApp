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

        public VehicleController(IVehicleService vehicleService, ApplicationDbContext db)
        {
            _vehicleService = vehicleService;
            _db = db;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddVehicle(Vehicle vehicle)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get UserId from authenticated user's claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //if (string.IsNullOrEmpty(userIdClaim))
            //{
            //    return Unauthorized("User ID not found in claims.");
            //}

            var newVehicle = new VehicleModel
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

        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditVehicle(Vehicle vehicle, Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingVehicle = await _vehicleService.GetVehicleByIdAsync(id);
            if (existingVehicle == null)
                return NotFound();

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

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteVehicle(Guid id)
        {
            var existingVehicle = await _vehicleService.GetVehicleByIdAsync(id);
            if (existingVehicle == null)
                return NotFound();

            await _vehicleService.DeleteVehicle(id);

            return Ok("Deleted Successfully.."); // Successfully deleted
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetVehicleById(Guid id)
        {
            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
            if (vehicle == null)
                return NotFound();

            return Ok(vehicle);
        }
        [HttpGet("getAllVehicle")]
        public async Task<IActionResult> GetAllVehicle()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //if (string.IsNullOrEmpty(userIdClaim))
            //{
            //    return Unauthorized("User ID not found in claims.");
            //}

            var vehicles = await _vehicleService.GetAllVehicles(userIdClaim);

            return Ok(vehicles);
        }

    }
}
