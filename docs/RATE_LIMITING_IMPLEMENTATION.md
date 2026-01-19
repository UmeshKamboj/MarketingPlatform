# Rate Limiting & Throttling Implementation

This document describes the implementation of API rate limiting and throttling for the Marketing Platform.

## Overview

The rate limiting system provides comprehensive API throttling with support for:
- Per-user rate limiting
- Per-tenant rate limiting  
- Endpoint-specific limits
- Provider rate limiting (SMS, Email, MMS)
- Rate limit logging and monitoring
- Admin configuration via API

## Architecture

### Components

#### 1. Core Entities (`MarketingPlatform.Core.Entities`)

**ApiRateLimit**
- Stores rate limit rules for API endpoints
- Supports user-specific, tenant-specific, and global limits
- Tracks current request counts and time windows
- Configurable priority for rule precedence

**RateLimitLog**
- Logs all rate limit violations
- Captures user, endpoint, IP address, and violation details
- Provides audit trail for monitoring and analysis

**ProviderRateLimit**
- Tracks external provider rate limits
- Supports provider-specific limits (Twilio, SendGrid, etc.)
- Prevents exceeding provider API quotas

#### 2. Service Layer (`MarketingPlatform.Application`)

**IRateLimitService / RateLimitService**
- Implements rate limiting logic
- Methods for checking limits, recording requests, and logging violations
- Automatic window reset and counter management
- Pattern matching for endpoint rules

**Key Methods:**
- `CheckApiRateLimitAsync()` - Check if request is within limits
- `RecordApiRequestAsync()` - Record API request
- `LogRateLimitViolationAsync()` - Log rate limit violation
- `CheckProviderRateLimitAsync()` - Check provider limits
- `CreateApiRateLimitAsync()` - Create new rate limit rule
- `GetRateLimitLogsAsync()` - Retrieve violation logs

#### 3. Middleware (`MarketingPlatform.API.Middleware`)

**RateLimitingMiddleware**
- Intercepts API requests
- Checks rate limits before processing
- Returns HTTP 429 (Too Many Requests) when limits exceeded
- Adds standard rate limit headers to responses:
  - `X-RateLimit-Limit` - Maximum requests allowed
  - `X-RateLimit-Remaining` - Remaining requests in window
  - `X-RateLimit-Reset` - Unix timestamp when window resets
  - `Retry-After` - Seconds until client can retry

#### 4. API Controller (`MarketingPlatform.API.Controllers`)

**RateLimitsController**
- Admin endpoints for managing rate limits
- CRUD operations on rate limit rules
- View rate limit logs
- Check current rate limit status

**Endpoints:**
- `GET /api/ratelimits/status?endpoint={endpoint}` - Check current status
- `GET /api/ratelimits` - List all rate limits (admin)
- `POST /api/ratelimits` - Create new rule (admin)
- `PUT /api/ratelimits/{id}` - Update rule (admin)
- `DELETE /api/ratelimits/{id}` - Delete rule (admin)
- `GET /api/ratelimits/logs` - View violation logs (admin)

## Configuration

### Default Rate Limits (appsettings.json)

```json
{
  "RateLimiting": {
    "DefaultRules": [
      {
        "EndpointPattern": "/api/messages/bulk",
        "MaxRequests": 10,
        "TimeWindowSeconds": 60,
        "Description": "Bulk message operations - 10 per minute"
      },
      {
        "EndpointPattern": "/api/messages/*",
        "MaxRequests": 100,
        "TimeWindowSeconds": 60,
        "Description": "Message API - 100 per minute"
      },
      {
        "EndpointPattern": "/api/campaigns/*",
        "MaxRequests": 50,
        "TimeWindowSeconds": 60,
        "Description": "Campaign API - 50 per minute"
      },
      {
        "EndpointPattern": "/api/*",
        "MaxRequests": 1000,
        "TimeWindowSeconds": 60,
        "Description": "Global API limit - 1000 per minute"
      }
    ],
    "ProviderLimits": {
      "SMS": {
        "MaxRequests": 100,
        "TimeWindowSeconds": 60
      },
      "Email": {
        "MaxRequests": 1000,
        "TimeWindowSeconds": 60
      },
      "MMS": {
        "MaxRequests": 50,
        "TimeWindowSeconds": 60
      }
    }
  }
}
```

## Usage

### Creating Rate Limit Rules

