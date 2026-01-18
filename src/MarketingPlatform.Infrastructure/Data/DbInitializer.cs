using MarketingPlatform.Core.Entities;
using MarketingPlatform.Core.Enums;
using Microsoft.AspNetCore.Identity;

namespace MarketingPlatform.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Seed Roles
            string[] roles = { "Admin", "User", "Manager" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
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
                    await userManager.AddToRoleAsync(adminUser, "Admin");
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
        }
    }
}
