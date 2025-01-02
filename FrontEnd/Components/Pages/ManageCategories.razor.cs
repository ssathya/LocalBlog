using FrontEnd.Components.Pages.PageComp;
using FrontEnd.Services;
using Microsoft.AspNetCore.Components;
using Models;

namespace FrontEnd.Components.Pages;

public partial class ManageCategories
{
    [Inject]
    public CategoryService? CategoryService { get; set; }

    private IEnumerable<Category>? categories;
    private bool loading = false;
    private CategoryForm? categoryForm;
    private Category? CategoryToEdit;

    protected override void OnInitialized()
    {
        categoryForm ??= new();
        CategoryToEdit = null;
    }

    protected override async Task OnInitializedAsync()
    {
        loading = true;
        if (CategoryService is not null)
        {
            categories = await CategoryService.GetCategoriesAsync();
        }
        categories ??= [];
        loading = false;
    }
}