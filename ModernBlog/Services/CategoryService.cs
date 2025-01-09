using Microsoft.EntityFrameworkCore;
using Models;
using Models.AppSpecific;

namespace ModernBlog.Services;

public class CategoryService(BlogContext context, ILogger<CategoryService> logger) : ICategoryService
{
    private readonly BlogContext context = context;
    private readonly ILogger<CategoryService> logger = logger;

    public async Task<List<Category>> GetCategories()
    {
        return await context.Categories
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Category?> GetCategoryByName(string name)
    {
        try
        {
            return await context.Categories
               .AsNoTracking()
               .FirstOrDefaultAsync(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
        }
        catch (Exception ex)
        {
            logger.LogError($"Error obtaining category by name.\n{ex.Message}");
            return null;
        }
    }

    public async Task<Category?> GetCategoryBySlug(string slug)
    {
        try
        {
            return await context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Slug.ToLower().Trim() == slug.ToLower().Trim());
        }
        catch (Exception ex)
        {
            logger.LogError($"Error obtaining category by slug.\n{ex.Message}");
            return null;
        }
    }

    public async Task<MethodResult> SaveCategoryInDb(Category category)
    {
        try
        {
            if (category.Id == 0)
            {
                context.Categories.Add(category);
            }
            else
            {
                context.Categories.Update(category);
            }
            await context.SaveChangesAsync();
            return new MethodResult(true, "Category saved successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError($"Error saving category.\n{ex.Message}");
            return new MethodResult(false, "Error saving category.");
        }
    }

    public async Task<MethodResult> DeleteCategoryFromDb(Category category)
    {
        try
        {
            var tmpCategory = await context.Categories
                .FirstOrDefaultAsync(c => c.Id == category.Id);
            if (tmpCategory is not null)
            {
                context.Categories.Remove(tmpCategory);
            }
            await context.SaveChangesAsync();
            return new MethodResult(true, "Category deleted successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError($"Error deleting category.\n{ex.Message}");
            return new MethodResult(false, "Error deleting category.");
        }
    }
}