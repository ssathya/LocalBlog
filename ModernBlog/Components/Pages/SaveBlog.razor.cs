using Blazorise.Markdown;
using Microsoft.AspNetCore.Components;
using Models;
using ModernBlog.Services;

namespace ModernBlog.Components.Pages;

public partial class SaveBlog
{
    [Parameter]
    public int? Id { get; set; }

    private int BlogId => Id ?? 0;

    [Inject]
    public IBlogPostService? BlgPstSvc { get; set; }

    [Inject]
    public ICategoryService? CtgSvc { get; set; }

    [Inject]
    public ILogger<SaveBlog>? Logger { get; set; }

    public BlogPost BlogPost { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public Category SelectedCategory { get; set; } = new();
    protected Markdown markdownRef = new();
    private string initialMarkdownValue = string.Empty;
    protected bool hasValueChanged = false;

    [Inject]
    public NavigationManager? NavigationManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (Logger is null)
        {
            throw new ArgumentNullException(nameof(Logger));
        }
        await PopulateExistingBlog();
        await PopulateCategories();
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
            hasValueChanged = true;
            SelectedCategory = Categories.FirstOrDefault(c => c.Id == BlogPost.CategoryId)!;
            return Categories.FirstOrDefault(c => c.Id == BlogPost.CategoryId)!.Name;
        }
    }

    protected Task OnMarkdownValueChanged(string value)
    {
        hasValueChanged = !initialMarkdownValue.Equals(value);
        return Task.CompletedTask;
    }

    protected async Task ButtonClicked(bool isSave)
    {
        if (BlgPstSvc is null)
        {
            throw new ArgumentNullException(nameof(BlgPstSvc));
        }
        if (isSave)
        {
            BlogPost.Content = await markdownRef.GetValueAsync();
            BlogPost.CategoryId = SelectedCategory.Id;
            await BlgPstSvc.CreateOrUpdateBlogPostAsync(BlogPost);
        }
        NavigationManager!.NavigateTo("/manageBlogs");
    }
}