﻿using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.AppSpecific;
using ModernBlog.Extensions;

namespace ModernBlog.Services;

public class BlogPostService(BlogContext context, ILogger<BlogPostService> logger, AuthenticationStateProvider stateProvider) : IBlogPostService
{
    private readonly BlogContext context = context;
    private readonly ILogger<BlogPostService> logger = logger;
    private readonly AuthenticationStateProvider stateProvider = stateProvider;

    public async Task<BlogPost?> GetPostByIdAsync(int id)
    {
        try
        {
            return await context.BlogPosts.FirstOrDefaultAsync(p => p.Id == id);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error obtaining all Blog Posts.\n{ex.Message}");
            return null;
        }
    }

    public async Task<MethodResult> CreateOrUpdateBlogPostAsync(BlogPost post)
    {
        var authState = await stateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        post.Title = post.Title.Truncate(120);
        post.Introduction = post.Introduction.Truncate(255);
        if (user.Identity?.IsAuthenticated == true)
        {
            post.UserId = user.Identity.Name;
        }
        try
        {
            if (post.Id == 0)
            {
                post.CreatedOn = DateTime.UtcNow;
                post.Slug = post.Title.ManageSlug();
                await context.BlogPosts.AddAsync(post);
            }
            else
            {
                post.ModifiedOn = DateTime.UtcNow;
                context.BlogPosts.Update(post);
            }
            await context.SaveChangesAsync();
            return new MethodResult(true);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error creating or updating Blog Post.\n{ex.Message}");
            return new MethodResult(false, $"Error creating or updating Blog Post.\n{ex.Message}");
        }
    }

    public async Task<MethodResult> DeleteBlogPostAsync(int id)
    {
        try
        {
            var post = await context.BlogPosts.FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return new MethodResult(false, "Blog post not found.");
            }
            context.BlogPosts.Remove(post);
            await context.SaveChangesAsync();
            return new MethodResult(true);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error deleting Blog Post.\n{ex.Message}");
            return new MethodResult(false, $"Error deleting Blog Post.\n{ex.Message}");
        }
    }
}