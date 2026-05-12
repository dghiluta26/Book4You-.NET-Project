using Project.Data;
using Project.Models;

namespace Project.Repositories;

public class UnavailablePeriodRepository : IUnavailablePeriodRepository
{
    private readonly AppDbContext _context;

    public UnavailablePeriodRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<UnavailablePeriod> GetAll() => _context.UnavailablePeriods.OrderByDescending(p => p.StartDate).ToList();

    public UnavailablePeriod? GetById(int id) => _context.UnavailablePeriods.FirstOrDefault(x => x.Id == id);

    public void Add(UnavailablePeriod period) => _context.UnavailablePeriods.Add(period);

    public void Remove(UnavailablePeriod period) => _context.UnavailablePeriods.Remove(period);

    public void SaveChanges() => _context.SaveChanges();

    public bool HasOverlap(int accommodationId, DateTime startDate, DateTime endDate)
    {
        return _context.UnavailablePeriods.Any(up => up.AccommodationId == accommodationId && up.StartDate < endDate && up.EndDate > startDate)
            || _context.Bookings.Any(b => b.AccommodationId == accommodationId && b.CheckInDate < endDate && b.CheckOutDate > startDate);
    }
}
