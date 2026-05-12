using Project.Models;
using Project.Repositories;

namespace Project.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public User? GetByEmail(string email) => _repository.GetByEmail(email);

    public User? GetById(int id) => _repository.GetById(id);

    public List<User> GetAll() => _repository.GetAll();

    public void Add(User user) => _repository.Add(user);

    public void SaveChanges() => _repository.SaveChanges();
}
