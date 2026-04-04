using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class SignupViewModel
    {
        
        [Required(ErrorMessage = "Email-ul este obligatoriu.")]
        [EmailAddress(ErrorMessage = "Introdu un email valid.")]
        [MaxLength(100, ErrorMessage = "Email-ul poate avea maxim 100 de caractere.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prenumele este obligatoriu.")]
        [MinLength(2, ErrorMessage = "Prenumele trebuie sa aiba minim 2 caractere.")]
        [MaxLength(100, ErrorMessage = "Prenumele poate avea maxim 100 de caractere.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Numele este obligatoriu.")]
        [MinLength(2, ErrorMessage = "Numele trebuie sa aiba minim 2 caractere.")]
        [MaxLength(100, ErrorMessage = "Numele poate avea maxim 100 de caractere.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Parola este obligatorie.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Parola trebuie sa aiba minim 6 caractere.")]
        [MaxLength(100, ErrorMessage = "Parola poate avea maxim 100 de caractere.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).+$",
            ErrorMessage = "Parola trebuie sa contina cel putin o litera mare, o litera mica si o cifra.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirmarea parolei este obligatorie.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Parolele nu coincid.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adresa este obligatorie.")]
        [MinLength(5, ErrorMessage = "Adresa trebuie sa aiba minim 5 caractere.")]
        [MaxLength(255, ErrorMessage = "Adresa poate avea maxim 255 de caractere.")]
        public string? Address { get; set; }
    }
}