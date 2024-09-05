using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleRegistration.Infrastructure.DataBaseModels;
using VehicleRegistration.Core.ServiceContracts;
using VehicleRegistration.WebAPI.Models;
using VehicleRegistration.Core.Services;
using System.Security.Claims;

namespace VehicleRegistration.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwttokenService;
        private readonly ILogger<AccountController> _logger;
        public AccountController(IUserService userService, IJwtService jwtService, ILogger<AccountController> logger)
        {
            _userService = userService;
            _jwttokenService = jwtService;
            _logger = logger;
        }

        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp(User user)
        {
            _logger.LogInformation("WebAPI_AccountController_SignUp");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if the userEmail already exists
            var existingUser = await _userService.GetUserByEmailAsync(user.Email);
            if (existingUser != null)
            {
                _logger.LogInformation("User Email Exists.");
                return BadRequest(new { Message = "This Email address is already registered." });
            }

            // Create a new user
            var newUser = new UserModel
            {
                UserName = user.UserName,
                UserEmail = user.Email
            };
            await _userService.AddUser(newUser, user.Password);
            _logger.LogInformation("User Added.");
            return Ok("Successfully signed up with Username = " + newUser.UserName + " & Email = " + newUser.UserEmail);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Login login)
        {
            _logger.LogInformation("WebAPI_AccountController_Login");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (isAuthenticated, errorMessage) = await _userService.AuthenticateUser(login.UserName, login.Password);

            if (!isAuthenticated || errorMessage != null)
            {
                _logger.LogInformation("User Not Authenticate.");
                return BadRequest(errorMessage);
            }

            var user = await _userService.GetUserByNameAsync(login.UserName);
            var tokenResponse = _jwttokenService.CreateJwtToken(user);

            if (tokenResponse == null)
            {
                _logger.LogInformation("Token not Generated.");
                return null;
            }
            _logger.LogInformation("Token Generated.");
            _logger.LogInformation("Log in Successful.");
            return Ok(new
            {
                JwtToken = tokenResponse.Token,
                Message = "Login Successful",
                TokenExpiration = tokenResponse.Expiration
            });

        }

        [HttpPost("updateProfile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(IFormFile profileImg)
        {
            if (profileImg == null || profileImg.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = await _userService.GetUserByIdAsync(userId);

                // Define the directory to save the uploaded file
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProfileImages");

                // Ensure the directory exists
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Create the full file path
                var fileName = Path.GetFileName(profileImg.FileName);
                var filePath = Path.Combine(uploadPath, fileName);

                // Save the file to the specified path
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profileImg.CopyToAsync(stream);
                }

                // Update the user's profile with the filePath
                user.ProfileImage = filePath; 
                await _userService.UpdateUser(user);

                return Ok(new
                {
                    filePath = filePath,
                });
            }
            catch (Exception ex)
            {
                // Handle exceptions (log error, etc.)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}

