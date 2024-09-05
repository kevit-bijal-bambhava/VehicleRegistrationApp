using System.ComponentModel.DataAnnotations;

namespace VehicleRegistration.WebAPI.Models
{
    public class User
    {
        public int UserId;
        [Required(ErrorMessage = "Name can't be blank")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please provide email address")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please set the Password Its mandatory")]
        public string Password { get; set; }
    }
}
