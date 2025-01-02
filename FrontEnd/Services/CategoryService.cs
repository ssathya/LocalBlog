﻿using Microsoft.EntityFrameworkCore;
using Models;
using Models.AppSpecific;

namespace FrontEnd.Services;

public class CategoryService(BlogContext context, ILogger<CategoryService> logger)
{
    private readonly BlogContext context = context;
    private readonly ILogger<CategoryService> logger = logger;

    public async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        try
        {
            return await context.Categories
                .AsNoTracking()
                .ToListAsync();
        }
        catch (Exception ex)
        {
            logger.LogError($"Error obtaining values from Categories table.\n{ex.Message}");
            return [];
        }
    }

    public async Task<MethodResult> SaveCategoryInDbAsync(Category category)
    {
        try
        {
            if (category.Id == 0)
            {
                await context.Categories.AddAsync(category);
            }
            else
            {
                context.Categories.Update(category);
            }
            await context.SaveChangesAsync();
            return MethodResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError($"Error saving category.\n{ex.Message}");
            return MethodResult.Fail($"Error saving category.\n{ex.Message}");
        }
    }
}