using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleRegistration.Infrastructure.DataBaseModels
{
    public class VehicleModel
    {
        [Key]
        public Guid VehicleId { get; set; } = new Guid();
        public string VehicleNumber { get; set; }
        public string? Description { get; set; }
        public string VehicleOwnerName { get; set; }
        public string? OwnerAddress { get; set; } 
        public string OwnerContactNumber { get; set; } 
        public string? Email { get; set; }
        public string VehicleClass { get; set; }
        public string FuelType { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public UserModel User { get; set; }
    }
}
