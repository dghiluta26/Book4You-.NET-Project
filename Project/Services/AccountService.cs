using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
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

    public void UpdateProfile(User user, string firstName, string lastName, string? address, string? profilePictureUrl)
    {
        user.FirstName = firstName;
        user.LastName = lastName;
        user.Address = address;
        user.ProfilePictureUrl = profilePictureUrl;
        _userRepository.SaveChanges();
    }
    
    public User? Authenticate(string email, string password)
    {
        var user = _userRepository.GetByEmail(email);
        if(user == null)
        {
            return null;
        }

        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user, user.Password, password);

        return result == PasswordVerificationResult.Failed? null : user;
    }

    public User? Register(string email, string firstName, string lastName, string? address, string password)
    {
        var user = new User
        {
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Address = address,

        };

        var hasher = new PasswordHasher<User>();
        user.Password = hasher.HashPassword(user, password);

        _userRepository.Add(user);
        _userRepository.SaveChanges();


        return user;
    }
}
