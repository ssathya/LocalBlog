using Microsoft.EntityFrameworkCore;

namespace Models;

public class BlogContext : DbContext
{
    public BlogContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<BlogPost> BlogPosts { get; set; }
    public DbSet<BlogCategory> BlogCategories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
#if DEBUG
        optionsBuilder.LogTo(Console.WriteLine);
#endif
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BlogCategory>(c =>
        {
            c.HasNoKey();
            c.ToView("BlogCategories");
        });
        base.OnModelCreating(modelBuilder);
    }
}