using Auth0.AspNetCore.Authentication;
using FrontEnd.AuthenticationStateSyncer;
using FrontEnd.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

//Auth0
builder.Services
    .AddAuth0WebAppAuthentication(options =>
    {
        options.Domain = builder.Configuration["Auth0:Domain"] ?? "Domain not populated";
        options.ClientId = builder.Configuration["Auth0:ClientId"] ?? "Client Id not populated";
    });
//Auth0

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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