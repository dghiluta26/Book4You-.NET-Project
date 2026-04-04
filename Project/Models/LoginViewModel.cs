using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email-ul este obligatoriu.")]
        [EmailAddress(ErrorMessage = "Introdu un email valid.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Parola este obligatorie.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}