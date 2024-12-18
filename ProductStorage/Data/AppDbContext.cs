using Microsoft.EntityFrameworkCore;
using ProductStorage.Models;

namespace ProductStorage.Data;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost; Database=product-storage; User ID=postgres; Port=5432; Password=0809;");
    }

    public void ApplyMigrations()
    {
        try
        {   
            Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Bazani yangilashda xatolik yuz berdi: {ex.Message}");
        }
    }
}