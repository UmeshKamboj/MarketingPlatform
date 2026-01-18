using Microsoft.AspNetCore.Mvc;

namespace MarketingPlatform.Web.Controllers;

/// <summary>
/// Controller for managing landing page configuration (admin only)
/// </summary>
public class LandingPageConfigController : Controller
{
    private readonly ILogger<LandingPageConfigController> _logger;
    private readonly IConfiguration _configuration;

    public LandingPageConfigController(ILogger<LandingPageConfigController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Landing page configuration dashboard
    /// </summary>
    public IActionResult Index()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    /// <summary>
    /// Configure hero section (banner/slider)
    /// </summary>
    public IActionResult HeroSection()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    /// <summary>
    /// Configure navigation menu
    /// </summary>
    public IActionResult MenuConfig()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    /// <summary>
    /// Configure features section
    /// </summary>
    public IActionResult Features()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    /// <summary>
    /// Configure pricing plans display
    /// </summary>
    public IActionResult PricingDisplay()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    /// <summary>
    /// Configure footer
    /// </summary>
    public IActionResult Footer()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    /// <summary>
    /// Preview landing page with current settings
    /// </summary>
    public IActionResult Preview()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }
}
