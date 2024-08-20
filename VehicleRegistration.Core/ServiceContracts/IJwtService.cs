using VehicleRegistration.Infrastructure.DataBaseModels;

namespace VehicleRegistration.Core.ServiceContracts
{
    public interface IJwtService
    {
        AuthenticationResponse CreateJwtToken(UserModel user);
    }
}
