using System.ComponentModel.DataAnnotations;

namespace VehicleRegistrationMVC.Models
{
    public class SignUpViewModel
    {
        public int UserId;
        [Required(ErrorMessage = "Name can't be blank")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please provide email address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please set the Password Its mandatory")]
        public string Password { get; set; }
    }
}
