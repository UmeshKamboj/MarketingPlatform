using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketingPlatform.Web.Controllers;

/// <summary>
/// Controller for user management and profile
/// </summary>
[Authorize]
public class UsersController : Controller
{
    private readonly ILogger<UsersController> _logger;
    private readonly IConfiguration _configuration;

    public UsersController(ILogger<UsersController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Display users list (Admin only)
    /// </summary>
    public IActionResult Index()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    /// <summary>
    /// User dashboard
    /// </summary>
    public IActionResult Dashboard()
    {
        _logger.LogInformation("Dashboard accessed. User authenticated: {IsAuthenticated}, User: {User}",
            User.Identity?.IsAuthenticated,
            User.Identity?.Name ?? "Anonymous");

        if (!User.Identity?.IsAuthenticated ?? true)
        {
            _logger.LogWarning("User not authenticated when accessing Dashboard - will be redirected to login");
        }
        else
        {
            _logger.LogInformation("User {User} successfully authenticated on Dashboard", User.Identity.Name);
        }

        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    /// <summary>
    /// View/Edit user profile
    /// </summary>
    public IActionResult Profile()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    /// <summary>
    /// User settings
    /// </summary>
    public IActionResult Settings()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }
}
