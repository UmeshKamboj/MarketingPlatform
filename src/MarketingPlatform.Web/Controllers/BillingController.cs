using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketingPlatform.Web.Controllers
{
    [Authorize]
    public class BillingController : Controller
    {
        private readonly ILogger<BillingController> _logger;
        private readonly IConfiguration _configuration;

        public BillingController(ILogger<BillingController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            ViewBag.StripePublishableKey = _configuration["Stripe:PublishableKey"];
            return View();
        }

        public IActionResult Subscribe()
        {
            ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            ViewBag.StripePublishableKey = _configuration["Stripe:PublishableKey"];
            return View();
        }

        public IActionResult PaymentHistory()
        {
            ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            return View();
        }

        public IActionResult Invoices()
        {
            ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            return View();
        }

        public IActionResult Usage()
        {
            ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            return View();
        }
    }
}
