using FrontEnd.Extensions;
using FrontEnd.Services;
using Microsoft.AspNetCore.Components;
using Models;

namespace FrontEnd.Components.Pages.PageComp;

public partial class CategoryForm
{
    [Parameter]
    public string Title { get; set; } = string.Empty;

    [Parameter]
    public Category? CatToFill { get; set; }

    [Inject]
    public CategoryService? CategoryService { get; set; }

    protected override void OnInitialized()
    {
        CatToFill ??= new();
    }

    public void CancelClicked()
    {
        Console.WriteLine("Cancel clicked");
    }

    private async Task Submit()
    {
        if (CategoryService is null || CatToFill is null)
        {
            return;
        }
        CatToFill.Slug = CatToFill.Slug.ManageSlug();
        var result = await CategoryService.SaveCategoryInDbAsync(CatToFill);
        if ((result.Status == false))
        {
            Console.WriteLine($"Error Saving record to database\n{result.Message}");
        }
    }
}