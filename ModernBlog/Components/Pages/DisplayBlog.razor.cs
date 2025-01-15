using Microsoft.AspNetCore.Components;
using Models;
using ModernBlog.Services;

namespace ModernBlog.Components.Pages;

public partial class DisplayBlog
{
    [Parameter]
    public int? Id { get; set; }

    [Inject]
    public IBlogPostService? PostService { get; set; }

    [Inject]
    public ILogger<DisplayBlog>? Logger { get; set; }

    protected string markdownHtml = "<h1>Parameter error</h1>";
    protected BlogPost BlgPst = new();

    protected override async Task OnInitializedAsync()
    {
        if (Logger is null)
        {
            throw new Exception("Logger is null");
        }
        if (PostService is null)
        {
            throw new Exception("PostService is null");
        }
        if (Id is null || Id == 0)
        {
            Logger.LogError("Id is null or 0");
            return;
        }
        try
        {
            BlgPst = (await PostService.GetPostByIdAsync(Id ?? 0)) ?? new();
            if (BlgPst.Id == 0)
            {
                return;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting post");
            return;
        }
        markdownHtml = Markdig.Markdown.ToHtml(BlgPst.Content);
    }

    protected string FooterGenerator()
    {
        DateTime refDate = BlgPst.ModifiedOn ?? BlgPst.CreatedOn;
        return $"Created/Modified on{refDate:MM/dd/yyyy} &emsp; " +
            $"Created by {BlgPst.UserId} &emsp;" +
            $" Published on {BlgPst.PublishedOn:MM/dd/yyyy}";
    }
}