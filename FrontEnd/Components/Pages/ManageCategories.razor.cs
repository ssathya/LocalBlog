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
    private bool renderCompleted = false;

    protected override void OnInitialized()
    {
        categoryForm ??= new();
        CategoryToEdit = null;
    }

    protected override void OnAfterRender(bool firstRender)
    {
        renderCompleted = !firstRender;
    }

    protected override async Task OnInitializedAsync()
    {
        await HandleCategoryChanged();
    }

    private async Task HandleCategoryChanged()
    {
        loading = true;
        if (CategoryService is not null)
        {
            categories = await CategoryService.GetCategoriesAsync();
        }
        categories ??= [];
        loading = false;
        CategoryToEdit = new();
        StateHasChanged();
    }

    private EventCallback SetChildCategory(int id)
    {
        if (renderCompleted)
        {
            CategoryToEdit = categories!.FirstOrDefault(c => c.Id == id);
            return EventCallback.Factory.Create(this, HandleCategoryChanged);
        }
        else
        {
            return EventCallback.Factory.Create(this, () => CategoryToEdit = categories!.FirstOrDefault(c => c.Id == id));
        }
    }
}