using MarketingPlatform.Core.Entities;
using MarketingPlatform.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MarketingPlatform.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger? logger = null)
        {
            logger?.LogInformation("Starting database seeding process...");

            try
            {
                await SeedIdentityRolesAsync(roleManager, logger);
                await SeedCustomRolesAsync(context, logger);
                await SeedUsersAsync(context, userManager, roleManager, logger);
                await SeedSubscriptionPlansAsync(context, logger);
                await SeedMessageProvidersAsync(context, logger);
                await SeedChannelRoutingConfigsAsync(context, logger);
                await SeedPricingModelsAsync(context, logger);
                await SeedLandingPageSettingsAsync(context, logger);
                await SeedPageContentAsync(context, logger);

                logger?.LogInformation("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "An error occurred during database seeding.");
                throw;
            }
        }

        private static async Task SeedIdentityRolesAsync(RoleManager<IdentityRole> roleManager, ILogger? logger)
        {
            logger?.LogInformation("Seeding Identity roles...");

            try
            {
                // Seed Identity Roles
                string[] identityRoles = { "Admin", "User", "Manager", "SuperAdmin" };
                foreach (var role in identityRoles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        var result = await roleManager.CreateAsync(new IdentityRole(role));
                        if (result.Succeeded)
                        {
                            logger?.LogInformation("Identity role '{Role}' created successfully.", role);
                        }
                        else
                        {
                            logger?.LogWarning("Failed to create Identity role '{Role}': {Errors}", 
                                role, string.Join(", ", result.Errors.Select(e => e.Description)));
                        }
                    }
                    else
                    {
                        logger?.LogInformation("Identity role '{Role}' already exists.", role);
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error seeding Identity roles.");
                throw;
            }
        }

        private static async Task SeedCustomRolesAsync(ApplicationDbContext context, ILogger? logger)
        {
            logger?.LogInformation("Seeding custom roles...");

            try
            {

                // Seed Custom Roles with Permissions
                if (!await context.CustomRoles.AnyAsync())
                {
                    logger?.LogInformation("Creating custom roles...");

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
                        Description = "Administrator with most permissions including user/role management",
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
                            Permission.ViewAuditLogs |
                            Permission.ViewUsers | Permission.CreateUsers | 
                            Permission.EditUsers | Permission.ViewRoles),
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
                            Permission.ViewCompliance |
                            Permission.ViewUsers),
                        IsSystemRole = true,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Role
                    {
                        Name = "User",
                        Description = "Standard user with basic campaign and contact management",
                        Permissions = (long)(Permission.ViewCampaigns | Permission.CreateCampaigns |
                            Permission.ViewContacts | Permission.CreateContacts | 
                            Permission.ViewTemplates | Permission.ViewAnalytics),
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
                    logger?.LogInformation("Successfully created {Count} custom roles.", customRoles.Count);
                }
                else
                {
                    logger?.LogInformation("Custom roles already exist.");
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error seeding custom roles.");
                throw;
            }
        }

        private static async Task SeedUsersAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger? logger)
        {
            logger?.LogInformation("Seeding users...");

            try
            {

                // Seed Default Admin User
                var adminEmail = "admin@marketingplatform.com";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                
                if (adminUser == null)
                {
                    logger?.LogInformation("Creating default admin user...");

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
                        logger?.LogInformation("Admin user created successfully.");
                        
                        await userManager.AddToRoleAsync(adminUser, "SuperAdmin");
                        
                        // Assign SuperAdmin custom role
                        var superAdminRole = await context.CustomRoles.FirstOrDefaultAsync(r => r.Name == "SuperAdmin");
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
                            logger?.LogInformation("Admin user assigned to SuperAdmin role.");
                        }
                    }
                    else
                    {
                        logger?.LogWarning("Failed to create admin user: {Errors}", 
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    logger?.LogInformation("Admin user already exists.");
                }

                // Seed Additional Test Users
                var testUsers = new List<(string email, string password, string firstName, string lastName, string role)>
                {
                    ("manager@marketingplatform.com", "Manager@123456", "John", "Manager", "Manager"),
                    ("user@marketingplatform.com", "User@123456", "Jane", "User", "User"),
                    ("analyst@marketingplatform.com", "Analyst@123456", "Bob", "Analyst", "Analyst"),
                    ("viewer@marketingplatform.com", "Viewer@123456", "Alice", "Viewer", "Viewer")
                };

                logger?.LogInformation("Creating {Count} test users...", testUsers.Count);

                foreach (var (email, password, firstName, lastName, role) in testUsers)
                {
                    var existingUser = await userManager.FindByEmailAsync(email);
                    if (existingUser == null)
                    {
                        var newUser = new ApplicationUser
                        {
                            UserName = email,
                            Email = email,
                            EmailConfirmed = true,
                            FirstName = firstName,
                            LastName = lastName,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        };

                        var result = await userManager.CreateAsync(newUser, password);
                        
                        if (result.Succeeded)
                        {
                            logger?.LogInformation("Test user '{Email}' created successfully.", email);
                            
                            await userManager.AddToRoleAsync(newUser, role);
                            
                            // Assign custom role
                            var customRole = await context.CustomRoles.FirstOrDefaultAsync(r => r.Name == role);
                            if (customRole != null)
                            {
                                var userRole = new Core.Entities.UserRole
                                {
                                    UserId = newUser.Id,
                                    RoleId = customRole.Id,
                                    AssignedAt = DateTime.UtcNow,
                                    AssignedBy = "System"
                                };
                                context.CustomUserRoles.Add(userRole);
                            }
                        }
                        else
                        {
                            logger?.LogWarning("Failed to create test user '{Email}': {Errors}", 
                                email, string.Join(", ", result.Errors.Select(e => e.Description)));
                        }
                    }
                    else
                    {
                        logger?.LogInformation("Test user '{Email}' already exists.", email);
                    }
                }
                
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error seeding users.");
                throw;
            }
        }

        private static async Task SeedSubscriptionPlansAsync(ApplicationDbContext context, ILogger? logger)
        {
            logger?.LogInformation("Seeding subscription plans...");

            try
            {

                // Seed Subscription Plans
                if (!await context.SubscriptionPlans.AnyAsync())
                {
                    logger?.LogInformation("Creating subscription plans...");

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
                        IsVisible = true,
                        ShowOnLanding = true, // Display on landing page
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
                        IsVisible = true,
                        ShowOnLanding = true, // Display on landing page (Most Popular)
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
                        IsVisible = true,
                        ShowOnLanding = true, // Display on landing page
                        CreatedAt = DateTime.UtcNow
                    }
                    };

                    context.SubscriptionPlans.AddRange(plans);
                    await context.SaveChangesAsync();
                    logger?.LogInformation("Successfully created {Count} subscription plans.", plans.Count);
                }
                else
                {
                    logger?.LogInformation("Subscription plans already exist.");
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error seeding subscription plans.");
                throw;
            }
        }

        private static async Task SeedMessageProvidersAsync(ApplicationDbContext context, ILogger? logger)
        {
            logger?.LogInformation("Seeding message providers...");

            try
            {

                // Seed Message Providers
                if (!await context.MessageProviders.AnyAsync())
                {
                    logger?.LogInformation("Creating message providers...");

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
                    logger?.LogInformation("Successfully created {Count} message providers.", providers.Count);
                }
                else
                {
                    logger?.LogInformation("Message providers already exist.");
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error seeding message providers.");
                throw;
            }
        }

        private static async Task SeedChannelRoutingConfigsAsync(ApplicationDbContext context, ILogger? logger)
        {
            logger?.LogInformation("Seeding channel routing configurations...");

            try
            {

                // Seed Channel Routing Configurations
                if (!await context.ChannelRoutingConfigs.AnyAsync())
                {
                    logger?.LogInformation("Creating channel routing configurations...");

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
                    logger?.LogInformation("Successfully created {Count} channel routing configurations.", routingConfigs.Count);
                }
                else
                {
                    logger?.LogInformation("Channel routing configurations already exist.");
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error seeding channel routing configurations.");
                throw;
            }
        }

        private static async Task SeedPricingModelsAsync(ApplicationDbContext context, ILogger? logger)
        {
            logger?.LogInformation("Seeding pricing models...");

            try
            {

                // Seed Pricing Models for Landing Page
                if (!await context.PricingModels.AnyAsync())
                {
                    logger?.LogInformation("Creating pricing models...");

                    var pricingModels = new List<PricingModel>
                {
                    new PricingModel
                    {
                        Name = "Starter",
                        Description = "Perfect for small businesses getting started",
                        Type = PricingModelType.Flat,
                        BasePrice = 29.00m,
                        BillingPeriod = BillingPeriod.Monthly,
                        IsActive = true,
                        Priority = 1,
                        Configuration = "{\"features\":[\"1,000 SMS messages/month\",\"500 emails/month\",\"Basic analytics\",\"Email support\",\"1 user\"]}",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PricingModel
                    {
                        Name = "Professional",
                        Description = "For growing businesses with larger audiences",
                        Type = PricingModelType.Flat,
                        BasePrice = 99.00m,
                        BillingPeriod = BillingPeriod.Monthly,
                        IsActive = true,
                        Priority = 2,
                        Configuration = "{\"features\":[\"10,000 SMS messages/month\",\"5,000 emails/month\",\"Advanced analytics\",\"Priority support\",\"5 users\",\"Custom templates\",\"Automation workflows\"],\"isPopular\":true}",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PricingModel
                    {
                        Name = "Enterprise",
                        Description = "For large organizations with custom needs",
                        Type = PricingModelType.Flat,
                        BasePrice = 299.00m,
                        BillingPeriod = BillingPeriod.Monthly,
                        IsActive = true,
                        Priority = 3,
                        Configuration = "{\"features\":[\"Unlimited SMS messages\",\"Unlimited emails\",\"Advanced analytics & reporting\",\"24/7 phone support\",\"Unlimited users\",\"Custom branding\",\"API access\",\"Dedicated account manager\"]}",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                    };

                    context.PricingModels.AddRange(pricingModels);
                    await context.SaveChangesAsync();
                    logger?.LogInformation("Successfully created {Count} pricing models.", pricingModels.Count);
                }
                else
                {
                    logger?.LogInformation("Pricing models already exist.");
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error seeding pricing models.");
                throw;
            }
        }

        private static async Task SeedLandingPageSettingsAsync(ApplicationDbContext context, ILogger? logger)
        {
            logger?.LogInformation("Seeding landing page settings...");

            try
            {

                // Seed Landing Page Configuration Settings
                if (!await context.PlatformSettings.AnyAsync(s => s.Category == "LandingPage"))
                {
                    logger?.LogInformation("Creating landing page settings...");

                    var landingPageSettings = new List<PlatformSetting>
                {
                    // Hero Section Settings
                    new PlatformSetting
                    {
                        Key = "LandingPage.Hero.Type",
                        Value = "banner",
                        Category = "LandingPage",
                        Description = "Hero section type: 'banner' or 'slider'",
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Hero.Title",
                        Value = "Transform Your Marketing with SMS, MMS & Email",
                        Category = "LandingPage",
                        Description = "Hero section main title",
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Hero.Subtitle",
                        Value = "A powerful, enterprise-grade marketing platform to reach your customers where they are. Send targeted campaigns, track performance, and grow your business.",
                        Category = "LandingPage",
                        Description = "Hero section subtitle/description",
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Hero.BannerImage",
                        Value = "/images/hero-banner.jpg",
                        Category = "LandingPage",
                        Description = "Hero banner image URL",
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Hero.CTAText",
                        Value = "Get Started Free",
                        Category = "LandingPage",
                        Description = "Primary call-to-action button text",
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Hero.CTALink",
                        Value = "/Auth/Register",
                        Category = "LandingPage",
                        Description = "Primary call-to-action button link",
                        DataType = SettingDataType.String,
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
                        DataType = SettingDataType.Json,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Slider.AutoPlay",
                        Value = "true",
                        Category = "LandingPage",
                        Description = "Enable slider auto-play",
                        DataType = SettingDataType.Boolean,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Slider.Interval",
                        Value = "5000",
                        Category = "LandingPage",
                        Description = "Slider auto-play interval in milliseconds",
                        DataType = SettingDataType.Integer,
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
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Menu.TextColor",
                        Value = "#212529",
                        Category = "LandingPage",
                        Description = "Navigation menu text color",
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Menu.HoverColor",
                        Value = "#667eea",
                        Category = "LandingPage",
                        Description = "Navigation menu hover color",
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Menu.FontSize",
                        Value = "16",
                        Category = "LandingPage",
                        Description = "Navigation menu font size (in pixels)",
                        DataType = SettingDataType.Integer,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Menu.Position",
                        Value = "top",
                        Category = "LandingPage",
                        Description = "Navigation menu position: 'top' or 'fixed'",
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Menu.Items",
                        Value = "[{\"text\":\"Home\",\"link\":\"#home\",\"order\":1},{\"text\":\"Features\",\"link\":\"#features\",\"order\":2},{\"text\":\"Pricing\",\"link\":\"#pricing\",\"order\":3},{\"text\":\"Contact\",\"link\":\"#contact\",\"order\":4},{\"text\":\"Login\",\"link\":\"/Auth/Login\",\"order\":5,\"class\":\"btn-outline-primary\"}]",
                        Category = "LandingPage",
                        Description = "Navigation menu items (JSON array)",
                        DataType = SettingDataType.Json,
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
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Theme.SecondaryColor",
                        Value = "#764ba2",
                        Category = "LandingPage",
                        Description = "Secondary theme color",
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Theme.AccentColor",
                        Value = "#f093fb",
                        Category = "LandingPage",
                        Description = "Accent theme color",
                        DataType = SettingDataType.String,
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
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Company.Logo",
                        Value = "/images/logo.png",
                        Category = "LandingPage",
                        Description = "Company logo URL",
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Company.Tagline",
                        Value = "SMS, MMS & Email Marketing Platform",
                        Category = "LandingPage",
                        Description = "Company tagline",
                        DataType = SettingDataType.String,
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
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Stats.DeliveryRate",
                        Value = "98%",
                        Category = "LandingPage",
                        Description = "Delivery rate statistic",
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Stats.ActiveUsers",
                        Value = "5K+",
                        Category = "LandingPage",
                        Description = "Active users statistic",
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Stats.Support",
                        Value = "24/7",
                        Category = "LandingPage",
                        Description = "Support availability statistic",
                        DataType = SettingDataType.String,
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
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Footer.SocialLinks",
                        Value = "[{\"platform\":\"facebook\",\"url\":\"https://facebook.com/marketingplatform\",\"icon\":\"bi-facebook\"},{\"platform\":\"twitter\",\"url\":\"https://twitter.com/marketingplatform\",\"icon\":\"bi-twitter\"},{\"platform\":\"linkedin\",\"url\":\"https://linkedin.com/company/marketingplatform\",\"icon\":\"bi-linkedin\"},{\"platform\":\"instagram\",\"url\":\"https://instagram.com/marketingplatform\",\"icon\":\"bi-instagram\"}]",
                        Category = "LandingPage",
                        Description = "Footer social media links (JSON array)",
                        DataType = SettingDataType.Json,
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
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.SEO.Description",
                        Value = "Transform your marketing with our enterprise-grade SMS, MMS & Email platform. Powerful automation, advanced analytics, and seamless integration.",
                        Category = "LandingPage",
                        Description = "Meta description for SEO",
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.SEO.Keywords",
                        Value = "SMS marketing, email marketing, MMS marketing, marketing automation, campaign management",
                        Category = "LandingPage",
                        Description = "Meta keywords for SEO",
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },

                    // Features Section
                    new PlatformSetting
                    {
                        Key = "LandingPage.Features.SectionTitle",
                        Value = "Powerful Features for Modern Marketing",
                        Category = "LandingPage",
                        Description = "Features section title",
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Features.SectionSubtitle",
                        Value = "Everything you need to create, manage, and optimize your campaigns",
                        Category = "LandingPage",
                        Description = "Features section subtitle",
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Features.List",
                        Value = "[{\"icon\":\"bi-broadcast\",\"title\":\"Multi-Channel Campaigns\",\"description\":\"Send SMS, MMS, and Email campaigns from one unified platform. Reach your audience on their preferred channels.\",\"color\":\"primary\"},{\"icon\":\"bi-graph-up-arrow\",\"title\":\"Advanced Analytics\",\"description\":\"Track campaign performance in real-time with detailed analytics and reporting. Make data-driven decisions to optimize your results.\",\"color\":\"success\"},{\"icon\":\"bi-clock-history\",\"title\":\"Automation & Scheduling\",\"description\":\"Schedule campaigns in advance and automate your marketing workflows. Save time and improve efficiency.\",\"color\":\"info\"},{\"icon\":\"bi-people\",\"title\":\"Contact Management\",\"description\":\"Organize your contacts with dynamic groups and tags. Segment your audience for targeted messaging.\",\"color\":\"warning\"},{\"icon\":\"bi-file-earmark-text\",\"title\":\"Template Library\",\"description\":\"Create reusable message templates with dynamic variables. Personalize content at scale.\",\"color\":\"danger\"},{\"icon\":\"bi-shield-check\",\"title\":\"Compliance & Security\",\"description\":\"Stay compliant with GDPR, CAN-SPAM, and TCPA regulations. Enterprise-grade security for your data.\",\"color\":\"secondary\"}]",
                        Category = "LandingPage",
                        Description = "Features list (JSON array)",
                        DataType = SettingDataType.Json,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },

                    // Pricing Section
                    new PlatformSetting
                    {
                        Key = "LandingPage.Pricing.SectionTitle",
                        Value = "Simple, Transparent Pricing",
                        Category = "LandingPage",
                        Description = "Pricing section title",
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Pricing.SectionSubtitle",
                        Value = "Choose the plan that fits your business needs",
                        Category = "LandingPage",
                        Description = "Pricing section subtitle",
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Pricing.ShowYearlyToggle",
                        Value = "true",
                        Category = "LandingPage",
                        Description = "Show monthly/yearly pricing toggle",
                        DataType = SettingDataType.Boolean,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },

                    // CTA Section
                    new PlatformSetting
                    {
                        Key = "LandingPage.CTA.Title",
                        Value = "Ready to Transform Your Marketing?",
                        Category = "LandingPage",
                        Description = "Call-to-action section title",
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.CTA.Subtitle",
                        Value = "Join thousands of businesses using our platform to grow their reach",
                        Category = "LandingPage",
                        Description = "Call-to-action section subtitle",
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.CTA.ButtonText",
                        Value = "Start Free Trial",
                        Category = "LandingPage",
                        Description = "Call-to-action button text",
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.CTA.ButtonLink",
                        Value = "/Auth/Register",
                        Category = "LandingPage",
                        Description = "Call-to-action button link",
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.CTA.BackgroundColor",
                        Value = "#667eea",
                        Category = "LandingPage",
                        Description = "Call-to-action section background color",
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },

                    // Contact Section
                    new PlatformSetting
                    {
                        Key = "LandingPage.Contact.Email",
                        Value = "support@marketingplatform.com",
                        Category = "LandingPage",
                        Description = "Contact email address",
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Contact.Phone",
                        Value = "+1 (555) 123-4567",
                        Category = "LandingPage",
                        Description = "Contact phone number",
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Contact.Address",
                        Value = "123 Marketing Street, San Francisco, CA 94102",
                        Category = "LandingPage",
                        Description = "Company address",
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },

                    // Testimonials Section
                    new PlatformSetting
                    {
                        Key = "LandingPage.Testimonials.ShowSection",
                        Value = "true",
                        Category = "LandingPage",
                        Description = "Show testimonials section",
                        DataType = SettingDataType.Boolean,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Testimonials.SectionTitle",
                        Value = "What Our Customers Say",
                        Category = "LandingPage",
                        Description = "Testimonials section title",
                        DataType = SettingDataType.String,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PlatformSetting
                    {
                        Key = "LandingPage.Testimonials.List",
                        Value = "[{\"name\":\"John Smith\",\"company\":\"TechCorp Inc.\",\"position\":\"Marketing Director\",\"testimonial\":\"This platform has transformed how we communicate with our customers. The automation features alone have saved us countless hours.\",\"rating\":5,\"image\":\"/images/testimonials/john.jpg\"},{\"name\":\"Sarah Johnson\",\"company\":\"E-commerce Plus\",\"position\":\"CEO\",\"testimonial\":\"Outstanding service and support. Our email campaigns have never performed better. Highly recommended!\",\"rating\":5,\"image\":\"/images/testimonials/sarah.jpg\"},{\"name\":\"Michael Chen\",\"company\":\"Retail Solutions\",\"position\":\"Operations Manager\",\"testimonial\":\"The multi-channel approach is exactly what we needed. We can now reach our customers on their preferred platforms seamlessly.\",\"rating\":5,\"image\":\"/images/testimonials/michael.jpg\"}]",
                        Category = "LandingPage",
                        Description = "Testimonials list (JSON array)",
                        DataType = SettingDataType.Json,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                    };

                    context.PlatformSettings.AddRange(landingPageSettings);
                    await context.SaveChangesAsync();
                    logger?.LogInformation("Successfully created {Count} landing page settings.", landingPageSettings.Count);
                }
                else
                {
                    logger?.LogInformation("Landing page settings already exist.");
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error seeding landing page settings.");
                throw;
            }
        }

        private static async Task SeedPageContentAsync(ApplicationDbContext context, ILogger? logger)
        {
            logger?.LogInformation("Seeding page content (Privacy Policy and Terms of Service)...");

            try
            {
                // Seed Privacy Policy if it doesn't exist
                var existingPrivacy = await context.PageContents.FirstOrDefaultAsync(p => p.PageKey == "privacy-policy");
                if (existingPrivacy == null)
                {
                    logger?.LogInformation("Creating Privacy Policy page content...");

                    var privacy = new PageContent
                    {
                        PageKey = "privacy-policy",
                        Title = "Privacy Policy",
                        MetaDescription = "Learn how we collect, use, and protect your personal information.",
                        Content = @"
<h2>1. Information We Collect</h2>
<p>We collect information that you provide directly to us, including:</p>
<ul>
    <li>Name and contact information (email address, phone number)</li>
    <li>Account credentials</li>
    <li>Payment information</li>
    <li>Communication preferences</li>
    <li>Campaign and marketing data</li>
</ul>

<h2>2. How We Use Your Information</h2>
<p>We use the information we collect to:</p>
<ul>
    <li>Provide, maintain, and improve our services</li>
    <li>Process transactions and send related information</li>
    <li>Send technical notices, updates, security alerts, and support messages</li>
    <li>Respond to your comments, questions, and customer service requests</li>
    <li>Monitor and analyze trends, usage, and activities</li>
</ul>

<h2>3. Data Security</h2>
<p>We implement appropriate technical and organizational measures to protect your personal data against unauthorized or unlawful processing, accidental loss, destruction, or damage. This includes encryption of sensitive data, regular security assessments, and access controls.</p>

<h2>4. Data Retention</h2>
<p>We retain your personal data for as long as necessary to provide our services, comply with legal obligations, resolve disputes, and enforce our agreements.</p>

<h2>5. Your Rights</h2>
<p>You have the right to:</p>
<ul>
    <li>Access your personal data</li>
    <li>Correct inaccurate data</li>
    <li>Request deletion of your data</li>
    <li>Object to processing of your data</li>
    <li>Request data portability</li>
    <li>Withdraw consent at any time</li>
</ul>

<h2>6. Contact Us</h2>
<p>If you have any questions about this Privacy Policy or our data practices, please contact us at privacy@marketingplatform.com</p>

<p><em>Last updated: January 2024</em></p>
",
                        IsPublished = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    context.PageContents.Add(privacy);
                    logger?.LogInformation("Privacy Policy page content created.");
                }
                else
                {
                    logger?.LogInformation("Privacy Policy already exists.");
                }

                // Seed Terms of Service if it doesn't exist
                var existingTerms = await context.PageContents.FirstOrDefaultAsync(p => p.PageKey == "terms-of-service");
                if (existingTerms == null)
                {
                    logger?.LogInformation("Creating Terms of Service page content...");

                    var terms = new PageContent
                    {
                        PageKey = "terms-of-service",
                        Title = "Terms of Service",
                        MetaDescription = "Read our terms of service and user agreement.",
                        Content = @"
<h2>1. Acceptance of Terms</h2>
<p>By accessing and using Marketing Platform (""the Service""), you accept and agree to be bound by the terms and provisions of this agreement. If you do not agree to these terms, please do not use the Service.</p>

<h2>2. Use License</h2>
<p>Permission is granted to access and use the Service for legitimate business purposes. This license shall automatically terminate if you violate any of these restrictions.</p>

<h2>3. Account Terms</h2>
<p>When you create an account with us, you must provide accurate and complete information. You are responsible for:</p>
<ul>
    <li>Maintaining the security of your account and password</li>
    <li>All activities that occur under your account</li>
    <li>Immediately notifying us of any unauthorized use</li>
    <li>Ensuring your use complies with all applicable laws</li>
</ul>

<h2>4. Service Availability</h2>
<p>We strive to provide a reliable service, but we do not guarantee that:</p>
<ul>
    <li>The Service will be uninterrupted, timely, secure, or error-free</li>
    <li>Any errors or defects will be corrected</li>
    <li>The Service is free of viruses or other harmful components</li>
</ul>

<h2>5. Prohibited Uses</h2>
<p>You may not use our Service:</p>
<ul>
    <li>For any unlawful purpose or to violate any laws</li>
    <li>To send spam, unsolicited messages, or illegal content</li>
    <li>To transmit malware or other harmful code</li>
    <li>To interfere with or disrupt the Service or servers</li>
    <li>To impersonate any person or entity</li>
</ul>

<h2>6. Intellectual Property</h2>
<p>The Service and its original content, features, and functionality are owned by Marketing Platform and are protected by international copyright, trademark, and other intellectual property laws.</p>

<h2>7. Termination</h2>
<p>We may terminate or suspend your account and access to the Service immediately, without prior notice or liability, for any reason, including if you breach these Terms.</p>

<h2>8. Limitation of Liability</h2>
<p>In no event shall Marketing Platform be liable for any indirect, incidental, special, consequential, or punitive damages resulting from your use of or inability to use the Service.</p>

<h2>9. Changes to Terms</h2>
<p>We reserve the right to modify or replace these Terms at any time. If a revision is material, we will provide at least 30 days' notice prior to any new terms taking effect.</p>

<h2>10. Contact Information</h2>
<p>If you have any questions about these Terms, please contact us at legal@marketingplatform.com</p>

<p><em>Last updated: January 2024</em></p>
",
                        IsPublished = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    context.PageContents.Add(terms);
                    logger?.LogInformation("Terms of Service page content created.");
                }
                else
                {
                    logger?.LogInformation("Terms of Service already exists.");
                }

                await context.SaveChangesAsync();
                logger?.LogInformation("Page content seeding completed.");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error seeding page content.");
                throw;
            }
        }
    }
}
