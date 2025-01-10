using Blazorise.DataGrid;
using Microsoft.AspNetCore.Components;
using Models;
using ModernBlog.Services;

namespace ModernBlog.Components.Pages;

public partial class ManageBlogs
{
    [Inject]
    public IBlogCategoryService? BlogCategoryService { get; set; }

    [Inject]
    public ILogger<ManageBlogs>? Logger { get; set; }

    [Inject]
    public NavigationManager? NavigationManager { get; set; }

    private List<BlogCategory> BlogCategories = [];
    private BlogCategory selectedBlogCategory = new();

    protected override async Task OnInitializedAsync()
    {
        if (Logger is null)
        {
            throw new ArgumentNullException(nameof(Logger));
        }
        if (BlogCategoryService is null)
        {
            Logger.LogError("BlogCategoryService is null.");
            throw new ArgumentNullException(nameof(BlogCategoryService));
        }
        BlogCategories = (await BlogCategoryService.GetAllCategoriesAsync()) ?? [];
    }

    protected void EditBlog(DataGridRowMouseEventArgs<BlogCategory> blogCategoryevent)
    {
        int blogCategoryId = blogCategoryevent.Item.BlogId;
        if (NavigationManager is null)
        {
            Logger!.LogError("NavigationManager is null.");
            throw new ArgumentNullException(nameof(NavigationManager));
        }
        NavigationManager.NavigateTo($"/SaveBlog/{blogCategoryId}");
    }
}