# Repository-Service Pattern Implementation

## Overview

The Marketing Platform follows a clean architecture approach with a well-defined **Repository-Service** pattern that separates concerns between data access and business logic.

## Pattern Structure

```
┌─────────────┐
│ Controllers │ ← API Layer (HTTP requests/responses, validation)
└──────┬──────┘
       │ depends on
┌──────▼──────┐
│  Services   │ ← Business Logic Layer (orchestration, rules, transactions)
└──────┬──────┘
       │ depends on
┌──────▼───────┐
│ Repositories │ ← Data Access Layer (CRUD, queries, persistence)
└──────┬───────┘
       │ depends on
┌──────▼──────┐
│   Entities  │ ← Domain Models
└─────────────┘
```

## Architectural Principles

### 1. Separation of Concerns
- **Controllers**: Handle HTTP requests/responses, parameter binding, and return appropriate status codes
- **Services**: Implement business logic, validate business rules, and coordinate operations
- **Repositories**: Abstract data access, provide CRUD operations, and manage entity persistence

### 2. Dependency Inversion
- Controllers depend on Service interfaces (not implementations)
- Services depend on Repository interfaces (not implementations)
- All dependencies are injected via constructors

### 3. Single Responsibility
- Each layer has a specific purpose
- Controllers don't contain business logic
- Services don't perform direct data access
- Repositories don't contain business rules

## Implementation Details

### Generic Repository Pattern

The platform uses a **generic `IRepository<T>` interface** for optimal performance and reduced code duplication:

```csharp
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<List<T>> GetAllAsync();
    IQueryable<T> GetQueryable();
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
}
```

**Benefits**:
- ✅ **Performance**: No additional abstraction overhead
- ✅ **Flexibility**: LINQ queries via `GetQueryable()`
- ✅ **Simplicity**: No need for custom repositories for each entity
- ✅ **Testability**: Easy to mock for unit tests

### Unit of Work Pattern

All data modifications go through the `IUnitOfWork` to ensure transactional consistency:

```csharp
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
```

## Service Layer Implementation

### Service Interface Example

```csharp
public interface IMessageRoutingService
{
    // Core Business Operations
    Task<(bool Success, string? ExternalId, string? Error, decimal? Cost, int AttemptNumber)> 
        RouteMessageAsync(CampaignMessage message);
    
    // Configuration Management (delegated from controller)
    Task<List<ChannelRoutingConfig>> GetAllConfigsAsync();
    Task<ChannelRoutingConfig?> GetConfigByIdAsync(int id);
    Task<ChannelRoutingConfig> CreateConfigAsync(ChannelRoutingConfig config);
    Task<ChannelRoutingConfig?> UpdateConfigAsync(int id, ChannelRoutingConfig updatedConfig);
    Task<bool> DeleteConfigAsync(int id);
    
    // Analytics & Reporting
    Task<object> GetChannelStatsAsync(ChannelType channel, DateTime? startDate, DateTime? endDate);
}
```

### Service Implementation Example

```csharp
public class MessageRoutingService : IMessageRoutingService
{
    private readonly IRepository<ChannelRoutingConfig> _routingConfigRepository;
    private readonly IRepository<MessageDeliveryAttempt> _deliveryAttemptRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISMSProvider _smsProvider;
    private readonly ILogger<MessageRoutingService> _logger;

    public MessageRoutingService(
        IRepository<ChannelRoutingConfig> routingConfigRepository,
        IRepository<MessageDeliveryAttempt> deliveryAttemptRepository,
        IUnitOfWork unitOfWork,
        ISMSProvider smsProvider,
        ILogger<MessageRoutingService> logger)
    {
        _routingConfigRepository = routingConfigRepository;
        _deliveryAttemptRepository = deliveryAttemptRepository;
        _unitOfWork = unitOfWork;
        _smsProvider = smsProvider;
        _logger = logger;
    }

    public async Task<ChannelRoutingConfig> CreateConfigAsync(ChannelRoutingConfig config)
    {
        // Business logic: Set timestamps
        config.CreatedAt = DateTime.UtcNow;
        
        // Data access through repository
        await _routingConfigRepository.AddAsync(config);
        await _unitOfWork.SaveChangesAsync();
        
        return config;
    }
    
    // More methods...
}
```

## Controller Layer Implementation

Controllers are thin and delegate all business logic to services:

