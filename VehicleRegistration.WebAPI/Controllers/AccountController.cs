using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleRegistration.Infrastructure.DataBaseModels;
using VehicleRegistration.Core.ServiceContracts;
using VehicleRegistration.WebAPI.Models;
using VehicleRegistration.Core.Services;

namespace VehicleRegistration.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwttokenService;

        public AccountController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwttokenService = jwtService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if the username already exists
            var existingUser = await _userService.GetUserByEmailAsync(user.Email);
            if (existingUser != null)
                return BadRequest(new { Message = "This Email address is already registered." });

            // Create a new user
            var newUser = new UserModel
            {
                UserName = user.UserName,
                UserEmail = user.Email
            };
            await _userService.AddUser(newUser, user.Password);
            return Ok("Successfully signed up with Username = " + newUser.UserName + " & Email = " + newUser.UserEmail);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Login login)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var isAuthenticated = await _userService.AuthenticateUser(login.UserName, login.Password);

            if (!isAuthenticated)
                return BadRequest("Invalid credentials");

            var user = await _userService.GetUserByNameAsync(login.UserName);
            var tokenResponse = _jwttokenService.CreateJwtToken(user);

            return Ok(new
            {
                JwtToken = tokenResponse.Token,
                Message = "Login Successful",
                TokenExpiration = tokenResponse.Expiration

            });

        }
    }
}

