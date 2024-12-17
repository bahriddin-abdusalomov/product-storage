using Microsoft.EntityFrameworkCore;

using ProductStorage.Models;
using System.Collections.Generic;

namespace ProductStorage.Data;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost; Database=product-storage; User ID=postgres; Port=5432; Password=0809;");
    }
}