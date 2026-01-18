using Microsoft.AspNetCore.Mvc;

namespace MarketingPlatform.Web.Controllers;

/// <summary>
/// Controller for analytics and reporting
/// </summary>
public class AnalyticsController : Controller
{
    private readonly ILogger<AnalyticsController> _logger;
    private readonly IConfiguration _configuration;

    public AnalyticsController(ILogger<AnalyticsController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Display analytics dashboard
    /// </summary>
    public IActionResult Index()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    /// <summary>
    /// Campaign performance report
    /// </summary>
    public IActionResult Campaigns()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    /// <summary>
    /// Export reports
    /// </summary>
    public IActionResult Reports()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }
}
