using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Repositories;

public class AccommodationRepository : IAccommodationRepository
{
    private readonly AppDbContext _context;

    public AccommodationRepository(AppDbContext context)
    {
        _context = context;
    }

    public Accommodation? GetById(int id) => _context.Accommodations.FirstOrDefault(a => a.Id == id);

    public List<Accommodation> GetAll() => _context.Accommodations.ToList();

    public List<Accommodation> GetAllForAdmin() => _context.Accommodations.OrderByDescending(a => a.CreatedAt).ToList();

    public void Add(Accommodation accommodation) => _context.Accommodations.Add(accommodation);

    public void Remove(Accommodation accommodation) => _context.Accommodations.Remove(accommodation);

    public void SaveChanges() => _context.SaveChanges();

    public List<Amenity> GetAmenities(int accommodationId)
    {
        return _context.AccommodationAmenities
            .Where(aa => aa.AccommodationId == accommodationId)
            .Join(_context.Amenities, aa => aa.AmenityId, a => a.Id, (aa, a) => a)
            .ToList();
    }

    public List<AccommodationImage> GetImages(int accommodationId)
    {
        return _context.AccommodationImages
            .Where(i => i.AccommodationId == accommodationId)
            .OrderBy(i => i.DisplayOrder)
            .ToList();
    }

    public List<Review> GetReviews(int accommodationId)
    {
        return _context.Reviews
            .Include(r => r.User)
            .Where(r => r.AccommodationId == accommodationId)
            .OrderByDescending(r => r.CreatedAt)
            .ToList();
    }

    
    public List<Accommodation> SearchAvailable(string? location, string? type, decimal? maxPrice, int? guests, bool hasDates, DateTime checkIn, DateTime checkOut)
    {
    var query = _context.Accommodations.AsQueryable();

    if (!string.IsNullOrWhiteSpace(location))
    {
        var loc = location.Trim().ToLower();
        query = query.Where(a => a.Location.ToLower().Contains(loc));
    }

    if (!string.IsNullOrWhiteSpace(type))
    {
        query = query.Where(a => a.Type.ToLower() == type.ToLower());
    }

    if (maxPrice.HasValue)
    {
        query = query.Where(a => a.PricePerNight <= maxPrice.Value);
    }

    if (guests.HasValue)
    {
        query = query.Where(a => a.Capacity >= guests.Value);
    }

    if (hasDates)
    {
        query = query.Where(a =>
            !_context.Bookings.Any(b =>
                b.AccommodationId == a.Id &&
                b.Status != "Cancelled" &&
                b.CheckInDate < checkOut &&
                b.CheckOutDate > checkIn)
            && !_context.UnavailablePeriods.Any(up =>
                up.AccommodationId == a.Id &&
                up.StartDate < checkOut &&
                up.EndDate > checkIn));
    }

    return query.ToList();
    }

}
