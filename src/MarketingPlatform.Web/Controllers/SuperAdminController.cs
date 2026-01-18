using Microsoft.AspNetCore.Mvc;

namespace MarketingPlatform.Web.Controllers;

/// <summary>
/// Controller for super admin functions
/// </summary>
public class SuperAdminController : Controller
{
    private readonly ILogger<SuperAdminController> _logger;
    private readonly IConfiguration _configuration;

    public SuperAdminController(ILogger<SuperAdminController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Super admin dashboard
    /// </summary>
    public IActionResult Dashboard()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    /// <summary>
    /// Manage super admins
    /// </summary>
    public IActionResult Users()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    /// <summary>
    /// Platform configuration
    /// </summary>
    public IActionResult PlatformConfig()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    /// <summary>
    /// Audit logs
    /// </summary>
    public IActionResult AuditLogs()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }
}
