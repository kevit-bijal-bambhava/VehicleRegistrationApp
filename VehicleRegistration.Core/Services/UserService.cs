using VehicleRegistration.Infrastructure.DataBaseModels;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using VehicleRegistration.Core.ServiceContracts;
using VehicleRegistration.Infrastructure;
using Microsoft.Extensions.Logging;

namespace VehicleRegistration.Core.Services
{
    public class UserService : IUserService
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;

        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(ApplicationDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<UserModel> GetUserByIdAsync(Guid userId)
        {
            //_logger.LogInformation("WebAPI_GetUserById_UserService UserID: " + userId);
            return await _context.Users.FindAsync(userId);
        }
        public async Task<UserModel> GetUserByEmailAsync(string userEmail)
        {
            //_logger.LogInformation("WebAPI_GetUserByEmail_UserService Email: " + userEmail);
            return await _context.Users.FirstOrDefaultAsync(u => u.UserEmail == userEmail);
        }
        public async Task<UserModel> GetUserByNameAsync(string userName)
        {
            //_logger.LogInformation("WebAPI_GetUserByname_UserService UserName: " + userName);
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        }
        public async Task<(string PasswordHash, string Salt)> GetPasswordHashAndSalt(string userName)
        {
            var result = await _context.Users.Where(u => u.UserName == userName).Select(u => new { u.PasswordHash, u.Salt }).FirstOrDefaultAsync();
            return (result.PasswordHash, result.Salt);
        }

        public async Task AddUser(UserModel user, string plainPassword)
        {
            // Generate password hash and salt
            var (passwordHash, salt) = CreatePasswordHash(plainPassword);

            // Set the hashed password and salt on the user model
            user.PasswordHash = passwordHash;
            user.Salt = salt;

            // Add the user to the database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        public async Task<(bool IsAuthenticated, string ErrorMessage)> AuthenticateUser(string userName, string plainPassword)
        {
            // from database 
            var user = await GetUserByNameAsync(userName);
            if (user == null)
            {
                return (false, "User Doesn't Exists with this Credential.");
            }
                var (storedPasswordHash, storedSalt) = await GetPasswordHashAndSalt(userName);

            // Convert the stored salt from Base64 string to byte array
            var saltBytes = Convert.FromBase64String(storedSalt);

            // Compute the hash of the provided plain password using the stored salt
            var computedHash = ComputeHash(plainPassword, saltBytes);

            // Compare the computed hash with the stored password hash
            var isAuthenticated = computedHash == storedPasswordHash;

            // Return the result
            return (isAuthenticated, isAuthenticated ? null : "Invalid credential.");
        }


        // for creating password hash and salt 
        public (string PasswordHash, string Salt) CreatePasswordHash(string password)
        {
            // Generate a salt
            var salt = GenerateSalt();

            // Create a password hash using SHA-256
            var passwordHash = ComputeHash(password, salt);

            // Convert the salt to a Base64 string
            var saltString = Convert.ToBase64String(salt);
            return (passwordHash, saltString);
        }
        private byte[] GenerateSalt()
        {
            var salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
        private string ComputeHash(string password, byte[] salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);

                // Combine password bytes and salt
                var saltedPasswordBytes = passwordBytes.Concat(salt).ToArray();

                // Compute the hash
                var hashBytes = sha256.ComputeHash(saltedPasswordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}

