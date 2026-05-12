using Project.Data;
using Project.Models;

namespace Project.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public User? GetByEmail(string email) => _context.Users.FirstOrDefault(u => u.Email == email);

    public User? GetById(int id) => _context.Users.FirstOrDefault(u => u.Id == id);

    public List<User> GetAll() => _context.Users.OrderBy(u => u.Email).ToList();

    public void Add(User user) => _context.Users.Add(user);

    public void SaveChanges() => _context.SaveChanges();
}
