using Project.Models;

namespace Project.Repositories;

public interface IUserRepository
{
    User? GetByEmail(string email);
    User? GetById(int id);
    List<User> GetAll();
    void Add(User user);
    void SaveChanges();
}
