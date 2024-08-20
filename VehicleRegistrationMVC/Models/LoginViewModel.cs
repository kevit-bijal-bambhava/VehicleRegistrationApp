using System.ComponentModel.DataAnnotations;

namespace VehicleRegistrationMVC.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Enter the UserName")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Enter the password")]
        public string Password { get; set; }
    }
}
