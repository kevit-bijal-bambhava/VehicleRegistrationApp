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

        private readonly ApplicationDbContext _db;
        private readonly ILogger<UserService> _logger;

        public UserService(ApplicationDbContext db, ILogger<UserService> logger)
        {
            _db = db;
            _logger = logger;
        }
        public async Task<UserModel> GetUserByIdAsync(int userId)
        {
            try
            {
                return await _db.Users.FindAsync(userId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<UserModel> GetUserByEmailAsync(string userEmail)
        {
            try
            {
                return await _db.Users.FirstOrDefaultAsync(u => u.UserEmail == userEmail);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<UserModel> GetUserByNameAsync(string userName)
        {
            try
            {
                return await _db.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<(string PasswordHash, string Salt)> GetPasswordHashAndSalt(string userName)
        {
            try
            {
                var result = await _db.Users.Where(u => u.UserName == userName).Select(u => new { u.PasswordHash, u.Salt }).FirstOrDefaultAsync();
                return (result.PasswordHash, result.Salt);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> AddUser(UserModel user, string plainPassword)
        {
            try
            {
                // Generate password hash and salt
                var (passwordHash, salt) = CreatePasswordHash(plainPassword);

                // Set the hashed password and salt on the user model
                user.PasswordHash = passwordHash;
                user.Salt = salt;

                // Add the user to the database
                _db.Users.Add(user);
                await _db.SaveChangesAsync();
                return user.UserId;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task UpdateUser(UserModel user)
        {
            try
            {
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<(bool IsAuthenticated, string ErrorMessage)> AuthenticateUser(string userName, string plainPassword)
        {
            try
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
            catch (Exception)
            {
                throw;
            }
        }
        // for creating password hash and salt 
        public (string PasswordHash, string Salt) CreatePasswordHash(string password)
        {
            try
            {
                // Generate a salt
                var salt = GenerateSalt();

                // Create a password hash using SHA-256
                var passwordHash = ComputeHash(password, salt);

                // Convert the salt to a Base64 string
                var saltString = Convert.ToBase64String(salt);
                return (passwordHash, saltString);
            }
            catch (Exception)
            {
                throw;
            }
        }
        private byte[] GenerateSalt()
        {
            try
            {
                var salt = new byte[SaltSize];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }
                return salt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private string ComputeHash(string password, byte[] salt)
        {
            try
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
            catch (Exception)
            {
                throw;
            }
        }
    }
}

