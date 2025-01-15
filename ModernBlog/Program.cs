using Auth0.AspNetCore.Authentication;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Models;
using ModernBlog.AuthenticationStateSync;
using ModernBlog.Components;
using ModernBlog.Services;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
ConfigureLogging(builder);

RegisterAuth0(builder);

RegisterConnectionToDb(builder);

RegisterServices(builder);
builder.Services.AddOutputCache(cfg =>
{
    cfg.AddBasePolicy(bldr =>
    {
        bldr.With(r => r.HttpContext.Request.Path.StartsWithSegments("/"));
        bldr.Expire(TimeSpan.FromMinutes(1));
    });
});

builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

RegisterAuth0Routes(app);
app.UseOutputCache();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

static void RegisterAuth0(WebApplicationBuilder builder)
{
    // 👇 new code
    builder.Services.AddCascadingAuthenticationState();
    builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();
    // 👆 new code

    // 👇 new code
    builder.Services
        .AddAuth0WebAppAuthentication(options =>
        {
            options.Domain = builder.Configuration["Auth0:Domain"] ?? throw (new Exception("Auth0:Domain not configured"));
            options.ClientId = builder.Configuration["Auth0:ClientId"] ?? throw (new Exception("Auth0:Domain not configured"));
        });
    // 👆 new code
}

static void RegisterAuth0Routes(WebApplication app)
{
    // 👇 new code
    app.MapGet("/Account/Login", async (HttpContext httpContext, string returnUrl = "/") =>
    {
        var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
                .WithRedirectUri(returnUrl)
                .Build();

        await httpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    });

    app.MapGet("/Account/Logout", async (HttpContext httpContext) =>
    {
        var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
                .WithRedirectUri("/")
                .Build();

        await httpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    });
    // 👆 new code
}

static void RegisterConnectionToDb(WebApplicationBuilder builder)
{
    var blogConnectionString = builder.Configuration.GetConnectionString("BlogDbConnection")
        ?? throw new Exception("BlogDbConnection not configured");
    builder.Services.AddDbContext<BlogContext>(options =>
    {
        options.UseNpgsql(blogConnectionString);
    });
}

static void RegisterServices(WebApplicationBuilder builder)
{
    builder.Services.AddTransient<ICategoryService, CategoryService>();
    builder.Services.AddTransient<IBlogCategoryService, BlogCategoryService>();
    builder.Services.AddTransient<IBlogPostService, BlogPostService>();
}
static void ConfigureLogging(WebApplicationBuilder builder)
{
    //Logging
    var config = builder.Configuration;
    StringBuilder filePath = new();
    filePath.Append(Path.GetTempPath() + "/");
    filePath.Append("BlazorChat-.log");
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(config)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(filePath.ToString(), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 3)
        .CreateLogger();
    builder.Services.AddLogging(c =>
    {
        c.SetMinimumLevel(LogLevel.Debug);
        c.AddSerilog(Log.Logger);
    });
}