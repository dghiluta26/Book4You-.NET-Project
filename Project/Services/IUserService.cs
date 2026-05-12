using Project.Models;

namespace Project.Services;

public interface IUserService
{
    User? GetByEmail(string email);
    User? GetById(int id);
    List<User> GetAll();
    void Add(User user);
    void SaveChanges();
}
