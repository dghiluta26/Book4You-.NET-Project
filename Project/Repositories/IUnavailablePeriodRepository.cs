using Project.Models;

namespace Project.Repositories;

public interface IUnavailablePeriodRepository
{
    List<UnavailablePeriod> GetAll();
    UnavailablePeriod? GetById(int id);
    void Add(UnavailablePeriod period);
    void Remove(UnavailablePeriod period);
    void SaveChanges();
    bool HasOverlap(int accommodationId, DateTime startDate, DateTime endDate);
}