```csharp
var createDto = new CreateApiRateLimitDto
{
    UserId = "user-123",  // null for tenant/global rules
    TenantId = null,       // null for user/global rules
    EndpointPattern = "/api/messages/*",
    MaxRequests = 100,
    TimeWindowSeconds = 60,
    IsActive = true,
    Priority = 10
};

var result = await rateLimitService.CreateApiRateLimitAsync(createDto);
```

### Checking Rate Limits in Code

```csharp
var status = await rateLimitService.CheckApiRateLimitAsync(
    userId, 
    tenantId, 
    endpoint, 
    httpMethod
);

if (status.IsLimited)
{
    // Handle rate limit exceeded
    return new HttpStatusCodeResult(429);
}

// Process request
await rateLimitService.RecordApiRequestAsync(userId, tenantId, endpoint, httpMethod);
```

### Provider Rate Limiting

```csharp
// Check provider limit before sending
if (!await rateLimitService.CheckProviderRateLimitAsync("Twilio", "SMS", userId))
{
    // Provider limit exceeded
    throw new InvalidOperationException("SMS provider rate limit exceeded");
}

// Send message
await smsProvider.SendAsync(message);

// Record provider request
await rateLimitService.RecordProviderRequestAsync("Twilio", "SMS", userId);
```

## Rate Limit Rule Priority

Rules are evaluated in order of specificity:
1. **User-specific rules** - Highest priority
2. **Tenant-specific rules** - Medium priority
3. **Global rules** - Lowest priority

Within each category, rules with higher `Priority` values are checked first.

## Pattern Matching

Endpoint patterns support wildcard matching:
- `/api/messages/*` - Matches all message endpoints
- `/api/messages/bulk` - Exact match for bulk endpoint
- `/api/*` - Matches all API endpoints

Patterns are converted to regex for matching, so `*` is replaced with `.*`.

## Monitoring and Logging

### Rate Limit Violations

All violations are logged to the `RateLimitLogs` table with:
- User ID and Tenant ID
- Endpoint and HTTP method
- Client IP address
- Rule that was triggered
- Request count vs. limit
- Timestamp and retry-after time

### Viewing Logs

```http
GET /api/ratelimits/logs?userId=user-123&startDate=2026-01-01&endDate=2026-01-31&pageSize=100
Authorization: Bearer {admin-token}
```

Response:
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "userId": "user-123",
      "endpoint": "/api/messages/bulk",
      "httpMethod": "POST",
      "ipAddress": "192.168.1.1",
      "rateLimitRule": "10 requests per 60 seconds",
      "requestCount": 11,
      "maxRequests": 10,
      "timeWindowSeconds": 60,
      "triggeredAt": "2026-01-18T18:30:00Z",
      "retryAfterSeconds": 45
    }
  ]
}
```

## Error Responses

When a rate limit is exceeded, the API returns:

**Status Code:** 429 Too Many Requests

**Headers:**
```
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 0
X-RateLimit-Reset: 1705598400
Retry-After: 45
```

**Body:**
```json
{
  "error": "Rate limit exceeded",
  "message": "You have exceeded the rate limit of 100 requests per 60 seconds for this endpoint.",
  "retryAfter": 45,
  "resetTime": "2026-01-18T18:30:00Z",
  "endpoint": "/api/messages/bulk"
}
```

## Database Schema

### ApiRateLimits Table
```sql
CREATE TABLE ApiRateLimits (
    Id INT PRIMARY KEY IDENTITY,
    UserId NVARCHAR(450) NULL,
    TenantId NVARCHAR(450) NULL,
    EndpointPattern NVARCHAR(500) NOT NULL,
    MaxRequests INT NOT NULL,
    TimeWindowSeconds INT NOT NULL,
    CurrentRequestCount INT NOT NULL DEFAULT 0,
    WindowStartTime DATETIME2 NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    Priority INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);
```

### RateLimitLogs Table
```sql
CREATE TABLE RateLimitLogs (
    Id INT PRIMARY KEY IDENTITY,
    UserId NVARCHAR(450) NOT NULL,
    TenantId NVARCHAR(450) NULL,
    Endpoint NVARCHAR(500) NOT NULL,
    HttpMethod NVARCHAR(10) NOT NULL,
    IpAddress NVARCHAR(45) NOT NULL,
    RateLimitRule NVARCHAR(500) NOT NULL,
    RequestCount INT NOT NULL,
    MaxRequests INT NOT NULL,
    TimeWindowSeconds INT NOT NULL,
    TriggeredAt DATETIME2 NOT NULL,
    RetryAfterSeconds INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);
