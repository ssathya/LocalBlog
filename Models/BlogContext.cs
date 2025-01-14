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

/*
 * The SQL that is used to create the view is as follows:
 * -- public."BlogCategories" source

CREATE OR REPLACE VIEW public."BlogCategories"
AS SELECT bp."Id" AS "BlogId",
    bp."Title",
    bp."Introduction",
    c."Name",
    bp."IsPublished",
    bp."CreatedOn",
    bp."PublishedOn",
    bp."ModifiedOn"
   FROM "BlogPosts" bp,
    "Categories" c
  WHERE c."Id" = bp."CategoryId";

 */