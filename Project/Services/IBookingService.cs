using Project.Models;

namespace Project.Services;

public interface IBookingService
{
    List<Booking> GetForUser(int userId);
    List<Booking> GetForAdmin();
    BookingCheckoutViewModel? GetCheckoutPreview(int accommodationId, string? checkIn, string? checkOut, int? guests);
    (bool Success, string Message) Book(int accommodationId, int userId, string? checkIn, string? checkOut, int? guests);
    void Cancel(int id);
}
