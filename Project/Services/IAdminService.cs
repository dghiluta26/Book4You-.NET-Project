using Project.Models;

namespace Project.Services;

public interface IAdminService
{
    List<Booking> GetBookings();
    void CancelBooking(int id);
    List<User> GetUsers();
    void ToggleRole(int id);
    List<UnavailablePeriod> GetUnavailablePeriods();
    CreateUnavailablePeriodViewModel GetCreateUnavailablePeriodViewModel();
    void CreateUnavailablePeriod(UnavailablePeriod unavailablePeriod, string reason);
    void DeleteUnavailablePeriod(int id);
}
