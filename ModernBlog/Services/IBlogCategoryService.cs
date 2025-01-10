using Models;

namespace ModernBlog.Services;
public interface IBlogCategoryService
{
    Task<List<BlogCategory>?> GetAllCategoriesAsync();
}