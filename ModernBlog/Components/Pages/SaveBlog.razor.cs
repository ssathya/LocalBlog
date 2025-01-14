using Blazorise.Markdown;
using Microsoft.AspNetCore.Components;
using Models;
using ModernBlog.Services;

namespace ModernBlog.Components.Pages;

public partial class SaveBlog
{
    protected Markdown markdownRef = new();

    private string initialMarkdownValue = string.Empty;

    [Inject]
    public IBlogPostService? BlgPstSvc { get; set; }

    public BlogPost BlogPost { get; set; } = new();

    public List<Category> Categories { get; set; } = new();

    [Inject]
    public ICategoryService? CtgSvc { get; set; }

    [Parameter]
    public int? Id { get; set; }

    [Inject]
    public ILogger<SaveBlog>? Logger { get; set; }

    [Inject]
    public NavigationManager? NavigationManager { get; set; }

    public Category SelectedCategory { get; set; } = new();

    private int BlogId => Id ?? 0;

    protected async Task DeleteBlog()
    {
        if (BlgPstSvc is null)
        {
            Logger!.LogCritical("BlgPstSvc is null");
            throw new ArgumentNullException(nameof(BlgPstSvc));
        }
        if (BlogPost.Id > 0)
        {
            await BlgPstSvc.DeleteBlogPostAsync(BlogPost.Id);
        }
        NavigationManager!.NavigateTo("/manageBlogs");
    }

    protected string GetSelectedCategoryName()
    {
        if (Categories.FirstOrDefault(c => c.Id == BlogPost.CategoryId) is null)
        {
            return "Select a category";
        }
        else
        {
            if (BlogPost.CategoryId == SelectedCategory.Id)
            {
                return SelectedCategory.Name;
            }

            SelectedCategory = Categories.FirstOrDefault(c => c.Id == BlogPost.CategoryId)!;
            return Categories.FirstOrDefault(c => c.Id == BlogPost.CategoryId)!.Name;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        if (Logger is null)
        {
            throw new ArgumentNullException(nameof(Logger));
        }
        await PopulateExistingBlog();
        await PopulateCategories();
    }

    protected async Task SaveButtonClicked(bool isSave)
    {
        if (BlgPstSvc is null)
        {
            throw new ArgumentNullException(nameof(BlgPstSvc));
        }
        if (isSave)
        {
            if (BlogPost.IsPublished && BlogPost.PublishedOn == null)
            {
                BlogPost.PublishedOn = DateTime.UtcNow;
            }
            if (!BlogPost.IsPublished)
            {
                BlogPost.PublishedOn = null;
            }
            BlogPost.Content = await markdownRef.GetValueAsync();
            BlogPost.CategoryId = SelectedCategory.Id;
            await BlgPstSvc.CreateOrUpdateBlogPostAsync(BlogPost);
        }
        NavigationManager!.NavigateTo("/manageBlogs");
    }

    private async Task PopulateCategories()
    {
        if (CtgSvc is null)
        {
            throw new ArgumentNullException(nameof(CtgSvc));
        }
        try
        {
            Categories = await CtgSvc.GetCategories();
        }
        catch (Exception ex)
        {
            Logger!.LogError(ex, "Error getting categories");
            NavigationManager!.NavigateTo("/errorPage");
        }
    }

    private async Task PopulateExistingBlog()
    {
        if (BlgPstSvc is null)
        {
            throw new ArgumentNullException(nameof(BlgPstSvc));
        }
        if (BlogId > 0)
        {
            Logger!.LogInformation("BlogId: {BlogId}", BlogId);
            try
            {
                BlogPost = (await BlgPstSvc.GetPostByIdAsync(BlogId)) ?? new();
                initialMarkdownValue = BlogPost.Content ?? string.Empty;
                await markdownRef.SetValueAsync(initialMarkdownValue);
            }
            catch (Exception ex)
            {
                Logger!.LogError(ex, "Error getting blog post by id: {BlogId}", BlogId);
                NavigationManager!.NavigateTo("/errorPage");
            }
        }
    }
}