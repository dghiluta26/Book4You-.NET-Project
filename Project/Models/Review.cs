using System;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int AccommodationId { get; set; }
        public Accommodation Accommodation { get; set; } = null!;

        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}