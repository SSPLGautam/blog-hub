using System.ComponentModel.DataAnnotations;

namespace BlogApp.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="Email isRequired Field")]
        [EmailAddress(ErrorMessage ="Email must be in format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password isRequired Field")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password",ErrorMessage ="Password must match the ConfirmPassword")]
        [DataType(DataType.Password)]
        public string ConfrimPassword { get; set; }
    }
}

