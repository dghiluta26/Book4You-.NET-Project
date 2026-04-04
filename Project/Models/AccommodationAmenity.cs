namespace Project.Models
{
    public class AccommodationAmenity
    {
        public int Id { get; set; }

        public int AccommodationId { get; set; }
        public Accommodation Accommodation { get; set; } = null!;

        public int AmenityId { get; set; }

        public Amenity Amenity { get; set; } = null!;
    }
}
