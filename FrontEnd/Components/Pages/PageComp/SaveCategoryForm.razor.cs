using FrontEnd.Services;
using Microsoft.AspNetCore.Components;
using Models;

namespace FrontEnd.Components.Pages.PageComp;

public partial class SaveCategoryForm
{
    private Category categoryModel = new();

    [Inject]
    public CategoryService? CategoryService { get; set; }

    [Inject]
    public Logger<SaveCategoryForm>? logger { get; set; }

    public async Task SaveCategoryAsync()
    {
        if (CategoryService == null)
        {
            logger!.LogError("CategoryService is null");
            return;
        }
        var saveResult = await CategoryService.SaveCategoryInDbAsync(categoryModel);
        if (saveResult.Status == false)
        {
            logger!.LogError("Error saving category");
            logger!.LogError(saveResult.Message);
        }
    }
}