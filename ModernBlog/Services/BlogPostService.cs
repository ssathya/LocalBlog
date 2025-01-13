using Microsoft.EntityFrameworkCore;
using Models;
using Models.AppSpecific;

namespace ModernBlog.Services;

public class BlogPostService(BlogContext context, ILogger<BlogPostService> logger) : IBlogPostService
{
    private readonly BlogContext context = context;
    private readonly ILogger<BlogPostService> logger = logger;

    public async Task<BlogPost?> GetPostByIdAsync(int id)
    {
        try
        {
            return await context.BlogPosts.FirstOrDefaultAsync(p => p.Id == id);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error obtaining all Blog Posts.\n{ex.Message}");
            return null;
        }
    }

    public async Task<MethodResult> CreateOrUpdateBlogPostAsync(BlogPost post)
    {
        try
        {
            if (post.Id == 0)
            {
                post.CreatedOn = DateTime.UtcNow;
                await context.BlogPosts.AddAsync(post);
            }
            else
            {
                post.ModifiedOn = DateTime.UtcNow;
                context.BlogPosts.Update(post);
            }
            await context.SaveChangesAsync();
            return new MethodResult(true);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error creating or updating Blog Post.\n{ex.Message}");
            return new MethodResult(false, $"Error creating or updating Blog Post.\n{ex.Message}");
        }
    }

    public async Task<MethodResult> DeleteBlogPostAsync(int id)
    {
        try
        {
            var post = await context.BlogPosts.FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return new MethodResult(false, "Blog post not found.");
            }
            context.BlogPosts.Remove(post);
            await context.SaveChangesAsync();
            return new MethodResult(true);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error deleting Blog Post.\n{ex.Message}");
            return new MethodResult(false, $"Error deleting Blog Post.\n{ex.Message}");
        }
    }
}