using Project.Models;

namespace Project.Services;

public interface IAccommodationService
{
    List<Accommodation> GetAllForAdmin();
    List<Accommodation> Search(string? location, string? type, decimal? maxPrice, int? guests, string? checkIn, string? checkOut);
    AccommodationDetailsViewModel? GetDetails(int id, int? currentUserId);
    Accommodation? GetById(int id);
    void Create(Accommodation accommodation, int ownerId);
    void Update(Accommodation accommodation);
    void Delete(int id);
    void AddReview(int accommodationId, int userId, int rating, string comment);
}
