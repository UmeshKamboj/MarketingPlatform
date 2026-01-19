using Microsoft.AspNetCore.Mvc;

namespace MarketingPlatform.Web.Controllers;

/// <summary>
/// Controller for authentication (Login, Register, etc.)
/// </summary>
public class AuthController : Controller
{
    private readonly ILogger<AuthController> _logger;
    private readonly IConfiguration _configuration;

    public AuthController(ILogger<AuthController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Login page
    /// </summary>
    public IActionResult Login()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        ViewBag.RecaptchaSiteKey = _configuration["ReCaptcha:SiteKey"];
        return View();
    }

    /// <summary>
    /// Registration page
    /// </summary>
    public IActionResult Register()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        ViewBag.RecaptchaSiteKey = _configuration["ReCaptcha:SiteKey"];
        return View();
    }

    /// <summary>
    /// Logout action
    /// </summary>
    public IActionResult Logout()
    {
        // Clear authentication cookies/tokens
        return RedirectToAction("Login");
    }

    /// <summary>
    /// Forgot password page
    /// </summary>
    public IActionResult ForgotPassword()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        ViewBag.RecaptchaSiteKey = _configuration["ReCaptcha:SiteKey"];
        return View();
    }

    /// <summary>
    /// Get ReCaptcha configuration for dynamic loading
    /// </summary>
    /// <returns>ReCaptcha configuration JSON</returns>
    [HttpGet("recaptcha-config")]
    public IActionResult GetRecaptchaConfig()
    {
        return Json(new
        {
            siteKey = _configuration["ReCaptcha:SiteKey"],
            version = _configuration["ReCaptcha:Version"]
        });
    }
}
