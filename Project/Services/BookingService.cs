using Project.Models;
using Project.Repositories;

namespace Project.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IAccommodationRepository _accommodationRepository;
    private readonly IUnavailablePeriodRepository _unavailablePeriodRepository;

    public BookingService(
        IBookingRepository bookingRepository,
        IAccommodationRepository accommodationRepository,
        IUnavailablePeriodRepository unavailablePeriodRepository)
    {
        _bookingRepository = bookingRepository;
        _accommodationRepository = accommodationRepository;
        _unavailablePeriodRepository = unavailablePeriodRepository;
    }

    public List<Booking> GetForUser(int userId) => _bookingRepository.GetByUserId(userId);

    public List<Booking> GetForAdmin() => _bookingRepository.GetAllForAdmin();

    public (bool Success, string Message) Book(int accommodationId, int userId, string? checkIn, string? checkOut, int? guests)
    {
        var accommodation = _accommodationRepository.GetById(accommodationId);
        if (accommodation == null)
        {
            return (false, "Accommodation not found.");
        }

        if (string.IsNullOrEmpty(checkIn) || string.IsNullOrEmpty(checkOut) || !DateTime.TryParse(checkIn, out var requestedCheckIn) || !DateTime.TryParse(checkOut, out var requestedCheckOut))
        {
            return (false, "Please provide valid check-in and check-out dates.");
        }

        if (requestedCheckIn.Date >= requestedCheckOut.Date)
        {
            return (false, "Check-out must be after check-in.");
        }

        if (requestedCheckIn.Date < DateTime.Today)
        {
            return (false, "Check-in cannot be in the past.");
        }

        var numGuests = guests ?? 1;
        if (numGuests < 1 || numGuests > accommodation.Capacity)
        {
            return (false, $"Number of guests must be between 1 and {accommodation.Capacity}.");
        }

        var hasOverlap = _bookingRepository.HasOverlappingBooking(accommodationId, requestedCheckIn, requestedCheckOut)
            || _unavailablePeriodRepository.HasOverlap(accommodationId, requestedCheckIn, requestedCheckOut);

        if (hasOverlap)
        {
            return (false, "The accommodation is not available for the selected dates.");
        }

        var nights = (requestedCheckOut.Date - requestedCheckIn.Date).Days;
        if (nights <= 0)
        {
            nights = 1;
        }

        var booking = new Booking
        {
            UserId = userId,
            AccommodationId = accommodationId,
            CheckInDate = requestedCheckIn.Date,
            CheckOutDate = requestedCheckOut.Date,
            Guests = numGuests,
            TotalPrice = accommodation.PricePerNight * nights,
            Status = "Pending",
            CreatedAt = DateTime.Now
        };

        _bookingRepository.Add(booking);
        _bookingRepository.SaveChanges();

        return (true, "Booking created.");
    }

    public void Cancel(int id)
    {
        var booking = _bookingRepository.GetById(id);
        if (booking == null)
        {
            throw new InvalidOperationException("Booking not found.");
        }

        booking.Status = "Cancelled";
        _bookingRepository.SaveChanges();
    }
}