```csharp
[ApiController]
[Route("api/[controller]")]
public class RoutingConfigController : ControllerBase
{
    private readonly IMessageRoutingService _messageRoutingService;
    private readonly ILogger<RoutingConfigController> _logger;

    public RoutingConfigController(
        IMessageRoutingService messageRoutingService,
        ILogger<RoutingConfigController> logger)
    {
        _messageRoutingService = messageRoutingService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<List<ChannelRoutingConfig>>>> GetAllConfigs()
    {
        var configs = await _messageRoutingService.GetAllConfigsAsync();
        return Ok(ApiResponse<List<ChannelRoutingConfig>>.SuccessResponse(configs, 
            "Routing configurations retrieved successfully"));
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<ChannelRoutingConfig>>> CreateConfig(
        [FromBody] ChannelRoutingConfig config)
    {
        try
        {
            var createdConfig = await _messageRoutingService.CreateConfigAsync(config);
            return Ok(ApiResponse<ChannelRoutingConfig>.SuccessResponse(createdConfig, 
                "Routing configuration created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating routing configuration");
            return StatusCode(500, ApiResponse<ChannelRoutingConfig>.ErrorResponse(
                "An error occurred while creating the routing configuration"));
        }
    }
}
```

## Service Registration

All services are registered in `Program.cs`:

```csharp
// Generic Repository Pattern
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Custom Repositories (only when additional methods are needed)
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IExternalAuthProviderRepository, ExternalAuthProviderRepository>();

// Application Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<ICampaignService, CampaignService>();
builder.Services.AddScoped<IMessageRoutingService, MessageRoutingService>();
builder.Services.AddScoped<IOAuth2Service, OAuth2Service>();
// ... more services
```

## Current Implementation Status

### ✅ Controllers Following Pattern (22/22)

All controllers now properly use the service layer:

1. **AuthController** → AuthService
2. **OAuth2Controller** → OAuth2Service
3. **UsersController** → UserService
4. **RolesController** → RoleService
5. **ContactsController** → ContactService
6. **ContactGroupsController** → ContactGroupService
7. **ContactTagsController** → ContactTagService
8. **CampaignsController** → CampaignService, CampaignABTestingService
9. **MessagesController** → MessageService
10. **TemplatesController** → TemplateService
11. **RoutingConfigController** → MessageRoutingService *(refactored)*
12. **UrlsController** → UrlShortenerService
13. **KeywordsController** → KeywordService
14. **JourneysController** → WorkflowService
15. **AnalyticsController** → AnalyticsService
16. **AudienceController** → AudienceSegmentationService
17. **ComplianceController** → ComplianceService
18. **SuppressionListsController** → SuppressionListService
19. **RateLimitsController** → RateLimitService
20. **IntegrationController** → CRMIntegrationService
21. **WebhooksController** → WebhookService
22. **HealthController** → HealthService

### ✅ Services Using Generic Repositories (24/24)

All services use the optimized generic `IRepository<T>` pattern:

- CampaignService
- ContactService
- MessageService
- TemplateService
- ComplianceService
- SuppressionListService
- KeywordService
- ContactGroupService
- ContactTagService
- UrlShortenerService
- AnalyticsService
- AudienceSegmentationService
- WorkflowService
- EventTriggerService
- WebhookService
- CRMIntegrationService
- MessageRoutingService *(enhanced)*
- RateLimitService
- CampaignSchedulerService
- CampaignABTestingService
- DynamicGroupEvaluationService
- ReportExportService
- UserService
- RoleService

### Custom Repositories (4 specialized)

Custom repositories exist only when additional query methods are needed:

1. **IRoleRepository** - Role-specific queries (permissions, etc.)
2. **IUserRoleRepository** - User-role relationship queries
3. **IExternalAuthProviderRepository** - OAuth provider queries
4. **IUserExternalLoginRepository** - External login queries

## Benefits of Current Implementation

### 1. Performance ✅
- Generic repositories eliminate abstraction overhead
- Direct LINQ queries via `GetQueryable()` allow query optimization
- Entity Framework Core handles query translation efficiently

### 2. Maintainability ✅
- Clear separation of concerns
- Easy to locate business logic (in services)
- Easy to modify data access (in repositories)

### 3. Testability ✅
- Services can be unit tested with mocked repositories
- Controllers can be tested with mocked services
- Integration tests can use real repositories

### 4. Scalability ✅
- New features follow established patterns
- Generic repositories reduce boilerplate code
- Easy to add new entities and services

### 5. Flexibility ✅
- Services can coordinate multiple repositories
- Complex business rules stay in services
- Data access logic stays in repositories

## Best Practices

### DO ✅
- Use generic `IRepository<T>` for standard CRUD operations
- Put business logic in services
- Use `IUnitOfWork` for transactions
- Inject dependencies via constructors
- Return DTOs from services (not entities directly)
- Log operations appropriately
- Handle exceptions at the controller level

