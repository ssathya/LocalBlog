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

    [Parameter]
    public EventCallback<ManageCategories> ChangeSubmitted { get; set; }

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

    private void NameBlurred()
    {
        if (CatToFill is not null && CatToFill.Id == 0)
        {
            CatToFill.Slug = CatToFill.Name.ManageSlug();
        }
    }

    private async Task Submit()
    {
        if (CategoryService is null || CatToFill is null || string.IsNullOrEmpty(CatToFill.Name))
        {
            return;
        }
        var existingCat = await CategoryService.GetCategoryByNameAsync(CatToFill.Name);
        if (CatToFill.Id == 0)
        {
            existingCat ??= await CategoryService.GetCategoryBySlugAsync(CatToFill.Slug);
        }
        if (existingCat is not null)
        {
            if (existingCat.Name == CatToFill.Name)
            {
                Console.WriteLine($"Category with name {CatToFill.Name} already exists");
            }
            else
            {
                Console.WriteLine($"Category with slug {CatToFill.Slug} already exists");
            }
            return;
        }
        CatToFill.Slug = CatToFill.Slug.ManageSlug();
        var result = await CategoryService.SaveCategoryInDbAsync(CatToFill);
        if ((result.Status == false))
        {
            Console.WriteLine($"Error Saving record to database\n{result.Message}");
        }
        else
        {
            await ChangeSubmitted.InvokeAsync();
        }
    }
}