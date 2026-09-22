using CompraCerca.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CompraCerca.API.Data;

public class CompraCercaDbContext : DbContext
{
    public CompraCercaDbContext(DbContextOptions<CompraCercaDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Product> Products { get; set; }
}