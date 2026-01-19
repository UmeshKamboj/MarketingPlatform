using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketingPlatform.Web.Controllers;

/// <summary>
/// Controller for message management and composition
/// </summary>
[Authorize]
public class MessagesController : Controller
{
    private readonly ILogger<MessagesController> _logger;
    private readonly IConfiguration _configuration;

    public MessagesController(ILogger<MessagesController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Display messages list
    /// </summary>
    public IActionResult Index()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    /// <summary>
    /// Compose new message
    /// </summary>
    public IActionResult Compose()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    /// <summary>
    /// View message details
    /// </summary>
    public IActionResult Details(int id)
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        ViewBag.MessageId = id;
        return View();
    }

    /// <summary>
    /// Message preview and testing
    /// </summary>
    public IActionResult Preview()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }
}
