using Microsoft.EntityFrameworkCore;
using Phonebook.selnoom.Models;

namespace Phonebook.selnoom.Data;

public class CategoryRepository
{
    private AppDbContext _dbContext;
    public CategoryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<List<Category>> GetCategories()
    {
        return await _dbContext.Categories.ToListAsync();
    }

    public async Task CreateCategory(string name)
    {
        Category category = new Category
        {
            Name = name
        };
        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateCategory(int id, string name)
    {
        await _dbContext.Categories
            .Where(x => x.Id == id)
            .ExecuteUpdateAsync(setters => setters.SetProperty(c => c.Name, name));
    }

    public async Task DeleteCategory(int id)
    {
        await _dbContext.Categories.Where(x => x.Id == id).ExecuteDeleteAsync();
    }
}