```

### ProviderRateLimits Table
```sql
CREATE TABLE ProviderRateLimits (
    Id INT PRIMARY KEY IDENTITY,
    ProviderName NVARCHAR(100) NOT NULL,
    ProviderType NVARCHAR(50) NOT NULL,
    MaxRequests INT NOT NULL,
    TimeWindowSeconds INT NOT NULL,
    CurrentRequestCount INT NOT NULL DEFAULT 0,
    WindowStartTime DATETIME2 NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    UserId NVARCHAR(450) NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);
```

## Integration Points

### Message Service Integration

The message service should check provider rate limits before sending:

```csharp
public async Task<bool> SendMessageAsync(Message message)
{
    // Check provider rate limit
    if (!await _rateLimitService.CheckProviderRateLimitAsync(
        providerName, 
        message.MessageType.ToString(), 
        message.UserId))
    {
        _logger.LogWarning("Provider rate limit exceeded for {Provider}", providerName);
        return false;
    }

    // Send message
    var result = await _provider.SendAsync(message);

    // Record provider request
    if (result.Success)
    {
        await _rateLimitService.RecordProviderRequestAsync(
            providerName, 
            message.MessageType.ToString(), 
            message.UserId);
    }

    return result.Success;
}
```

## Security Considerations

1. **Authentication Required**: Rate limiting middleware only applies to authenticated requests
2. **IP Tracking**: Client IP addresses are logged for abuse detection
3. **Admin-Only Management**: Rate limit configuration requires admin permissions
4. **Soft Deletes**: Rate limit rules use soft deletes to maintain audit history

## Performance Considerations

1. **In-Memory Caching**: Consider adding caching for rate limit rules
2. **Database Indexes**: All relevant columns are indexed for performance
3. **Background Cleanup**: Implement periodic cleanup of old rate limit logs
4. **Window Reset**: Automatic window reset reduces database load

## Future Enhancements

1. **Distributed Rate Limiting**: Use Redis for multi-server deployments
2. **Dynamic Rate Limits**: Adjust limits based on user subscription tier
3. **Rate Limit Exemptions**: Whitelist specific users or IPs
4. **Burst Allowance**: Allow short bursts above the limit
5. **Rate Limit Analytics**: Dashboard for visualizing rate limit usage

## Related Documentation

- [Frequency Control Documentation](./SCHEDULING_AUTOMATION_IMPLEMENTATION.md) - Message-level frequency control
- [Messaging Engine Documentation](./MESSAGING_ENGINE_IMPLEMENTATION.md) - Message routing and delivery
- [RBAC Documentation](./RBAC_IMPLEMENTATION.md) - Role-based access control

## Testing

### Manual Testing

1. **Test rate limit enforcement**:
   ```bash
   for i in {1..150}; do
     curl -X POST https://localhost:5001/api/messages/bulk \
       -H "Authorization: Bearer {token}" \
       -H "Content-Type: application/json" \
       -d '{"recipients": ["test@example.com"], "message": "Test"}'
   done
   ```

2. **Check rate limit status**:
   ```bash
   curl https://localhost:5001/api/ratelimits/status?endpoint=/api/messages/bulk \
     -H "Authorization: Bearer {token}"
   ```

3. **View violation logs** (admin):
   ```bash
   curl https://localhost:5001/api/ratelimits/logs \
     -H "Authorization: Bearer {admin-token}"
   ```

## Troubleshooting

### Rate limits not enforced
- Check that `RateLimitingMiddleware` is registered in `Program.cs`
- Verify rate limit rules exist in database
- Check that endpoint patterns match actual endpoints

### Rate limits too strict
- Review and adjust `MaxRequests` and `TimeWindowSeconds`
- Check rule priority - more specific rules override general ones
- Consider increasing limits for specific users/tenants

### Provider rate limits exceeded
- Review provider documentation for actual limits
- Adjust `ProviderRateLimits` configuration
- Implement request queuing for high-volume scenarios

## Summary

The rate limiting implementation provides a robust, configurable system for protecting the Marketing Platform APIs from abuse while maintaining good user experience. It follows the Repository and Service pattern, integrates seamlessly with the existing infrastructure, and provides comprehensive monitoring and administration capabilities.
