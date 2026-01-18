using Microsoft.AspNetCore.Mvc;

namespace MarketingPlatform.Web.Controllers;

/// <summary>
/// Controller for webhook management
/// </summary>
public class WebhooksController : Controller
{
    private readonly ILogger<WebhooksController> _logger;
    private readonly IConfiguration _configuration;

    public WebhooksController(ILogger<WebhooksController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Display webhooks list
    /// </summary>
    public IActionResult Index()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    /// <summary>
    /// Create new webhook
    /// </summary>
    public IActionResult Create()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    /// <summary>
    /// Edit webhook
    /// </summary>
    public IActionResult Edit(int id)
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        ViewBag.WebhookId = id;
        return View();
    }

    /// <summary>
    /// Webhook logs and testing
    /// </summary>
    public IActionResult Logs(int id)
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        ViewBag.WebhookId = id;
        return View();
    }
}
