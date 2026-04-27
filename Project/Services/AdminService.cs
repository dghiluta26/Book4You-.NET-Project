using Project.Models;
using Project.Repositories;

namespace Project.Services;

public class AdminService : IAdminService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnavailablePeriodRepository _unavailablePeriodRepository;
    private readonly IAccommodationRepository _accommodationRepository;

    public AdminService(
        IBookingRepository bookingRepository,
        IUserRepository userRepository,
        IUnavailablePeriodRepository unavailablePeriodRepository,
        IAccommodationRepository accommodationRepository)
    {
        _bookingRepository = bookingRepository;
        _userRepository = userRepository;
        _unavailablePeriodRepository = unavailablePeriodRepository;
        _accommodationRepository = accommodationRepository;
    }

    public List<Booking> GetBookings() => _bookingRepository.GetAllForAdmin();

    public void CancelBooking(int id)
    {
        var booking = _bookingRepository.GetById(id) ?? throw new InvalidOperationException("Booking not found.");
        booking.Status = "Cancelled";
        _bookingRepository.SaveChanges();
    }

    public List<User> GetUsers() => _userRepository.GetAll();

    public void ToggleRole(int id)
    {
        var user = _userRepository.GetById(id) ?? throw new InvalidOperationException("User not found.");
        user.Role = user.Role == "Admin" ? "User" : "Admin";
        _userRepository.SaveChanges();
    }

    public List<UnavailablePeriod> GetUnavailablePeriods() => _unavailablePeriodRepository.GetAll();

    public CreateUnavailablePeriodViewModel GetCreateUnavailablePeriodViewModel()
    {
        return new CreateUnavailablePeriodViewModel
        {
            Accommodations = _accommodationRepository.GetAll().OrderBy(a => a.Title).ToList()
        };
    }

    public void CreateUnavailablePeriod(UnavailablePeriod unavailablePeriod, string reason)
    {
        if (unavailablePeriod.StartDate >= unavailablePeriod.EndDate)
        {
            throw new ArgumentException("End date must be after start date.");
        }

        _unavailablePeriodRepository.Add(unavailablePeriod);
        _unavailablePeriodRepository.SaveChanges();
    }

    public void DeleteUnavailablePeriod(int id)
    {
        var period = _unavailablePeriodRepository.GetById(id) ?? throw new InvalidOperationException("Unavailable period not found.");
        _unavailablePeriodRepository.Remove(period);
        _unavailablePeriodRepository.SaveChanges();
    }
}
