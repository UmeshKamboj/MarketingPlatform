using Microsoft.AspNetCore.Mvc;

namespace MarketingPlatform.Web.Controllers;

/// <summary>
/// Controller for managing automated workflows and journeys
/// </summary>
public class WorkflowsController : Controller
{
    private readonly ILogger<WorkflowsController> _logger;
    private readonly IConfiguration _configuration;

    public WorkflowsController(ILogger<WorkflowsController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Display workflows list
    /// </summary>
    public IActionResult Index()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    /// <summary>
    /// Create new workflow
    /// </summary>
    public IActionResult Create()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    /// <summary>
    /// Edit workflow
    /// </summary>
    public IActionResult Edit(int id)
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        ViewBag.WorkflowId = id;
        return View();
    }
}
