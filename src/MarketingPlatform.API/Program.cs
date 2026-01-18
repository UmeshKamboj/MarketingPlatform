using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MarketingPlatform.Infrastructure.Data;
using MarketingPlatform.Core.Entities;
using Microsoft.AspNetCore.Identity;
using MarketingPlatform.Application.Interfaces;
using MarketingPlatform.Infrastructure.Services;
using MarketingPlatform.Application.Services;
using MarketingPlatform.Core.Interfaces.Repositories;
using MarketingPlatform.Infrastructure.Repositories;
using MarketingPlatform.Application.Mappings;
using MarketingPlatform.API.Middleware;
using Serilog;
using Hangfire;
using Hangfire.SqlServer;
using MarketingPlatform.Core.Interfaces;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build())
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.Host.UseSerilog();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Encryption Services - Configuration-based by default with Azure/AWS options
var encryptionProvider = builder.Configuration["Encryption:Provider"] ?? "Configuration";
switch (encryptionProvider.ToLowerInvariant())
{
    case "azure":
        // Azure Key Vault integration (requires Azure.Security.KeyVault.Keys package)
        // builder.Services.AddSingleton<IKeyManagementService, AzureKeyVaultService>();
        throw new NotImplementedException("Azure Key Vault integration requires Azure.Security.KeyVault.Keys package. Install and uncomment the registration.");
    
    case "aws":
        // AWS KMS integration (requires AWSSDK.KeyManagementService package)
        // builder.Services.AddSingleton<IKeyManagementService, AwsKmsService>();
        throw new NotImplementedException("AWS KMS integration requires AWSSDK.KeyManagementService package. Install and uncomment the registration.");
    
    case "configuration":
    default:
        builder.Services.AddSingleton<IKeyManagementService, ConfigurationKeyManagementService>();
        break;
}

builder.Services.AddScoped<IEncryptionService, AesEncryptionService>();

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!))
    };
});

// Authorization with Permission-based policies
builder.Services.AddAuthorization(options =>
{
    // Register authorization handler
    foreach (MarketingPlatform.Core.Enums.Permission permission in Enum.GetValues(typeof(MarketingPlatform.Core.Enums.Permission)))
    {
        if (permission != MarketingPlatform.Core.Enums.Permission.All)
        {
            options.AddPolicy($"Permission:{permission}", policy =>
                policy.Requirements.Add(new MarketingPlatform.API.Authorization.PermissionRequirement(permission)));
        }
    }
});

builder.Services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, MarketingPlatform.API.Authorization.PermissionAuthorizationHandler>();

// Repository Pattern
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();

// Application Services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IContactGroupService, ContactGroupService>();
builder.Services.AddScoped<IDynamicGroupEvaluationService, DynamicGroupEvaluationService>();
builder.Services.AddScoped<ICampaignService, CampaignService>();
builder.Services.AddScoped<ICampaignABTestingService, CampaignABTestingService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<ISuppressionListService, SuppressionListService>();
builder.Services.AddScoped<IContactTagService, ContactTagService>();
builder.Services.AddScoped<IAudienceSegmentationService, AudienceSegmentationService>();
builder.Services.AddScoped<IKeywordService, KeywordService>();
builder.Services.AddScoped<IUrlShortenerService, UrlShortenerService>();
builder.Services.AddScoped<IComplianceService, ComplianceService>();

// Scheduling & Automation Services
builder.Services.AddScoped<ICampaignSchedulerService, CampaignSchedulerService>();
builder.Services.AddScoped<IWorkflowService, WorkflowService>();
builder.Services.AddScoped<IEventTriggerService, EventTriggerService>();
builder.Services.AddScoped<IRateLimitService, RateLimitService>();

// Message Service
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IMessageRoutingService, MessageRoutingService>();

// Analytics & Reporting Services
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IReportExportService, ReportExportService>();

// Integration Services
builder.Services.AddScoped<IWebhookService, WebhookService>();
builder.Services.AddScoped<ICRMIntegrationService, CRMIntegrationService>();

// Provider Services (Mock implementations)
builder.Services.AddScoped<ISMSProvider, MockSMSProvider>();
builder.Services.AddScoped<IMMSProvider, MockMMSProvider>();
builder.Services.AddScoped<IEmailProvider, MockEmailProvider>();

// TODO: Replace with real providers in production:
// builder.Services.AddScoped<ISMSProvider, TwilioSMSProvider>();
// builder.Services.AddScoped<IMMSProvider, TwilioMMSProvider>();
// builder.Services.AddScoped<IEmailProvider, SendGridEmailProvider>();

