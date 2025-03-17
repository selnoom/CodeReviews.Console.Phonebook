using Microsoft.EntityFrameworkCore;

namespace Phonebook.selnoom.Data;

static class DatabaseInitializer
{
    public static void Initialize(AppDbContext dbContext)
    {
        dbContext.Database.Migrate();
    }
}
