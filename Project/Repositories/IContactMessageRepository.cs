using Project.Models;

namespace Project.Repositories;

public interface IContactMessageRepository
{
    List<ContactMessage> GetAll();
    ContactMessage? GetById(int id);
    void Add(ContactMessage message);
    void Remove(ContactMessage message);
    void SaveChanges();
}
