using Project.Models;

namespace Project.Repositories;

public interface IAccommodationRepository
{
    Accommodation? GetById(int id);
    List<Accommodation> GetAll();
    List<Accommodation> GetAllForAdmin();
    void Add(Accommodation accommodation);
    void Remove(Accommodation accommodation);
    void SaveChanges();
    List<Amenity> GetAmenities(int accommodationId);
    List<AccommodationImage> GetImages(int accommodationId);
    List<Review> GetReviews(int accommodationId);
}
