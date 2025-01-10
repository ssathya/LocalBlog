using Microsoft.EntityFrameworkCore;
using Models;

namespace ModernBlog.Services;

public class BlogCategoryService(BlogContext context, ILogger<BlogCategoryService> logger) : IBlogCategoryService
{
    private readonly BlogContext context = context;
    private readonly ILogger<BlogCategoryService> logger = logger;

    public async Task<List<BlogCategory>?> GetAllCategoriesAsync()
    {
        try
        {
            return await context.BlogCategories
                .AsNoTracking()
                .ToListAsync();
        }
        catch (Exception ex)
        {
            logger.LogError($"Error obtaining all Blogs with categories.\n{ex.Message}");
            return null;
        }
    }
}