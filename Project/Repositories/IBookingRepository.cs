using Project.Models;

namespace Project.Repositories;

public interface IBookingRepository
{
    Booking? GetById(int id);
    List<Booking> GetByUserId(int userId);
    List<Booking> GetAllForAdmin();
    void Add(Booking booking);
    void SaveChanges();
    bool HasOverlappingBooking(int accommodationId, DateTime checkIn, DateTime checkOut);
    bool HasUserStayed(int userId, int accommodationId);
}
