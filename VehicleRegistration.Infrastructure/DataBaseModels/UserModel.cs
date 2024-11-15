using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using VehicleRegistration.Infrastructure.DataBaseModels.RBAC_Model;

namespace VehicleRegistration.Infrastructure.DataBaseModels
{
    public class UserModel
    {
        [Key]
        public int UserId {  get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string PasswordHash { get; set; }
        public string Salt {  get; set; }
        public string? ProfileImage { get; set; }
        public ICollection<VehicleModel> Vehicles { get; set; }
    }
}
