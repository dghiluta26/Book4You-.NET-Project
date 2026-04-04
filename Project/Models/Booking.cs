using System;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int AccommodationId { get; set; }
        public Accommodation Accommodation { get; set; } = null!;

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        public int Guests { get; set; }

        public decimal TotalPrice { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}