using Blazorise.DataGrid;
using Humanizer;
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
    private const int introductionMaxLength = 256;

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
        foreach (var blogCategory in BlogCategories)
        {
            blogCategory.Introduction = blogCategory.Introduction.Truncate(introductionMaxLength);
        }
    }

    protected void EditBlog(DataGridRowMouseEventArgs<BlogCategory> blogCategoryEvent)
    {
        int blogCategoryId = blogCategoryEvent.Item.BlogId;
        if (NavigationManager is null)
        {
            Logger!.LogError("NavigationManager is null.");
            throw new ArgumentNullException(nameof(NavigationManager));
        }
        NavigationManager.NavigateTo($"/SaveBlog/{blogCategoryId}");
    }
}