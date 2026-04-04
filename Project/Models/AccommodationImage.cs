using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class AccommodationImage
    {
        public int Id { get; set; }

        public int AccommodationId { get; set; }
        public Accommodation Accommodation { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsMain { get; set; } = false;

        public int DisplayOrder { get; set; }
    }
}