using Auth0.AspNetCore.Authentication;
using FrontEnd.AuthenticationStateSyncer;
using FrontEnd.Components;
using FrontEnd.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

ConfigureLogging(builder, config);

ConfigureCaching(builder);

builder.Services.AddCascadingAuthenticationState();

SetupDependencyInjection(builder);

SetupAuth0Authentication(builder);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

SetupDatabaseConnection(builder);

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
//Auth0
app.MapGet("/Account/Login", async (HttpContext context, string returnUrl = "/") =>
{
    var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
        .WithRedirectUri(returnUrl)
        .Build();
    await context.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
});
app.MapGet("/Account/Logout", async (HttpContext context) =>
{
    var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
         .WithRedirectUri("/")
         .Build();
    await context.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
});
//Auth0

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

static void SetupDependencyInjection(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();
    builder.Services.AddScoped<CategoryService>();
}

static void SetupDatabaseConnection(WebApplicationBuilder builder)
{
    //Database
    var blogConnectionString = builder.Configuration.GetConnectionString("BlogDbConnection");
    if (string.IsNullOrEmpty(blogConnectionString))
    {
        throw new Exception("Configuration missing: ConnectionStrings:BlogDbConnection");
    }
    builder.Services.AddDbContext<BlogContext>(options =>
    {
        options.UseNpgsql(blogConnectionString);
    });
}

static void ConfigureLogging(WebApplicationBuilder builder, ConfigurationManager config)
{
    //Logging
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

static void ConfigureCaching(WebApplicationBuilder builder)
{
    //Caching
    builder.Services.AddOutputCache(cfg =>
    {
        cfg.AddBasePolicy(bldr =>
        {
            bldr.With(r => r.HttpContext.Request.Path.StartsWithSegments("/"));
            bldr.Expire(TimeSpan.FromMinutes(5));
        });
        cfg.AddPolicy("ShortCache", builder =>
        {
            builder.Expire(TimeSpan.FromSeconds(25));
        });
    });
}

static void SetupAuth0Authentication(WebApplicationBuilder builder)
{
    //Auth0
    builder.Services
        .AddAuth0WebAppAuthentication(options =>
        {
            options.Domain = builder.Configuration["Auth0:Domain"] ?? "Domain not populated";
            options.ClientId = builder.Configuration["Auth0:ClientId"] ?? "Client Id not populated";
        });
    //Auth0
}