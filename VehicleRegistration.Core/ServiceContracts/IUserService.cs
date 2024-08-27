using VehicleRegistration.Infrastructure.DataBaseModels;

namespace VehicleRegistration.Core.ServiceContracts
{
    public interface IUserService
    {
        Task<UserModel> GetUserByIdAsync(Guid userId);
        Task<UserModel> GetUserByEmailAsync(string userEmail);
        Task<UserModel> GetUserByNameAsync(string userName);
        Task<(string PasswordHash, string Salt)> GetPasswordHashAndSalt(string userName);
        Task AddUser(UserModel user, string plainPassword);
        Task<(bool IsAuthenticated, string ErrorMessage)> AuthenticateUser(string userName, string plainPassword);
        (string PasswordHash, string Salt) CreatePasswordHash(string password);
    }
}
