using VehicleRegistration.Infrastructure.DataBaseModels;

namespace VehicleRegistration.Core.ServiceContracts
{
    public interface IVehicleService
    {
        Task<VehicleModel> GetVehicleByIdAsync(Guid vehicleId);
        public Task<IEnumerable<VehicleModel>> GetAllVehicles(string userId);
        Task<VehicleModel> AddVehicle(VehicleModel vehicle);
        Task<VehicleModel> EditVehicle(VehicleModel vehicle);
        Task<VehicleModel> DeleteVehicle(Guid vehicleId);
    }
}
