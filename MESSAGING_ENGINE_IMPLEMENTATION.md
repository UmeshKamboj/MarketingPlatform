# Messaging Engine: Delivery & Routing Implementation

## Overview

This document describes the implementation of the robust messaging engine for the Marketing Platform, including message queuing, channel routing logic (SMS, MMS, Email), retry & failure handling, fallback messaging on channel failure, and detailed status tracking.

## Features Implemented

### 1. Message Queuing ✅
- **Background Processing**: `MessageQueueProcessor` service runs continuously
- **Batch Processing**: Processes up to 50 messages per batch to prevent database locks
- **Scheduled Messages**: Supports scheduled message delivery
- **Status Management**: Messages transition through states: Queued → Sending → Sent/Failed

### 2. Channel Routing Logic ✅
- **Multi-Channel Support**: SMS, MMS, and Email channels
- **Routing Service**: `MessageRoutingService` handles intelligent routing
- **Provider Abstraction**: Interface-based provider implementation for flexibility
- **Configuration-Driven**: `ChannelRoutingConfig` entity for runtime configuration

### 3. Retry & Failure Handling ✅
- **Retry Strategies**:
  - None: No automatic retry
  - Linear: Fixed delay between retries
  - Exponential: Exponential backoff (2^n)
  - Custom: Extensible for custom logic
- **Configurable Limits**: `MaxRetries` and delay thresholds per channel
- **Automatic Rescheduling**: Failed messages are automatically queued for retry

### 4. Fallback Messaging ✅
- **Provider Fallback**: Automatic failover to backup provider on primary failure
- **Fallback Reasons Tracked**:
  - Primary Failed
  - Provider Unavailable
  - Rate Limit Exceeded
  - Cost Threshold
  - Quality Threshold
- **Configurable**: Enable/disable fallback per channel

### 5. Detailed Status Tracking ✅
- **Message Statuses**:
  - Queued: Waiting to be sent
  - Sending: Currently being sent
  - Sent: Successfully sent to provider
  - Delivered: Confirmed delivery by provider
  - Failed: Send attempt failed
  - Bounced: Message bounced/rejected
- **Delivery Attempts**: Full audit trail of every send attempt
- **Performance Metrics**: Response time, cost, and error tracking

## Architecture

### Core Components

#### 1. Entities

**MessageDeliveryAttempt**
```csharp
- CampaignMessageId: Reference to message
- AttemptNumber: Sequential attempt number
- Channel: Channel used (SMS/MMS/Email)
- ProviderName: Provider used for this attempt
- AttemptedAt: Timestamp of attempt
- Success: Whether attempt succeeded
- ExternalMessageId: Provider's message ID
- ErrorMessage: Error if failed
- ErrorCode: Categorized error code
- CostAmount: Cost incurred
- ResponseTimeMs: Provider response time
- FallbackReason: Why fallback was used (if applicable)
- AdditionalMetadata: JSON for extra info
```

**ChannelRoutingConfig**
```csharp
- Channel: Channel type (SMS/MMS/Email)
- PrimaryProvider: Main provider name
- FallbackProvider: Backup provider name
- RoutingStrategy: Routing strategy enum
- EnableFallback: Toggle fallback
- MaxRetries: Maximum retry attempts
- RetryStrategy: Retry strategy enum
- InitialRetryDelaySeconds: Base delay
- MaxRetryDelaySeconds: Maximum delay cap
- CostThreshold: Optional cost limit
- IsActive: Configuration status
- Priority: Configuration priority
- AdditionalSettings: JSON for custom config
```

#### 2. Services

**IMessageRoutingService**
```csharp
- RouteMessageAsync(): Main routing logic
- ShouldRetryMessageAsync(): Retry decision logic
- CalculateRetryDelayAsync(): Delay calculation
- TryFallbackChannelAsync(): Fallback handling
- LogDeliveryAttemptAsync(): Audit logging
```

**MessageService** (Enhanced)
- Integrated with routing service
- Automatic retry scheduling
- Enhanced error handling

#### 3. Enums

**RoutingStrategy**
- Primary: Use primary provider
- Fallback: Use fallback provider
- RoundRobin: Distribute across providers
- LeastCost: Choose cheapest provider
- HighestReliability: Choose most reliable

**RetryStrategy**
- None: No retries
- Linear: Fixed intervals
- Exponential: Growing intervals
- Custom: User-defined logic

