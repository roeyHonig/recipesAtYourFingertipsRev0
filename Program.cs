using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using recipesAtYourFingertipsRev0.Data;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using recipesAtYourFingertipsRev0.Services;
using recipesAtYourFingertipsRev0.Middleware;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment())
{
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders =
            ForwardedHeaders.XForwardedFor |
            ForwardedHeaders.XForwardedProto |
            ForwardedHeaders.XForwardedHost;

        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();
    });
}


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<CurrentUserService>();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");
if (connectionString == null)
{
    throw new InvalidOperationException(
        "Connection string inside 'DefaultConnection' was not found. look at the readme file for instructions on how to set up the database connection string.");
}
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDataProtection()
    .PersistKeysToDbContext<ApplicationDbContext>();

var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
if (string.IsNullOrWhiteSpace(googleClientId))
{
    Console.WriteLine("ERROR: Google Client ID was not found.");
}
if (string.IsNullOrWhiteSpace(googleClientSecret))
{
    Console.WriteLine("ERROR: Google Client Secret was not found.");
}
if (string.IsNullOrWhiteSpace(googleClientId) ||
    string.IsNullOrWhiteSpace(googleClientSecret))
{
    throw new InvalidOperationException(
        "Google authentication credentials are missing.");
}
var microsoftClientId =
    builder.Configuration["Authentication:Microsoft:ClientId"];

var microsoftClientSecret =
    builder.Configuration["Authentication:Microsoft:ClientSecret"];

if (string.IsNullOrWhiteSpace(microsoftClientId) ||
    string.IsNullOrWhiteSpace(microsoftClientSecret))
{
    throw new InvalidOperationException(
        "Microsoft authentication credentials are missing.");
}
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            CookieAuthenticationDefaults.AuthenticationScheme;

        options.DefaultSignInScheme =
            CookieAuthenticationDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.LoginPath = "/Home";
    })
    .AddGoogle(options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;

        options.Events.OnCreatingTicket = context =>
        {
            context.Identity!.AddClaim(
                new Claim("ExternalProvider", "Google"));

            return Task.CompletedTask;
        };
    })
    .AddMicrosoftAccount(options =>
    {
        options.ClientId = microsoftClientId;
        options.ClientSecret = microsoftClientSecret;

        options.Events.OnCreatingTicket = context =>
        {
            context.Identity!.AddClaim(
                new Claim("ExternalProvider", "Microsoft"));

            return Task.CompletedTask;
        };
    });


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders();
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    // DigitalOcean terminates HTTPS at the reverse proxy.
    // The application container receives HTTP internally,
    // but the public application is HTTPS-only.
    app.Use((context, next) =>
    {
        context.Request.Scheme = "https";
        return next();
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseCurrentUser();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
