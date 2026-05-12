using Project.Models;
using Project.Repositories;

namespace Project.Services;

public class ContactService : IContactService
{
    private readonly IContactMessageRepository _repository;

    public ContactService(IContactMessageRepository repository)
    {
        _repository = repository;
    }

    public void Submit(ContactMessage message)
    {
        message.Name = message.Name.Trim();
        message.Email = message.Email.Trim().ToLowerInvariant();
        message.Subject = message.Subject.Trim();
        message.Message = message.Message.Trim();
        message.CreatedAt = DateTime.UtcNow;
        message.IsRead = false;

        _repository.Add(message);
        _repository.SaveChanges();
    }
}
