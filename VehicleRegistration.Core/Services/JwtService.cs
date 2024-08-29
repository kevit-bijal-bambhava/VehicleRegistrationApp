using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VehicleRegistration.Infrastructure.DataBaseModels;
using VehicleRegistration.Core.ServiceContracts;


namespace VehicleRegistration.Core.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public AuthenticationResponse CreateJwtToken(UserModel user)
        {
            var tokenId = Guid.NewGuid().ToString();

           if(user == null)
            {
                return null;
            }

            List<Claim> claims = new()
            {
                new Claim("TokenId", tokenId),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
            };
            SecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new(
                 claims: claims,
                 expires: DateTime.UtcNow.AddDays(1),
                 signingCredentials: creds);

            string jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthenticationResponse()
            {
                Token = jwt,
                Expiration = DateTime.UtcNow.AddDays(1),
            };
        }
    }
}
