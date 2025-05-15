using Microsoft.EntityFrameworkCore;
using TestLibrus.Models;

namespace TestLibrus.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Item> Items { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public AppDbContext() : base(new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlite("Data Source=app.db")
        .Options)
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite("Data Source=app.db");   
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(p => p.Items)
            .WithOne(i => i.User)
            .HasForeignKey(i => i.UserId);
    }

    public void EnsureCreated()
    {
        using (var context = new AppDbContext())
        {
            context.Database.EnsureCreated();
        }
    }
}