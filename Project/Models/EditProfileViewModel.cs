using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Project.Models
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Prenumele este obligatoriu.")]
        [MinLength(2, ErrorMessage = "Prenumele trebuie sa aiba minim 2 caractere.")]
        [MaxLength(100, ErrorMessage = "Prenumele poate avea maxim 100 de caractere.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Numele este obligatoriu.")]
        [MinLength(2, ErrorMessage = "Numele trebuie sa aiba minim 2 caractere.")]
        [MaxLength(100, ErrorMessage = "Numele poate avea maxim 100 de caractere.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adresa este obligatorie.")]
        [MinLength(5, ErrorMessage = "Adresa trebuie sa aiba minim 5 caractere.")]
        [MaxLength(255, ErrorMessage = "Adresa poate avea maxim 255 de caractere.")]
        public string? Address { get; set; }

        public string? ProfilePictureUrl { get; set; }

        public IFormFile? ProfilePictureFile { get; set; }
    }
}
