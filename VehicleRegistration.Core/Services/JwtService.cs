using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VehicleRegistration.Core.ServiceContracts;
using VehicleRegistration.Infrastructure.DataBaseModels;


namespace VehicleRegistration.Core.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtService> _logger;
        public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public AuthenticationResponse CreateJwtToken(UserModel user, string role)
        {
            try
            {
                var tokenId = Guid.NewGuid().ToString();

                if (user == null)
                {
                    return null;
                }

                List<Claim> claims = new()
                {
                    new Claim("TokenId", tokenId),
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, role.ToString())
                };

                SecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

                JwtSecurityToken token = new(
                     claims: claims,
                     expires: DateTime.UtcNow.AddDays(10),
                     signingCredentials: creds);

                string jwt = new JwtSecurityTokenHandler().WriteToken(token);

                return new AuthenticationResponse()
                {
                    Token = jwt,
                    Expiration = DateTime.UtcNow.AddDays(10),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured while creating JWT Token. {ex}", ex);
                throw;
            }
        }
    }
}
