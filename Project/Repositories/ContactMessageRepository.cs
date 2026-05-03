using Project.Data;
using Project.Models;

namespace Project.Repositories;

public class ContactMessageRepository : IContactMessageRepository
{
    private readonly AppDbContext _context;

    public ContactMessageRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<ContactMessage> GetAll() => _context.ContactMessages.OrderByDescending(m => m.CreatedAt).ToList();

    public ContactMessage? GetById(int id) => _context.ContactMessages.FirstOrDefault(m => m.Id == id);

    public void Add(ContactMessage message) => _context.ContactMessages.Add(message);

    public void Remove(ContactMessage message) => _context.ContactMessages.Remove(message);

    public void SaveChanges() => _context.SaveChanges();
}
