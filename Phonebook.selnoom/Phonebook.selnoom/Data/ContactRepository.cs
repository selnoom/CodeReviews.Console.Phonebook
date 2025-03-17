using Microsoft.EntityFrameworkCore;
using Phonebook.selnoom.Models;

namespace Phonebook.selnoom.Data;

public class ContactRepository
{
    private AppDbContext _dbContext;
    public ContactRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<List<Contact>> GetContacts()
    {
        return await _dbContext.Contacts.ToListAsync();
    }
    public async Task CreateContact(string? name, string? email, string phoneNumber)
    {
        Contact contact = new Contact
        {
            Name = name,
            Email = email,
            PhoneNumber = phoneNumber
        };
        await _dbContext.Contacts.AddAsync(contact);
        await _dbContext.SaveChangesAsync();
    }

    public async Task CreateContact(string? name, string? email, string phoneNumber, int? categoryId = null)
    {
        Contact contact = new Contact
        {
            Name = name,
            Email = email,
            PhoneNumber = phoneNumber,
            CategoryId = categoryId
        };
        await _dbContext.Contacts.AddAsync(contact);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateContact(int id, string? name, string? email, string phoneNumber, int? categoryId)
    {
        await _dbContext.Contacts
            .Where(x => x.Id == id)
            .ExecuteUpdateAsync(setters => setters
            .SetProperty(c => c.Name, name)
            .SetProperty(c => c.Email, email)
            .SetProperty(c => c.PhoneNumber, phoneNumber)
            .SetProperty(c => c.CategoryId, categoryId));
    }

    public async Task DeleteContact(int id)
    {
        await _dbContext.Contacts.Where(x => x.Id == id).ExecuteDeleteAsync();
    }
}
