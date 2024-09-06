using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace VehicleRegistrationMVC.Models
{
    public class VehicleViewModel
    {
        [BindNever]
        public Guid VehicleId { get; set; } = new Guid();

        [Required(ErrorMessage = "RTO register vehicle number is necessary")]
        public string VehicleNumber { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Please provide owner name")]
        public string VehicleOwnerName { get; set; }

        public string? OwnerAddress { get; set; }

        [Required(ErrorMessage = "Please provide the contact number")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number should contain digits only")]
        public string OwnerContactNumber { get; set; }

        public string? Email { get; set; }

        [Required(ErrorMessage = "Please provide the class/type of the vehicle")]
        public string VehicleClass { get; set; }

        [Required(ErrorMessage = "Provide the type of fuel the vehicle consumes")]
        public string FuelType { get; set; }
    }
}
