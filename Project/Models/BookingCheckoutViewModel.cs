using System;

namespace Project.Models
{
    public class BookingCheckoutViewModel
    {
        public int AccommodationId { get; set; }
        public string AccommodationTitle { get; set; } = string.Empty;
        public string AccommodationLocation { get; set; } = string.Empty;
        public string AccommodationImageUrl { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public int Guests { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int Nights { get; set; }
        public decimal TotalPrice { get; set; }
        public string? ReturnUrl { get; set; }
        public string? ErrorMessage { get; set; }
    }
}