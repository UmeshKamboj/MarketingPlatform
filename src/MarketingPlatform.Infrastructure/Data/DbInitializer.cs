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
        }
    }
}
