using System.Collections.Generic;

namespace Project.Models
{
    public class AccommodationDetailsViewModel
    {
        public Accommodation Accommodation { get; set; } = null!;
        public List<Amenity> Amenities { get; set; } = new();
        public List<AccommodationImage> Images { get; set; } = new();
    }
}