using System;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class Accommodation
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public decimal PricePerNight { get; set; }

        public int Capacity { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }

        public string Type { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

        public decimal Rating { get; set; }

        public bool IsAvailable { get; set; }

        public DateTime CreatedAt { get; set; }
        public int OwnerId { get; set; } // user who created the accommodation
    }
}