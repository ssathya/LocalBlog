using FrontEnd.Extensions;
using FrontEnd.Services;
using Microsoft.AspNetCore.Components;
using Models;

namespace FrontEnd.Components.Pages;

public partial class SaveBlog
{
    [Parameter]
    public int? Id { get; set; }

    [Inject]
    public CategoryService? CategorySvc { get; set; }

    [Inject]
    public ILogger<SaveBlog>? Logger { get; set; }

    [Inject]
    public BlogService? BlogSvc { get; set; }

    private int BlogId => Id ?? 0;
    public BlogPost Blog { get; set; } = new();
    public IEnumerable<Category> Categories = [];

    protected override async Task OnInitializedAsync()
    {
        if (Logger is null)
        {
            throw new ArgumentNullException(nameof(Logger));
        }
        if (CategorySvc is null)
        {
            throw new ArgumentNullException(nameof(CategorySvc));
        }
        if (BlogSvc is null)
        {
            throw new ArgumentNullException(nameof(BlogSvc));
        }
        Categories = await CategorySvc.GetCategoriesAsync();
        if (BlogId > 0)
        {
            var blg = await BlogSvc.GetBlogPostByIdAsync(BlogId);
            if (blg is null)
            {
                Logger.LogError("Blog not found");
                return;
            }
            else
            {
                Blog = blg;
            }
        }
    }

    private async Task<bool> SaveUpdates()
    {
        return await Task.FromResult(true);
    }

    public void UpdateSlug()
    {
        if (Blog.Id == 0)
        {
            Blog.Slug = Blog.Title.ManageSlug();
        }
    }
}