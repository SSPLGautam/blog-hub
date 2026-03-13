using System.ComponentModel.DataAnnotations;

namespace BlogApp.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email isRequired Field")]
        [EmailAddress(ErrorMessage = "Email must be in format")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password isRequired Field")]
        [DataType(DataType.Password)]
        public string Password { get; set; }    


    }
}