// Background Services
builder.Services.AddHostedService<MessageQueueProcessor>();
builder.Services.AddHostedService<DynamicGroupUpdateProcessor>();

// Hangfire Configuration - Optimized to avoid DB locks and reduce load
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.FromSeconds(15), // Reduce polling frequency to minimize DB load
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true, // IMPORTANT: Disable global locks to prevent table locking
        PrepareSchemaIfNecessary = true,
        SchemaName = "hangfire"
    }));

// Add Hangfire server with optimized settings
builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = 5; // Limit worker count to control DB connections
    options.Queues = new[] { "default", "campaigns", "workflows", "rate-limits" }; // Separate queues
    options.ServerCheckInterval = TimeSpan.FromMinutes(1);
    options.HeartbeatInterval = TimeSpan.FromSeconds(30);
    options.ServerTimeout = TimeSpan.FromMinutes(5);
    options.SchedulePollingInterval = TimeSpan.FromSeconds(15); // Reduce schedule polling to minimize DB load
});

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Marketing Platform API",
        Version = "v1",
        Description = "RESTful API for Marketing Platform - SMS, MMS & Email Marketing Solution"
    });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Security Headers Middleware - Add before exception handling
app.Use(async (context, next) =>
{
    // Remove server header for security
    context.Response.Headers.Remove("Server");
    
    // Add security headers
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'");
    
    await next();
});

// Exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Rate limiting middleware (before authentication)
app.UseMiddleware<RateLimitingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS Redirection - Enforce TLS 1.2+
app.UseHttpsRedirection();

// HSTS (HTTP Strict Transport Security) - Enforces HTTPS for specified duration
if (builder.Configuration.GetValue<bool>("Security:EnableHSTS", true))
{
    var hstsMaxAge = builder.Configuration.GetValue<int>("Security:HSTSMaxAge", 31536000); // Default: 1 year
    var includeSubDomains = builder.Configuration.GetValue<bool>("Security:IncludeSubDomains", true);
    
    app.UseHsts();
    app.Use(async (context, next) =>
    {
        var hstsHeader = $"max-age={hstsMaxAge}";
        if (includeSubDomains)
            hstsHeader += "; includeSubDomains";
        
        context.Response.Headers.Append("Strict-Transport-Security", hstsHeader);
        await next();
    });
}

// Hangfire Dashboard with authorization
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireDashboardAuthorizationFilter() },
    StatsPollingInterval = 30000 // Poll every 30 seconds instead of default to reduce DB load
});

app.UseAuthentication();
app.UseAuthorization();

// Configure recurring jobs for rate limit resets using service provider
using (var scope = app.Services.CreateScope())
{
    var rateLimitService = scope.ServiceProvider.GetRequiredService<IRateLimitService>();
    var eventTriggerService = scope.ServiceProvider.GetRequiredService<IEventTriggerService>();
    
    RecurringJob.AddOrUpdate(
        "reset-daily-rate-limits",
        () => rateLimitService.ResetDailyCountersAsync(),
        Cron.Daily(0, 0)); // Run at midnight UTC

    RecurringJob.AddOrUpdate(
        "reset-weekly-rate-limits",
        () => rateLimitService.ResetWeeklyCountersAsync(),
        Cron.Weekly(DayOfWeek.Monday, 0, 0)); // Run at midnight Monday UTC

    RecurringJob.AddOrUpdate(
        "reset-monthly-rate-limits",
        () => rateLimitService.ResetMonthlyCountersAsync(),
        Cron.Monthly(1, 0, 0)); // Run at midnight on 1st of month UTC

    RecurringJob.AddOrUpdate(
        "check-inactivity-triggers",
        () => eventTriggerService.CheckInactivityTriggersAsync(),
        Cron.Daily(2, 0)); // Run at 2 AM UTC daily
}

app.MapControllers();

// Database seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        
        await context.Database.MigrateAsync();
        await DbInitializer.SeedAsync(context, userManager, roleManager);
        
        Log.Information("Database migrated and seeded successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();

// Hangfire Dashboard Authorization Filter
public class HangfireDashboardAuthorizationFilter : Hangfire.Dashboard.IDashboardAuthorizationFilter
{
    public bool Authorize(Hangfire.Dashboard.DashboardContext context)
    {
        // Development: Allow all access for testing
        // Production: Implement proper authentication
        // TODO: Replace with proper auth check that validates user is authenticated and has admin role
        return true;
    }
}
