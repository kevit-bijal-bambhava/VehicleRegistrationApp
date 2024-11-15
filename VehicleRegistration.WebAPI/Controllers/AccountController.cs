using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleRegistration.Infrastructure.DataBaseModels;
using VehicleRegistration.Core.ServiceContracts;
using VehicleRegistration.WebAPI.Models;
using VehicleRegistration.Core.Services;
using System.Security.Claims;
using VehicleRegistration.Infrastructure;
using Microsoft.EntityFrameworkCore;
using VehicleRegistration.Infrastructure.DataBaseModels.RBAC_Model;

namespace VehicleRegistration.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwttokenService;
        private readonly ILogger<AccountController> _logger;
        private readonly ApplicationDbContext _dbContext;
        public AccountController(IUserService userService, IJwtService jwtService, ILogger<AccountController> logger, ApplicationDbContext dbContext)
        {
            _userService = userService;
            _jwttokenService = jwtService;
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp(User user)
        {
            try
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
                UserModel newUser = new()
                {
                    UserName = user.UserName,
                    UserEmail = user.Email
                };

                int userId = await _userService.AddUser(newUser, user.Password);
                
                var roleDetails = await _dbContext.Roles.Where(x => x.RoleName == user.RoleName).FirstOrDefaultAsync();

                UserRole userRole = new()
                {
                    UserId = userId,
                    RoleId = roleDetails.RoleId
                };

                _dbContext.UserRoles.Add(userRole);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("User Added.");
                return Ok("Successfully signed up with Username = " + newUser.UserName + " & Email = " + newUser.UserEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError("WebAPI_AccountController_SignUp : Got an error while SignUp. {ex}", ex);
                throw;
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login(Login login)
        {
            try
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
                int userId = user.UserId;
                string? role = await _dbContext.UserRoles
                                  .Where(ur => ur.UserId == userId)
                                  .Select(ur => ur.Role.RoleName)
                                  .FirstOrDefaultAsync();

                var tokenResponse = _jwttokenService.CreateJwtToken(user, role);

                if (tokenResponse == null)
                {
                    _logger.LogInformation("Token not Generated.");
                    return BadRequest("Error occured while generating JWT Token.");
                }
                _logger.LogInformation("Token Generated.");
                return Ok(new
                {
                    JwtToken = tokenResponse.Token,
                    Message = "Login Successful",
                    TokenExpiration = tokenResponse.Expiration
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("WebAPI_AccountController_Login : Got an error while Login. {ex}", ex);
                throw;
            }
        }

        [HttpPost("updateProfile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(IFormFile profileImg)
        {
            try
            {
                if (profileImg == null || profileImg.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = await _userService.GetUserByIdAsync(userId);

                // Define the directory to save the uploaded file
                var uploadPath = Path.Combine("wwwroot", "ProfileImages");

                // Ensure the directory exists
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Create the full file path
                string fileName = Path.GetFileName(profileImg.FileName);
                string filePath = Path.Combine(uploadPath, fileName);

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
                _logger.LogError("WebAPI_AccountController_UpdateProfile : Got an error while uploading image. {ex}", ex);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