**FallbackReason**
- PrimaryFailed: Primary provider failed
- ProviderUnavailable: Provider timeout/unavailable
- RateLimitExceeded: Rate limit hit
- CostThreshold: Cost limit exceeded
- QualityThreshold: Quality metric below threshold

## API Endpoints

### Routing Configuration Management

#### Get All Routing Configurations
```http
GET /api/routingconfig
Authorization: Bearer {admin_token}
```

#### Get Configuration by Channel
```http
GET /api/routingconfig/channel/{channel}
Authorization: Bearer {admin_token}
```

Channel values: `0` (SMS), `1` (MMS), `2` (Email)

#### Create Routing Configuration
```http
POST /api/routingconfig
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "channel": 0,
  "primaryProvider": "TwilioSMS",
  "fallbackProvider": "VonageSMS",
  "routingStrategy": 0,
  "enableFallback": true,
  "maxRetries": 3,
  "retryStrategy": 2,
  "initialRetryDelaySeconds": 60,
  "maxRetryDelaySeconds": 3600,
  "isActive": true,
  "priority": 1
}
```

#### Update Routing Configuration
```http
PUT /api/routingconfig/{id}
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "primaryProvider": "UpdatedProvider",
  "enableFallback": false,
  "maxRetries": 5
}
```

#### Delete Routing Configuration
```http
DELETE /api/routingconfig/{id}
Authorization: Bearer {admin_token}
```

### Delivery Tracking

#### Get Delivery Attempts for a Message
```http
GET /api/routingconfig/delivery-attempts/{messageId}
Authorization: Bearer {token}
```

