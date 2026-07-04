using Project.Models;

namespace Project.Services;

public interface IAccountService
{
    User? GetByEmail(string email);
    User? GetById(int id);
    void UpdateProfile(User user, string firstName, string lastName, string? address, string? profilePictureUrl);

    User? Authenticate(string email, string password);
    User? Register(string email, string firstName, string lastName, string? address, string password);
}
