using Microsoft.EntityFrameworkCore;
using MarketingPlatform.Infrastructure.Data;
using MarketingPlatform.Core.Entities;
using Microsoft.AspNetCore.Identity;
using MarketingPlatform.Application.Interfaces;
using MarketingPlatform.Infrastructure.Services;
using MarketingPlatform.Application.Services;
using MarketingPlatform.Core.Interfaces.Repositories;
using MarketingPlatform.Infrastructure.Repositories;
using MarketingPlatform.Web;
using MarketingPlatform.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Repository Pattern
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPageContentRepository, PageContentRepository>();

// Application Services
builder.Services.AddScoped<IPageContentService, PageContentService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();

// File Storage Providers
builder.Services.AddScoped<IFileStorageProvider, MarketingPlatform.Infrastructure.Services.FileStorageProviders.LocalFileStorageProvider>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Content Security Policy Middleware with nonce-based inline script/style support
// This middleware generates a unique nonce per request and sets CSP headers
// Development: Permissive CSP to allow hot reload, browser-link, WebSockets
// Production: Strict nonce-based CSP for security
app.UseMiddleware<CspMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed initial page content
using (var scope = app.Services.CreateScope())
{
    try
    {
        await DatabaseSeeder.SeedPageContentAsync(scope.ServiceProvider);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding page content.");
    }
}

app.Run();
