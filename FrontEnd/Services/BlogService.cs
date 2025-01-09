using Microsoft.EntityFrameworkCore;
using Models;
using Models.AppSpecific;

namespace FrontEnd.Services;

public class BlogService(BlogContext context, ILogger<BlogService> logger)
{
    private readonly BlogContext context = context;
    private readonly ILogger<BlogService> logger = logger;

    public async Task<MethodResult> DeleteBlogAsync(BlogPost blog)
    {
        try
        {
            context.BlogPosts.Remove(blog);
            await context.SaveChangesAsync();
            return new MethodResult(true, "Blog deleted successfully");
        }
        catch (Exception ex)
        {
            logger.LogError($"Error deleting blog from database.\n{ex.Message}");
            return new MethodResult(false, "Error deleting blog from database");
        }
    }

    public async Task<BlogPost?> GetBlogPostByIdAsync(int id)
    {
        try
        {
            return await context.BlogPosts
                .Include(b => b.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error obtaining blog from BlogPosts table.\n{ex.Message}");
            return null;
        }
    }

    public async Task<IEnumerable<BlogPost>> GetBlogsAsync()
    {
        try
        {
            return await context.BlogPosts
                .Include(b => b.Category)
                .AsNoTracking()
                .OrderByDescending(b => b.CreatedOn)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            logger.LogError($"Error obtaining values from BlogPosts table.\n{ex.Message}");
            return [];
        }
    }

    public async Task<IEnumerable<BlogPost>> GetBlogsByCategoryAsync(int categoryId)
    {
        try
        {
            return await context.BlogPosts
                .Include(b => b.Category)
                .AsNoTracking()
                .Where(b => b.CategoryId == categoryId)
                .OrderByDescending(b => b.CreatedOn)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            logger.LogError($"Error obtaining blogs from BlogPosts table.\n{ex.Message}");
            return [];
        }
    }

    public async Task<MethodResult> SaveBlogInDbAsync(BlogPost blog)
    {
        try
        {
            if (blog.Id == 0)
            {
                await context.BlogPosts.AddAsync(blog);
            }
            else
            {
                context.BlogPosts.Update(blog);
            }
            await context.SaveChangesAsync();
            return new MethodResult(true, "Blog saved successfully");
        }
        catch (Exception ex)
        {
            logger.LogError($"Error saving blog to database.\n{ex.Message}");
            return new MethodResult(false, "Error saving blog to database");
        }
    }
}