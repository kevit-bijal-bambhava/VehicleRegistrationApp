using VehicleRegistration.Infrastructure.DataBaseModels;
using VehicleRegistration.Infrastructure.DataBaseModels.RBAC_Model;

namespace VehicleRegistration.Core.ServiceContracts
{
    public interface IJwtService
    {
        AuthenticationResponse CreateJwtToken(UserModel user, string role);
    }
}
