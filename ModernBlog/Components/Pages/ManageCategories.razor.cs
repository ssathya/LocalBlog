using Blazorise.DataGrid;
using Microsoft.AspNetCore.Components;
using Models;
using ModernBlog.Extensions;
using ModernBlog.Services;

namespace ModernBlog.Components.Pages;

public partial class ManageCategories
{
    [Inject]
    public ICategoryService? CategoryService { get; set; }

    public Category category { get; set; } = new();
    private List<Category> Categories = [];
    private Category selectedCategory = new();
    private DataGridEditMode editMode = DataGridEditMode.Popup;

    protected override async Task OnInitializedAsync()
    {
        Categories = (await CategoryService!.GetCategories()).ToList();
    }

    protected async Task SaveChanges()
    {
        foreach (var category in Categories)
        {
            if (category.Id == 0)
            {
                category.Slug = category.Name.ManageSlug();
                await CategoryService!.SaveCategoryInDb(category);
            }
        }
    }

    protected async Task UpdateRow(Category category)
    {
        await CategoryService!.SaveCategoryInDb(category);
    }
}