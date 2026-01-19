using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketingPlatform.Web.Controllers;

/// <summary>
/// Controller for managing contacts and contact groups
/// </summary>
[Authorize]
public class ContactsController : Controller
{
    private readonly ILogger<ContactsController> _logger;
    private readonly IConfiguration _configuration;

    public ContactsController(ILogger<ContactsController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Display contacts list
    /// </summary>
    public IActionResult Index()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    /// <summary>
    /// Create new contact
    /// </summary>
    public IActionResult Create()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    /// <summary>
    /// View contact details
    /// </summary>
    public IActionResult Details(int id)
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        ViewBag.ContactId = id;
        return View();
    }

    /// <summary>
    /// Manage contact groups
    /// </summary>
    public IActionResult Groups()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }
}
