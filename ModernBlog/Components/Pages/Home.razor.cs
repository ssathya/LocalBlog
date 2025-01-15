using Blazorise.DataGrid;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.OutputCaching;
using Models;
using ModernBlog.Services;

namespace ModernBlog.Components.Pages;

[OutputCache]
public partial class Home
{
    protected List<BlogCategory> blogPosts = [];
    protected BlogCategory selectedBlog = new();

    [Inject]
    public IBlogCategoryService? BlogCatSvc { get; set; }

    [Inject]
    public ILogger<Home>? Logger { get; set; }

    [Inject]
    public NavigationManager? NavMgr { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (Logger is null)
        {
            throw new ArgumentNullException(nameof(Logger));
        }
        if (BlogCatSvc is null)
        {
            throw new ArgumentNullException(nameof(BlogCatSvc));
        }
        if (NavMgr is null)
        {
            throw new ArgumentNullException(nameof(NavMgr));
        }
        try
        {
            blogPosts = (await BlogCatSvc.GetAllCategoriesAsync()) ?? [];
            blogPosts = (from blogPost in blogPosts
                         where blogPost.IsPublished
                         select blogPost)
                        .ToList();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting blog posts");
        }
    }

    protected void DisplayBlog(DataGridRowMouseEventArgs<BlogCategory> blogCategoryEvent)
    {
        int blogCategoryId = blogCategoryEvent.Item.BlogId;
        NavMgr?.NavigateTo($"/displayBlog/{blogCategoryId}");
    }
}