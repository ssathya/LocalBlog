using Microsoft.AspNetCore.Components;
using ModernBlog.Services;

namespace ModernBlog.Components.Pages;

public partial class BlogGenerator
{
    public bool IsGenerating = false;

    protected string ideas = string.Empty;

    [Inject]
    public IAiHandler? AiHandler { get; set; }

    public string? blog { get; set; }

    [Parameter]
    public bool? boolean { get; set; }

    [Inject]
    public ILogger<BlogGenerator>? Logger { get; set; }

    [Parameter]
    public string ParentBlog { get; set; } = string.Empty;

    [Parameter]
    public EventCallback<string> ParentBlogChanged { get; set; }

    [Parameter]
    public string ParentIntroduction { get; set; } = string.Empty;

    [Parameter]
    public EventCallback<string> ParentIntroductionChanged { get; set; }

    protected string? Introduction { get; set; }

    private async Task OnButtonClicked()
    {
        if (string.IsNullOrEmpty(ideas))
        {
            return;
        }
        if (Logger == null)
        {
            throw new Exception("Logger is null");
        }
        if (AiHandler == null)
        {
            Logger.LogCritical("AiHandler is null");
            throw new Exception("AiHandler is null");
        }
        IsGenerating = true;
        Introduction = await AiHandler.GetIntroduction(ideas);
        if (!ParentIntroduction.Equals(Introduction))
        {
            ParentIntroduction = Introduction;
        }
        await ParentIntroductionChanged.InvokeAsync(ParentIntroduction);
        blog = await AiHandler.GetContent(ideas);
        if (!ParentBlog.Equals(blog))
        {
            ParentBlog = blog;
        }
        await ParentBlogChanged.InvokeAsync(ParentBlog);
        StateHasChanged();
        IsGenerating = false;
        return;
    }
}