using FrontEnd.Services;
using Microsoft.AspNetCore.Components;
using Models;

namespace FrontEnd.Components.Pages;

public partial class ManageBlogs
{
    private IEnumerable<BlogPost> Blogs = [];
    private bool IsLoading = false;

    [Inject]
    public BlogService? BlogService { get; set; }

    [Inject]
    public ILogger<ManageBlogs>? Logger { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (Logger is null) return;
        if (BlogService is null)
        {
            Logger.LogError("BlogService is null.");
            return;
        }

        IsLoading = true;
        try
        {
            Blogs = await BlogService.GetBlogsAsync();
            if (Blogs is null || !Blogs.Any())
            {
                Logger.LogInformation("No blogs found.");
                Blogs = [];
            }
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error obtaining blogs.\n{ex.Message}");
            Blogs = [];
        }
        finally
        {
            IsLoading = false;
        }
        return;
    }
}