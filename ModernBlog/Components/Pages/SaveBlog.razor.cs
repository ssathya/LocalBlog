using Blazorise.Markdown;
using Microsoft.AspNetCore.Components;
using Models;
using ModernBlog.Services;
using System.ComponentModel.DataAnnotations;

namespace ModernBlog.Components.Pages;

[Flags]
public enum ErrorFlags
{
    None = 0,
    NoTitle = 1,
    NoIntroduction = 2,
    NoCategory = 4,
    NoContent = 8
}

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
    protected ErrorFlags errorFlags = ErrorFlags.None;

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
            BlogPost.Content = await markdownRef.GetValueAsync();
            bool isValid = IsContentValid();
            if (!isValid)
            {
                Logger!.LogWarning("Content is not valid");
                return;
            }
            if (BlogPost.IsPublished && BlogPost.PublishedOn == null)
            {
                BlogPost.PublishedOn = DateTime.UtcNow;
            }
            if (!BlogPost.IsPublished)
            {
                BlogPost.PublishedOn = null;
            }
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

    private bool IsContentValid()
    {
        errorFlags = ErrorFlags.None;
        bool isValid = true;
        if (string.IsNullOrEmpty(BlogPost.Title)
            || BlogPost.Title.Length >
            ((Attribute.GetCustomAttribute(BlogPost.GetType().GetProperty(nameof(BlogPost.Title))!,
            typeof(MaxLengthAttribute)) as MaxLengthAttribute)?.Length ?? 255))
        {
            errorFlags |= ErrorFlags.NoTitle;
            isValid = false;
        }
        if (string.IsNullOrEmpty(BlogPost.Introduction)
            || BlogPost.Title.Length >
            ((Attribute.GetCustomAttribute(BlogPost.GetType().GetProperty(nameof(BlogPost.Introduction))!,
            typeof(MaxLengthAttribute)) as MaxLengthAttribute)?.Length ?? 255))
        {
            errorFlags |= ErrorFlags.NoIntroduction;
            isValid = false;
        }
        if (string.IsNullOrEmpty(BlogPost.Content))
        {
            errorFlags |= ErrorFlags.NoContent;
            isValid = false;
        }
        if (SelectedCategory.Id == 0)
        {
            errorFlags |= ErrorFlags.NoCategory;
            isValid = false;
        }
        return isValid;
    }
}