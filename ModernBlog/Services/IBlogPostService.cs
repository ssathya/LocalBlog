using Models;
using Models.AppSpecific;

namespace ModernBlog.Services;

public interface IBlogPostService
{
    Task<MethodResult> CreateOrUpdateBlogPostAsync(BlogPost post);

    Task<MethodResult> DeleteBlogPostAsync(int id);

    Task<BlogPost?> GetPostByIdAsync(int id);
}