Response:
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "campaignMessageId": 123,
      "attemptNumber": 1,
      "channel": 0,
      "providerName": "MockSMSProvider",
      "attemptedAt": "2026-01-18T12:00:00Z",
      "success": false,
      "errorMessage": "Provider timeout",
      "responseTimeMs": 5000,
      "fallbackReason": null
    },
    {
      "id": 2,
      "campaignMessageId": 123,
      "attemptNumber": 2,
      "channel": 0,
      "providerName": "BackupSMSProvider",
      "attemptedAt": "2026-01-18T12:01:00Z",
      "success": true,
      "externalMessageId": "SMS_abc123",
      "costAmount": 0.0075,
      "responseTimeMs": 250,
      "fallbackReason": 1
    }
  ]
}
```

#### Get Channel Statistics
```http
GET /api/routingconfig/stats/channel/{channel}?startDate=2026-01-01&endDate=2026-01-31
Authorization: Bearer {admin_token}
```

Response:
```json
{
  "success": true,
  "data": {
    "channel": "SMS",
    "totalAttempts": 1500,
    "successfulAttempts": 1425,
    "failedAttempts": 75,
    "successRate": 95.0,
    "averageResponseTimeMs": 320,
    "totalCost": 11.25,
    "fallbackCount": 45,
    "period": {
      "startDate": "2026-01-01T00:00:00Z",
      "endDate": "2026-01-31T23:59:59Z"
    }
  }
}
```

#### Get Overall Statistics
```http
GET /api/routingconfig/stats/overall?startDate=2026-01-01&endDate=2026-01-31
Authorization: Bearer {admin_token}
```

## Configuration Examples

### SMS with Aggressive Retry
```json
{
  "channel": 0,
  "primaryProvider": "TwilioSMS",
  "fallbackProvider": "VonageSMS",
  "enableFallback": true,
  "maxRetries": 5,
  "retryStrategy": 2,
  "initialRetryDelaySeconds": 30,
  "maxRetryDelaySeconds": 1800,
  "isActive": true,
  "priority": 1
}
```

### Email with Conservative Retry
```json
{
  "channel": 2,
  "primaryProvider": "SendGrid",
  "fallbackProvider": "Mailgun",
  "enableFallback": true,
  "maxRetries": 3,
  "retryStrategy": 2,
  "initialRetryDelaySeconds": 300,
  "maxRetryDelaySeconds": 7200,
  "isActive": true,
  "priority": 1
}
```

### MMS without Fallback
```json
{
  "channel": 1,
  "primaryProvider": "TwilioMMS",
  "enableFallback": false,
  "maxRetries": 2,
  "retryStrategy": 1,
  "initialRetryDelaySeconds": 60,
  "maxRetryDelaySeconds": 600,
  "isActive": true,
  "priority": 1
}
```

## Retry Strategy Examples

### Exponential Backoff (Default)
- Attempt 1: 60 seconds
- Attempt 2: 120 seconds (60 * 2^1)
- Attempt 3: 240 seconds (60 * 2^2)
- Attempt 4: 480 seconds (60 * 2^3)
- Capped at MaxRetryDelaySeconds

### Linear Backoff
- Attempt 1: 60 seconds
- Attempt 2: 120 seconds (60 * 2)
- Attempt 3: 180 seconds (60 * 3)
- Attempt 4: 240 seconds (60 * 4)

## Database Schema

### MessageDeliveryAttempts Table
```sql
CREATE TABLE MessageDeliveryAttempts (
    Id INT PRIMARY KEY IDENTITY,
    CampaignMessageId INT NOT NULL,
    AttemptNumber INT NOT NULL,
    Channel INT NOT NULL,
    ProviderName NVARCHAR(100),
    AttemptedAt DATETIME2 NOT NULL,
    Success BIT NOT NULL,
    ExternalMessageId NVARCHAR(200),
    ErrorMessage NVARCHAR(2000),
    ErrorCode NVARCHAR(100),
    CostAmount DECIMAL(18,6),
    ResponseTimeMs INT NOT NULL,
    FallbackReason INT,
    AdditionalMetadata NVARCHAR(4000),
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2,
    FOREIGN KEY (CampaignMessageId) REFERENCES CampaignMessages(Id)
)
```

### ChannelRoutingConfigs Table
```sql
CREATE TABLE ChannelRoutingConfigs (
    Id INT PRIMARY KEY IDENTITY,
    Channel INT NOT NULL,
    PrimaryProvider NVARCHAR(100) NOT NULL,
    FallbackProvider NVARCHAR(100),
    RoutingStrategy INT NOT NULL,
    EnableFallback BIT NOT NULL,
    MaxRetries INT NOT NULL,
    RetryStrategy INT NOT NULL,
    InitialRetryDelaySeconds INT NOT NULL,
    MaxRetryDelaySeconds INT NOT NULL,
    CostThreshold DECIMAL(18,6),
    IsActive BIT NOT NULL,
    Priority INT NOT NULL,
    AdditionalSettings NVARCHAR(4000),
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2
)
```

## Logging and Monitoring

### Log Levels
- **Information**: Successful routing and delivery
- **Warning**: Failed attempts, fallback usage
- **Error**: Exceptions during routing

### Sample Log Messages
```
[INFO] Routing message 123 (Attempt 1) via SMS
[INFO] Message 123 successfully routed via SMS (Provider: TwilioSMS, ExternalId: SM123, Time: 250ms)
[WARN] Message 123 routing failed via SMS (Provider: TwilioSMS, Error: Timeout, Time: 5000ms)
[INFO] Attempting fallback for message 123. Primary error: Timeout
[INFO] Message 123 successfully sent via fallback provider BackupSMSProvider
[INFO] Message 123 will be retried (Attempt 2/3) after 120s
```

## Performance Considerations

### Batch Processing
- Messages processed in batches of 50
- Prevents database lock contention
- Optimizes throughput

### Transactional Isolation
- Status updates in separate transactions
- Minimizes lock duration
- Prevents blocking during external API calls

### Asynchronous Operations
- Provider calls are non-blocking
- Queue processor runs in background
- Scalable to high message volumes

## Future Enhancements

### Planned Features
- [ ] Circuit breaker pattern for provider health
- [ ] Dynamic provider selection based on real-time metrics
- [ ] A/B testing for routing strategies
- [ ] Machine learning-based optimal retry timing
- [ ] Webhook callbacks for delivery status updates
- [ ] Real-time dashboard for monitoring
- [ ] Provider cost comparison and optimization
- [ ] Geographic routing based on recipient location

## Troubleshooting

### High Failure Rates
1. Check provider status and credentials
2. Review error messages in MessageDeliveryAttempts
3. Verify routing configuration is active
4. Check for rate limiting issues

### Slow Delivery
1. Review ResponseTimeMs metrics
2. Check database performance
3. Verify batch size isn't too large
4. Monitor background service health

### Messages Stuck in Queue
1. Verify MessageQueueProcessor is running
2. Check for database connectivity issues
3. Review scheduled times on messages
4. Ensure status is correctly set to Queued

## Security Considerations

- Admin-only access to routing configuration
- Audit trail of all delivery attempts
- Secure provider credentials (not in routing config)
- Rate limiting to prevent abuse

## Conclusion

This messaging engine implementation provides a robust, scalable, and flexible foundation for message delivery across multiple channels with comprehensive tracking, intelligent routing, and automatic failure recovery.