### DON'T ❌
- Access repositories directly from controllers
- Put business logic in controllers or repositories
- Create custom repositories unless absolutely necessary
- Expose `IQueryable` from services to controllers
- Return entities with navigation properties to API clients
- Swallow exceptions without logging

## Example: Adding a New Feature

When adding a new feature, follow these steps:

### 1. Create Entity
```csharp
public class NewsletterSubscription : BaseEntity
{
    public string Email { get; set; }
    public bool IsActive { get; set; }
    public DateTime SubscribedAt { get; set; }
}
```

### 2. Create DTOs
```csharp
public class NewsletterSubscriptionDto
{
    public int Id { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; }
}
```

### 3. Create Service Interface
```csharp
public interface INewsletterService
{
    Task<List<NewsletterSubscriptionDto>> GetAllSubscriptionsAsync();
    Task<NewsletterSubscriptionDto?> GetByIdAsync(int id);
    Task<NewsletterSubscriptionDto> SubscribeAsync(string email);
    Task<bool> UnsubscribeAsync(string email);
}
```

### 4. Implement Service
```csharp
public class NewsletterService : INewsletterService
{
    private readonly IRepository<NewsletterSubscription> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public NewsletterService(
        IRepository<NewsletterSubscription> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<NewsletterSubscriptionDto> SubscribeAsync(string email)
    {
        var subscription = new NewsletterSubscription
        {
            Email = email,
            IsActive = true,
            SubscribedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(subscription);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(subscription);
    }
}
```

### 5. Create Controller
```csharp
[ApiController]
[Route("api/[controller]")]
public class NewsletterController : ControllerBase
{
    private readonly INewsletterService _service;

    public NewsletterController(INewsletterService service)
    {
        _service = service;
    }

    [HttpPost("subscribe")]
    public async Task<ActionResult<ApiResponse<NewsletterSubscriptionDto>>> Subscribe(
        [FromBody] string email)
    {
        var result = await _service.SubscribeAsync(email);
        return Ok(ApiResponse<NewsletterSubscriptionDto>.SuccessResponse(result));
    }
}
```

### 6. Register Service
```csharp
// In Program.cs
builder.Services.AddScoped<INewsletterService, NewsletterService>();
```

## Testing Strategy

### Unit Tests (Services)
```csharp
public class NewsletterServiceTests
{
    [Fact]
    public async Task SubscribeAsync_Should_CreateSubscription()
    {
        // Arrange
        var mockRepository = new Mock<IRepository<NewsletterSubscription>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var service = new NewsletterService(mockRepository.Object, mockUnitOfWork.Object);

        // Act
        var result = await service.SubscribeAsync("test@example.com");

        // Assert
        mockRepository.Verify(r => r.AddAsync(It.IsAny<NewsletterSubscription>()), Times.Once);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
```

### Integration Tests (Controllers)
```csharp
public class NewsletterControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Subscribe_Should_Return_201Created()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.PostAsJsonAsync("/api/newsletter/subscribe", 
            "test@example.com");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
```

## Performance Considerations

### Generic Repository Benefits
- **No abstraction penalty**: Direct Entity Framework queries
- **Query optimization**: LINQ queries are optimized by EF Core
- **Reduced memory**: No unnecessary object mappings

### When to Create Custom Repositories
Only create custom repositories when you need:
- Complex multi-table joins that are reused
- Specialized query methods with business meaning
- Stored procedure calls
- Raw SQL queries

### Example: Custom Repository
```csharp
public interface IAnalyticsRepository : IRepository<CampaignAnalytics>
{
    Task<List<CampaignPerformanceDto>> GetCampaignPerformanceAsync(
        DateTime startDate, 
        DateTime endDate);
}
```

## Conclusion

The Marketing Platform's Repository-Service pattern provides a solid foundation for:
- Clean, maintainable code
- High performance with generic repositories
- Easy testing and mocking
- Scalable architecture
- Clear separation of concerns

All controllers now properly delegate to services, and all services use the optimized generic repository pattern. This ensures consistency, performance, and maintainability across the entire codebase.

## References

- **Controllers**: `/src/MarketingPlatform.API/Controllers/`
- **Services**: `/src/MarketingPlatform.Application/Services/`
- **Service Interfaces**: `/src/MarketingPlatform.Application/Interfaces/`
- **Repositories**: `/src/MarketingPlatform.Infrastructure/Repositories/`
- **Repository Interfaces**: `/src/MarketingPlatform.Core/Interfaces/Repositories/`
- **Entities**: `/src/MarketingPlatform.Core/Entities/`
