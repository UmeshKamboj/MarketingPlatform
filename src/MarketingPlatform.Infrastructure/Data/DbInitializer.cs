using MarketingPlatform.Core.Entities;
using MarketingPlatform.Core.Enums;
using Microsoft.AspNetCore.Identity;

namespace MarketingPlatform.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Seed Identity Roles
            string[] identityRoles = { "Admin", "User", "Manager", "SuperAdmin" };
            foreach (var role in identityRoles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed Custom Roles with Permissions
            if (!context.CustomRoles.Any())
            {
                var customRoles = new List<Role>
                {
                    new Role
                    {
                        Name = "SuperAdmin",
                        Description = "Full system access with all permissions",
                        Permissions = (long)Permission.All,
                        IsSystemRole = true,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Role
                    {
                        Name = "Admin",
                        Description = "Administrator with most permissions except user/role management",
                        Permissions = (long)(Permission.ViewCampaigns | Permission.CreateCampaigns | 
                            Permission.EditCampaigns | Permission.DeleteCampaigns |
                            Permission.ViewContacts | Permission.CreateContacts | 
                            Permission.EditContacts | Permission.DeleteContacts |
                            Permission.ViewTemplates | Permission.CreateTemplates | 
                            Permission.EditTemplates | Permission.DeleteTemplates |
                            Permission.ViewAnalytics | Permission.ViewDetailedAnalytics | 
                            Permission.ExportAnalytics |
                            Permission.ViewWorkflows | Permission.CreateWorkflows | 
                            Permission.EditWorkflows | Permission.DeleteWorkflows |
                            Permission.ViewSettings | Permission.ManageSettings |
                            Permission.ViewCompliance | Permission.ManageCompliance |
                            Permission.ViewAuditLogs),
                        IsSystemRole = true,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Role
                    {
                        Name = "Manager",
                        Description = "Campaign and contact management with analytics access",
                        Permissions = (long)(Permission.ViewCampaigns | Permission.CreateCampaigns | 
                            Permission.EditCampaigns |
                            Permission.ViewContacts | Permission.CreateContacts | 
                            Permission.EditContacts |
                            Permission.ViewTemplates | Permission.CreateTemplates | 
                            Permission.EditTemplates |
                            Permission.ViewAnalytics | Permission.ViewDetailedAnalytics |
                            Permission.ViewWorkflows | Permission.CreateWorkflows | 
                            Permission.EditWorkflows |
                            Permission.ViewCompliance),
                        IsSystemRole = true,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Role
                    {
                        Name = "Analyst",
                        Description = "Read access with detailed analytics capabilities",
                        Permissions = (long)(Permission.ViewCampaigns | Permission.ViewContacts | 
                            Permission.ViewTemplates | Permission.ViewAnalytics | 
                            Permission.ViewDetailedAnalytics | Permission.ExportAnalytics |
                            Permission.ViewWorkflows | Permission.ViewCompliance),
                        IsSystemRole = true,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Role
                    {
                        Name = "Viewer",
                        Description = "Read-only access to campaigns and basic analytics",
                        Permissions = (long)(Permission.ViewCampaigns | Permission.ViewContacts | 
                            Permission.ViewTemplates | Permission.ViewAnalytics),
                        IsSystemRole = true,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                context.CustomRoles.AddRange(customRoles);
                await context.SaveChangesAsync();
            }

            // Seed Default Admin User
            var adminEmail = "admin@marketingplatform.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "User",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123456");
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "SuperAdmin");
                    
                    // Assign SuperAdmin custom role
                    var superAdminRole = context.CustomRoles.FirstOrDefault(r => r.Name == "SuperAdmin");
                    if (superAdminRole != null)
                    {
                        var userRole = new Core.Entities.UserRole
                        {
                            UserId = adminUser.Id,
                            RoleId = superAdminRole.Id,
                            AssignedAt = DateTime.UtcNow,
                            AssignedBy = "System"
                        };
                        context.CustomUserRoles.Add(userRole);
                        await context.SaveChangesAsync();
                    }
                }
            }

            // Seed Subscription Plans
            if (!context.SubscriptionPlans.Any())
            {
                var plans = new List<SubscriptionPlan>
                {
                    new SubscriptionPlan
                    {
                        Name = "Free",
                        Description = "Perfect for trying out the platform",
                        PriceMonthly = 0,
                        PriceYearly = 0,
                        SMSLimit = 100,
                        MMSLimit = 10,
                        EmailLimit = 500,
                        ContactLimit = 500,
                        Features = "[\"Basic campaign management\", \"Basic analytics\", \"Email support\"]",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new SubscriptionPlan
                    {
                        Name = "Pro",
                        Description = "For growing businesses",
                        PriceMonthly = 49.99m,
                        PriceYearly = 499.99m,
                        SMSLimit = 5000,
                        MMSLimit = 500,
                        EmailLimit = 25000,
                        ContactLimit = 10000,
                        Features = "[\"Advanced campaign management\", \"Workflows & automation\", \"Advanced analytics\", \"Priority support\", \"Custom templates\"]",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new SubscriptionPlan
                    {
                        Name = "Enterprise",
                        Description = "For large organizations",
                        PriceMonthly = 199.99m,
                        PriceYearly = 1999.99m,
                        SMSLimit = 50000,
                        MMSLimit = 5000,
                        EmailLimit = 250000,
                        ContactLimit = 100000,
                        Features = "[\"Unlimited campaigns\", \"Advanced workflows\", \"Premium analytics\", \"24/7 support\", \"Dedicated account manager\", \"API access\", \"White-label options\"]",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                context.SubscriptionPlans.AddRange(plans);
                await context.SaveChangesAsync();
            }

            // Seed Message Providers
            if (!context.MessageProviders.Any())
            {
                var providers = new List<MessageProvider>
                {
                    new MessageProvider
                    {
                        Name = "Twilio SMS",
                        Type = ProviderType.SMS,
                        IsActive = true,
                        IsPrimary = true,
                        HealthStatus = HealthStatus.Unknown,
                        CreatedAt = DateTime.UtcNow
                    },
                    new MessageProvider
                    {
                        Name = "SendGrid Email",
                        Type = ProviderType.Email,
                        IsActive = true,
                        IsPrimary = true,
                        HealthStatus = HealthStatus.Unknown,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                context.MessageProviders.AddRange(providers);
                await context.SaveChangesAsync();
            }

            // Seed Channel Routing Configurations
            if (!context.ChannelRoutingConfigs.Any())
            {
                var routingConfigs = new List<ChannelRoutingConfig>
                {
                    new ChannelRoutingConfig
                    {
                        Channel = ChannelType.SMS,
                        PrimaryProvider = "MockSMSProvider",
                        FallbackProvider = "BackupSMSProvider",
                        RoutingStrategy = RoutingStrategy.Primary,
                        EnableFallback = true,
                        MaxRetries = 3,
                        RetryStrategy = RetryStrategy.Exponential,
                        InitialRetryDelaySeconds = 60,
                        MaxRetryDelaySeconds = 3600,
                        IsActive = true,
                        Priority = 1,
                        CreatedAt = DateTime.UtcNow
                    },
                    new ChannelRoutingConfig
                    {
                        Channel = ChannelType.MMS,
                        PrimaryProvider = "MockMMSProvider",
                        FallbackProvider = "BackupMMSProvider",
                        RoutingStrategy = RoutingStrategy.Primary,
                        EnableFallback = true,
                        MaxRetries = 3,
                        RetryStrategy = RetryStrategy.Exponential,
                        InitialRetryDelaySeconds = 60,
                        MaxRetryDelaySeconds = 3600,
                        IsActive = true,
                        Priority = 1,
                        CreatedAt = DateTime.UtcNow
                    },
                    new ChannelRoutingConfig
                    {
                        Channel = ChannelType.Email,
                        PrimaryProvider = "MockEmailProvider",
                        FallbackProvider = "BackupEmailProvider",
                        RoutingStrategy = RoutingStrategy.Primary,
                        EnableFallback = true,
                        MaxRetries = 3,
                        RetryStrategy = RetryStrategy.Exponential,
                        InitialRetryDelaySeconds = 120,
                        MaxRetryDelaySeconds = 7200,
                        IsActive = true,
                        Priority = 1,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                context.ChannelRoutingConfigs.AddRange(routingConfigs);
                await context.SaveChangesAsync();
            }

            // Seed Pricing Models for Landing Page
            if (!context.PricingModels.Any())
            {
                var pricingModels = new List<PricingModel>
                {
                    new PricingModel
                    {
                        Name = "Starter",
                        Description = "Perfect for small businesses getting started",
                        BasePrice = 29.00m,
                        Currency = "USD",
                        BillingPeriod = "month",
                        IsActive = true,
                        DisplayOrder = 1,
                        Features = "[\"1,000 SMS messages/month\",\"500 emails/month\",\"Basic analytics\",\"Email support\",\"1 user\"]",
                        IsPopular = false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PricingModel
                    {
                        Name = "Professional",
                        Description = "For growing businesses with larger audiences",
                        BasePrice = 99.00m,
                        Currency = "USD",
                        BillingPeriod = "month",
                        IsActive = true,
                        DisplayOrder = 2,
                        Features = "[\"10,000 SMS messages/month\",\"5,000 emails/month\",\"Advanced analytics\",\"Priority support\",\"5 users\",\"Custom templates\",\"Automation workflows\"]",
                        IsPopular = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PricingModel
                    {
                        Name = "Enterprise",
                        Description = "For large organizations with custom needs",
                        BasePrice = 299.00m,
                        Currency = "USD",
                        BillingPeriod = "month",
                        IsActive = true,
                        DisplayOrder = 3,
                        Features = "[\"Unlimited SMS messages\",\"Unlimited emails\",\"Advanced analytics & reporting\",\"24/7 phone support\",\"Unlimited users\",\"Custom branding\",\"API access\",\"Dedicated account manager\"]",
                        IsPopular = false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };

                context.PricingModels.AddRange(pricingModels);
                await context.SaveChangesAsync();
            }

            // Seed Landing Page Configuration Settings
            if (!context.PlatformSettings.Any(s => s.Category == "LandingPage"))
            {
                var landingPageSettings = new List<PlatformSetting>
                {
                    // Hero Section Settings
                    new PlatformSetting
                    {
                        Key = "LandingPage.Hero.Type",
                        Value = "banner",
                        Category = "LandingPage",
                        Description = "Hero section type: 'banner' or 'slider'",
                        DataType = "string",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Hero.Title",
                        Value = "Transform Your Marketing with SMS, MMS & Email",
                        Category = "LandingPage",
                        Description = "Hero section main title",
                        DataType = "string",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Hero.Subtitle",
                        Value = "A powerful, enterprise-grade marketing platform to reach your customers where they are. Send targeted campaigns, track performance, and grow your business.",
                        Category = "LandingPage",
                        Description = "Hero section subtitle/description",
                        DataType = "string",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Hero.BannerImage",
                        Value = "/images/hero-banner.jpg",
                        Category = "LandingPage",
                        Description = "Hero banner image URL",
                        DataType = "string",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Hero.CTAText",
                        Value = "Get Started Free",
                        Category = "LandingPage",
                        Description = "Primary call-to-action button text",
                        DataType = "string",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Hero.CTALink",
                        Value = "/Auth/Register",
                        Category = "LandingPage",
                        Description = "Primary call-to-action button link",
                        DataType = "string",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },

                    // Slider Settings (if slider type is selected)
                    new PlatformSetting
                    {
                        Key = "LandingPage.Slider.Slides",
                        Value = "[{\"title\":\"Transform Your Marketing\",\"subtitle\":\"Reach customers on SMS, MMS & Email\",\"image\":\"/images/slide1.jpg\",\"ctaText\":\"Get Started\",\"ctaLink\":\"/Auth/Register\"},{\"title\":\"Advanced Analytics\",\"subtitle\":\"Track and optimize your campaigns\",\"image\":\"/images/slide2.jpg\",\"ctaText\":\"Learn More\",\"ctaLink\":\"#features\"},{\"title\":\"Automate Your Workflow\",\"subtitle\":\"Save time with powerful automation\",\"image\":\"/images/slide3.jpg\",\"ctaText\":\"See How\",\"ctaLink\":\"#features\"}]",
                        Category = "LandingPage",
                        Description = "Slider slides configuration (JSON array)",
                        DataType = "json",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Slider.AutoPlay",
                        Value = "true",
                        Category = "LandingPage",
                        Description = "Enable slider auto-play",
                        DataType = "boolean",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Slider.Interval",
                        Value = "5000",
                        Category = "LandingPage",
                        Description = "Slider auto-play interval in milliseconds",
                        DataType = "number",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },

                    // Menu/Navigation Settings
                    new PlatformSetting
                    {
                        Key = "LandingPage.Menu.BackgroundColor",
                        Value = "#ffffff",
                        Category = "LandingPage",
                        Description = "Navigation menu background color",
                        DataType = "color",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Menu.TextColor",
                        Value = "#212529",
                        Category = "LandingPage",
                        Description = "Navigation menu text color",
                        DataType = "color",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Menu.HoverColor",
                        Value = "#667eea",
                        Category = "LandingPage",
                        Description = "Navigation menu hover color",
                        DataType = "color",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Menu.FontSize",
                        Value = "16",
                        Category = "LandingPage",
                        Description = "Navigation menu font size (in pixels)",
                        DataType = "number",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Menu.Position",
                        Value = "top",
                        Category = "LandingPage",
                        Description = "Navigation menu position: 'top' or 'fixed'",
                        DataType = "string",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Menu.Items",
                        Value = "[{\"text\":\"Home\",\"link\":\"#home\",\"order\":1},{\"text\":\"Features\",\"link\":\"#features\",\"order\":2},{\"text\":\"Pricing\",\"link\":\"#pricing\",\"order\":3},{\"text\":\"Contact\",\"link\":\"#contact\",\"order\":4},{\"text\":\"Login\",\"link\":\"/Auth/Login\",\"order\":5,\"class\":\"btn-outline-primary\"}]",
                        Category = "LandingPage",
                        Description = "Navigation menu items (JSON array)",
                        DataType = "json",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },

                    // Theme Colors
                    new PlatformSetting
                    {
                        Key = "LandingPage.Theme.PrimaryColor",
                        Value = "#667eea",
                        Category = "LandingPage",
                        Description = "Primary theme color",
                        DataType = "color",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Theme.SecondaryColor",
                        Value = "#764ba2",
                        Category = "LandingPage",
                        Description = "Secondary theme color",
                        DataType = "color",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Theme.AccentColor",
                        Value = "#f093fb",
                        Category = "LandingPage",
                        Description = "Accent theme color",
                        DataType = "color",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },

                    // Company Info
                    new PlatformSetting
                    {
                        Key = "LandingPage.Company.Name",
                        Value = "Marketing Platform",
                        Category = "LandingPage",
                        Description = "Company name displayed on landing page",
                        DataType = "string",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Company.Logo",
                        Value = "/images/logo.png",
                        Category = "LandingPage",
                        Description = "Company logo URL",
                        DataType = "string",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Company.Tagline",
                        Value = "SMS, MMS & Email Marketing Platform",
                        Category = "LandingPage",
                        Description = "Company tagline",
                        DataType = "string",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },

                    // Statistics Section
                    new PlatformSetting
                    {
                        Key = "LandingPage.Stats.MessagesSent",
                        Value = "10M+",
                        Category = "LandingPage",
                        Description = "Messages sent statistic",
                        DataType = "string",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Stats.DeliveryRate",
                        Value = "98%",
                        Category = "LandingPage",
                        Description = "Delivery rate statistic",
                        DataType = "string",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Stats.ActiveUsers",
                        Value = "5K+",
                        Category = "LandingPage",
                        Description = "Active users statistic",
                        DataType = "string",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Stats.Support",
                        Value = "24/7",
                        Category = "LandingPage",
                        Description = "Support availability statistic",
                        DataType = "string",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },

                    // Footer Settings
                    new PlatformSetting
                    {
                        Key = "LandingPage.Footer.CopyrightText",
                        Value = "© 2024 Marketing Platform. All rights reserved.",
                        Category = "LandingPage",
                        Description = "Footer copyright text",
                        DataType = "string",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Footer.SocialLinks",
                        Value = "[{\"platform\":\"facebook\",\"url\":\"https://facebook.com/marketingplatform\",\"icon\":\"bi-facebook\"},{\"platform\":\"twitter\",\"url\":\"https://twitter.com/marketingplatform\",\"icon\":\"bi-twitter\"},{\"platform\":\"linkedin\",\"url\":\"https://linkedin.com/company/marketingplatform\",\"icon\":\"bi-linkedin\"},{\"platform\":\"instagram\",\"url\":\"https://instagram.com/marketingplatform\",\"icon\":\"bi-instagram\"}]",
                        Category = "LandingPage",
                        Description = "Footer social media links (JSON array)",
                        DataType = "json",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },

                    // SEO Settings
                    new PlatformSetting
                    {
                        Key = "LandingPage.SEO.Title",
                        Value = "Marketing Platform - SMS, MMS & Email Marketing",
                        Category = "LandingPage",
                        Description = "Page title for SEO",
                        DataType = "string",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.SEO.Description",
                        Value = "Transform your marketing with our enterprise-grade SMS, MMS & Email platform. Powerful automation, advanced analytics, and seamless integration.",
                        Category = "LandingPage",
                        Description = "Meta description for SEO",
                        DataType = "string",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.SEO.Keywords",
                        Value = "SMS marketing, email marketing, MMS marketing, marketing automation, campaign management",
                        Category = "LandingPage",
                        Description = "Meta keywords for SEO",
                        DataType = "string",
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };

                context.PlatformSettings.AddRange(landingPageSettings);
                await context.SaveChangesAsync();
            }
        }
    }
}
