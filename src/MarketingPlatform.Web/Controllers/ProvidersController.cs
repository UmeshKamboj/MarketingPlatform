using Microsoft.AspNetCore.Mvc;

namespace MarketingPlatform.Web.Controllers;

/// <summary>
/// Controller for provider management (Super Admin)
/// </summary>
public class ProvidersController : Controller
{
    private readonly ILogger<ProvidersController> _logger;
    private readonly IConfiguration _configuration;

    public ProvidersController(ILogger<ProvidersController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Display providers list
    /// </summary>
    public IActionResult Index()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    /// <summary>
    /// Create new provider
    /// </summary>
    public IActionResult Create()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    /// <summary>
    /// Edit provider configuration
    /// </summary>
    public IActionResult Edit(int id)
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        ViewBag.ProviderId = id;
        return View();
    }

    /// <summary>
    /// Provider health monitoring
    /// </summary>
    public IActionResult Health()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    /// <summary>
    /// Routing configuration
    /// </summary>
    public IActionResult Routing()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }
}
