using Project.Models;

namespace Project.Services;

public interface IPdfService
{
    // Generates the confirmation PDF and returns its relative web URL (e.g. /pdfs/bookings/booking_1.pdf).
    // Returns null if generation fails.
    string? GenerateBookingPdf(Booking booking);
}
