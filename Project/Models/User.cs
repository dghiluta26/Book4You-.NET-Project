using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class User
    {

        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Password { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Address { get; set; }

        public string? ProfilePictureUrl { get; set; }

        [MaxLength(50)]
        public string Role { get; set; } = "User"; // User or Admin


    }
}
