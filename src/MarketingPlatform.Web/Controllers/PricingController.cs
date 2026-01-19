using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketingPlatform.Web.Controllers;

/// <summary>
/// Controller for managing pricing plans and models
/// </summary>
[Authorize]
public class PricingController : Controller
{
    private readonly ILogger<PricingController> _logger;
    private readonly IConfiguration _configuration;

    public PricingController(ILogger<PricingController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Display pricing plans management (Admin only)
    /// </summary>
    public IActionResult Index()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    /// <summary>
    /// Create new pricing plan
    /// </summary>
    public IActionResult Create()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    /// <summary>
    /// Edit pricing plan
    /// </summary>
    public IActionResult Edit(int id)
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        ViewBag.PricingModelId = id;
        return View();
    }

    /// <summary>
    /// Manage channel pricing
    /// </summary>
    public IActionResult Channels(int modelId)
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        ViewBag.PricingModelId = modelId;
        return View();
    }
}
