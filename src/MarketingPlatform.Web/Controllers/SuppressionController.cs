using Microsoft.AspNetCore.Mvc;

namespace MarketingPlatform.Web.Controllers;

/// <summary>
/// Controller for suppression list and opt-out management
/// </summary>
public class SuppressionController : Controller
{
    private readonly ILogger<SuppressionController> _logger;
    private readonly IConfiguration _configuration;

    public SuppressionController(ILogger<SuppressionController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Display suppression lists
    /// </summary>
    public IActionResult Index()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    /// <summary>
    /// Create suppression list
    /// </summary>
    public IActionResult Create()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    /// <summary>
    /// Manage suppression list entries
    /// </summary>
    public IActionResult Entries(int id)
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        ViewBag.ListId = id;
        return View();
    }

    /// <summary>
    /// Import suppression list
    /// </summary>
    public IActionResult Import()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }
}
