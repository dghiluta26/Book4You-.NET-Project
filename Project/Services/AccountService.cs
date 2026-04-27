using Project.Models;
using Project.Repositories;

namespace Project.Services;

public class AccountService : IAccountService
{
    private readonly IUserRepository _userRepository;

    public AccountService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public User? GetByEmail(string email) => _userRepository.GetByEmail(email);

    public User? GetById(int id) => _userRepository.GetById(id);

    public void UpdateProfile(User user, string firstName, string lastName, string? address)
    {
        user.FirstName = firstName;
        user.LastName = lastName;
        user.Address = address;
        _userRepository.SaveChanges();
    }
}
