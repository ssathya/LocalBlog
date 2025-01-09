using Models;
using Models.AppSpecific;

namespace ModernBlog.Services;
public interface ICategoryService
{
    Task<MethodResult> DeleteCategoryFromDb(Category category);
    Task<List<Category>> GetCategories();
    Task<Category?> GetCategoryByName(string name);
    Task<Category?> GetCategoryBySlug(string slug);
    Task<MethodResult> SaveCategoryInDb(Category category);
}