using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _context;

    public BookingRepository(AppDbContext context)
    {
        _context = context;
    }

    public Booking? GetById(int id) => _context.Bookings.FirstOrDefault(b => b.Id == id);

    public List<Booking> GetByUserId(int userId)
    {
        return _context.Bookings
            .Include(b => b.Accommodation)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToList();
    }

    public List<Booking> GetAllForAdmin()
    {
        return _context.Bookings
            .Include(b => b.Accommodation)
            .Include(b => b.User)
            .OrderByDescending(b => b.CreatedAt)
            .ToList();
    }

    public void Add(Booking booking) => _context.Bookings.Add(booking);

    public void SaveChanges() => _context.SaveChanges();

    public bool HasOverlappingBooking(int accommodationId, DateTime checkIn, DateTime checkOut)
    {
        return _context.Bookings.Any(b => b.AccommodationId == accommodationId && b.CheckInDate < checkOut && b.CheckOutDate > checkIn);
    }

    public bool HasUserStayed(int userId, int accommodationId)
    {
        return _context.Bookings.Any(b =>
            b.UserId == userId &&
            b.AccommodationId == accommodationId &&
            b.Status != "Cancelled" &&
            b.CheckOutDate.Date <= DateTime.Today);
    }
}
