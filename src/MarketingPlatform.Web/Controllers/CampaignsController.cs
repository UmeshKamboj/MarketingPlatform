using Microsoft.AspNetCore.Mvc;

namespace MarketingPlatform.Web.Controllers;

public class CampaignsController : Controller
{
    private readonly ILogger<CampaignsController> _logger;
    private readonly IConfiguration _configuration;

    public CampaignsController(ILogger<CampaignsController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }

    public IActionResult Create()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View();
    }
}
