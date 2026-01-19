using Microsoft.AspNetCore.Mvc;
using MarketingPlatform.Web.Models;

namespace MarketingPlatform.Web.Controllers
{
    /// <summary>
    /// User Dashboard Controller - For regular users to access platform features
    /// </summary>
    public class UserDashboardController : Controller
    {
        private readonly ILogger<UserDashboardController> _logger;
        private readonly IConfiguration _configuration;

        public UserDashboardController(
            ILogger<UserDashboardController> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// User Dashboard Index page
        /// </summary>
        public IActionResult Index()
        {
            // Create mock data for demonstration
            var viewModel = new UserDashboardViewModel
            {
                UserName = "John Doe", // This would come from authenticated user
                Stats = new UserStats
                {
                    TotalCampaigns = 12,
                    ActiveCampaigns = 3,
                    TotalContacts = 2456,
                    MessagesSent = 15678,
                    EngagementRate = 72.5
                },
                MyCampaigns = GenerateMockUserCampaigns(),
                RecentActivities = GenerateMockUserActivities(),
                Analytics = new UserAnalytics
                {
                    MonthlyMessages = GenerateMockMonthlyData(),
                    TopCampaigns = GenerateMockTopCampaigns()
                }
            };

            ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            return View(viewModel);
        }

        /// <summary>
        /// Generate mock campaign data for user
        /// </summary>
        private List<CampaignSummary> GenerateMockUserCampaigns()
        {
            return new List<CampaignSummary>
            {
                new CampaignSummary
                {
                    Id = 1,
                    Name = "Product Launch Email",
                    Status = "Active",
                    Type = "Email",
                    Recipients = 1234,
                    SuccessCount = 1198,
                    FailureCount = 36,
                    ScheduledAt = DateTime.UtcNow.AddHours(-2),
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new CampaignSummary
                {
                    Id = 2,
                    Name = "Welcome Series SMS",
                    Status = "Active",
                    Type = "SMS",
                    Recipients = 567,
                    SuccessCount = 561,
                    FailureCount = 6,
                    ScheduledAt = DateTime.UtcNow.AddHours(-6),
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new CampaignSummary
                {
                    Id = 3,
                    Name = "Monthly Newsletter",
                    Status = "Scheduled",
                    Type = "Email",
                    Recipients = 2456,
                    SuccessCount = 0,
                    FailureCount = 0,
                    ScheduledAt = DateTime.UtcNow.AddDays(2),
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
                }
            };
        }

        /// <summary>
        /// Generate mock activity data for user
        /// </summary>
        private List<RecentActivityItem> GenerateMockUserActivities()
        {
            return new List<RecentActivityItem>
            {
                new RecentActivityItem
                {
                    Icon = "bi-megaphone",
                    Title = "Campaign Started",
                    Description = "Product Launch Email campaign is now active",
                    Timestamp = DateTime.UtcNow.AddHours(-2),
                    Type = "campaign"
                },
                new RecentActivityItem
                {
                    Icon = "bi-people",
                    Title = "Contacts Imported",
                    Description = "Added 150 new contacts to your list",
                    Timestamp = DateTime.UtcNow.AddHours(-4),
                    Type = "contacts"
                },
                new RecentActivityItem
                {
                    Icon = "bi-file-earmark-text",
                    Title = "Template Created",
                    Description = "New email template 'Promotion Template' created",
                    Timestamp = DateTime.UtcNow.AddDays(-1),
                    Type = "template"
                },
                new RecentActivityItem
                {
                    Icon = "bi-envelope-check",
                    Title = "Campaign Completed",
                    Description = "Welcome Series SMS sent successfully",
                    Timestamp = DateTime.UtcNow.AddDays(-2),
                    Type = "campaign"
                },
                new RecentActivityItem
                {
                    Icon = "bi-graph-up",
                    Title = "Report Generated",
                    Description = "Campaign performance report available",
                    Timestamp = DateTime.UtcNow.AddDays(-3),
                    Type = "analytics"
                }
            };
        }

        /// <summary>
        /// Generate mock monthly data for charts
        /// </summary>
        private List<MonthlyData> GenerateMockMonthlyData()
        {
            return new List<MonthlyData>
            {
                new MonthlyData { Month = "Jan", Count = 1234 },
                new MonthlyData { Month = "Feb", Count = 1567 },
                new MonthlyData { Month = "Mar", Count = 1890 },
                new MonthlyData { Month = "Apr", Count = 2123 },
                new MonthlyData { Month = "May", Count = 2456 },
                new MonthlyData { Month = "Jun", Count = 2789 }
            };
        }

        /// <summary>
        /// Generate mock top campaigns data
        /// </summary>
        private List<CampaignPerformance> GenerateMockTopCampaigns()
        {
            return new List<CampaignPerformance>
            {
                new CampaignPerformance
                {
                    CampaignName = "Product Launch",
                    SuccessRate = 97.1,
                    TotalSent = 1234
                },
                new CampaignPerformance
                {
                    CampaignName = "Welcome Series",
                    SuccessRate = 98.9,
                    TotalSent = 567
                },
                new CampaignPerformance
                {
                    CampaignName = "Weekly Update",
                    SuccessRate = 96.3,
                    TotalSent = 890
                }
            };
        }
    }
}
