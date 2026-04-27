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
}
