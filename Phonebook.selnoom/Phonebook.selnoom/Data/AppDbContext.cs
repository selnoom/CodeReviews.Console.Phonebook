using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Phonebook.selnoom.Models;

namespace Phonebook.selnoom.Data;

public class AppDbContext : DbContext
{
    public AppDbContext()
            : base(new DbContextOptionsBuilder<AppDbContext>()
                   .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=PhonebookDb;Trusted_Connection=True;MultipleActiveResultSets=true")
                   .Options)
    {
    }

    [ActivatorUtilitiesConstructor]
    public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
    {
    }

    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id)
                .ValueGeneratedOnAdd();
            entity.HasIndex(c => c.Name)
                .IsUnique();
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id)
                .ValueGeneratedOnAdd();
            entity.HasIndex(c => c.Name);
            entity.HasIndex(c => c.Email);
            entity.HasIndex(c => c.PhoneNumber)
                .IsUnique();
            entity.HasOne(c => c.Category)
                .WithMany(c => c.Contacts)  
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

