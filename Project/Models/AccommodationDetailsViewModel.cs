using System.Collections.Generic;

namespace Project.Models
{
    public class AccommodationDetailsViewModel
    {
        public Accommodation Accommodation { get; set; } = null!;
        public List<Amenity> Amenities { get; set; } = new();
        public List<AccommodationImage> Images { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
        public bool CanLeaveReview { get; set; }
        public bool HasReviewed { get; set; }
    }
}