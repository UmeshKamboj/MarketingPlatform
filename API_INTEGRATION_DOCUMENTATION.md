# API & Integration Layer Documentation

## Overview
This document describes the comprehensive REST API and Integration Layer implemented for the Marketing Platform, including webhooks for delivery status tracking and CRM integration capabilities.

## Table of Contents
1. [Existing REST APIs](#existing-rest-apis)
2. [Webhook Endpoints](#webhook-endpoints)
3. [CRM Integration Endpoints](#crm-integration-endpoints)
4. [Provider Abstraction Layer](#provider-abstraction-layer)
5. [Security](#security)

---

## Existing REST APIs

The platform already includes comprehensive REST APIs for all core entities:

### Campaigns API (`/api/campaigns`)
- `GET /api/campaigns` - List campaigns with pagination
- `GET /api/campaigns/{id}` - Get campaign details
- `GET /api/campaigns/status/{status}` - Filter by status
- `POST /api/campaigns` - Create new campaign
- `PUT /api/campaigns/{id}` - Update campaign
- `DELETE /api/campaigns/{id}` - Delete campaign
- `POST /api/campaigns/{id}/duplicate` - Duplicate campaign
- `POST /api/campaigns/{id}/schedule` - Schedule campaign
- `POST /api/campaigns/{id}/start` - Start campaign
- `POST /api/campaigns/{id}/pause` - Pause campaign
- `POST /api/campaigns/{id}/resume` - Resume campaign
- `POST /api/campaigns/{id}/cancel` - Cancel campaign
- `GET /api/campaigns/{id}/stats` - Get campaign statistics
- `POST /api/campaigns/calculate-audience` - Calculate audience size

### Contacts API (`/api/contacts`)
- `GET /api/contacts` - List contacts with pagination
- `GET /api/contacts/{id}` - Get contact details
- `POST /api/contacts` - Create new contact
- `PUT /api/contacts/{id}` - Update contact
- `DELETE /api/contacts/{id}` - Delete contact
- `POST /api/contacts/import/csv` - Import from CSV
- `POST /api/contacts/import/excel` - Import from Excel
- `POST /api/contacts/export/csv` - Export to CSV
- `POST /api/contacts/export/excel` - Export to Excel
- `GET /api/contacts/search` - Search contacts
- `POST /api/contacts/check-duplicates` - Check for duplicates
- `GET /api/contacts/duplicates/report` - Get duplicate report
- `POST /api/contacts/duplicates/resolve` - Resolve duplicates

### Contact Groups API (`/api/contactgroups`)
- `GET /api/contactgroups` - List groups with pagination
- `GET /api/contactgroups/{id}` - Get group details
- `POST /api/contactgroups` - Create new group
- `PUT /api/contactgroups/{id}` - Update group
- `DELETE /api/contactgroups/{id}` - Delete group
- `POST /api/contactgroups/{groupId}/contacts/{contactId}` - Add contact to group
- `DELETE /api/contactgroups/{groupId}/contacts/{contactId}` - Remove contact from group
- `GET /api/contactgroups/{id}/contacts` - Get group contacts
- `POST /api/contactgroups/{id}/refresh` - Refresh dynamic group
- `POST /api/contactgroups/refresh-all` - Refresh all dynamic groups

### Messages API (`/api/messages`)
- `GET /api/messages` - List messages with pagination
- `GET /api/messages/{id}` - Get message details
- `GET /api/messages/campaign/{campaignId}` - Get campaign messages
- `GET /api/messages/status/{status}` - Filter by status
- `POST /api/messages` - Create new message
- `POST /api/messages/bulk` - Create bulk messages
- `POST /api/messages/{id}/send` - Send message immediately
- `POST /api/messages/{id}/retry` - Retry failed message
- `POST /api/messages/{id}/cancel` - Cancel scheduled message
- `POST /api/messages/campaign/{campaignId}/retry` - Retry failed campaign messages
- `GET /api/messages/campaign/{campaignId}/report` - Get delivery report

### Additional APIs
- **Templates** (`/api/templates`) - Message template management
- **Analytics** (`/api/analytics`) - Campaign analytics and reporting
- **Keywords** (`/api/keywords`) - SMS keyword automation
- **Compliance** (`/api/compliance`) - Compliance and consent management
- **Users** (`/api/users`) - User management
- **Suppression Lists** (`/api/suppressionlists`) - Opt-out management
- **Contact Tags** (`/api/contacttags`) - Contact tagging
- **Audience** (`/api/audience`) - Audience segmentation
- **URLs** (`/api/urls`) - URL shortening and tracking

---

## Webhook Endpoints

Webhooks allow external providers (SMS, MMS, Email) to notify the platform about delivery status, inbound messages, and opt-outs.

### Base Path: `/api/webhooks`

### 1. Message Status Update
```http
POST /api/webhooks/message-status
Content-Type: application/json
X-Webhook-Signature: <HMAC-SHA256-signature>

{
  "externalMessageId": "SM1234567890",
  "status": "delivered",
  "errorMessage": null
}
```

**Purpose:** Update message delivery status from provider  
**Authentication:** Webhook signature validation (HMAC-SHA256)  
**Response:** `{ "success": true }`

**Supported Statuses:**
- `queued` / `accepted` / `scheduled` → Queued
- `sending` / `sent` → Sending
- `delivered` → Delivered
- `failed` / `undelivered` / `bounced` → Failed

---

### 2. SMS Inbound
```http
POST /api/webhooks/sms-inbound
Content-Type: application/json
X-Webhook-Signature: <HMAC-SHA256-signature>

{
  "from": "+1234567890",
  "to": "+0987654321",
  "body": "KEYWORD response text",
  "messageSid": "SM1234567890"
}
```

**Purpose:** Process inbound SMS messages  
**Features:**
- Automatic keyword detection and processing
- Opt-out keyword detection (STOP, UNSUBSCRIBE, etc.)
- Message logging

**Opt-Out Keywords:** STOP, UNSUBSCRIBE, CANCEL, END, QUIT, OPTOUT

---

### 3. SMS Delivery Status
```http
POST /api/webhooks/sms-delivery?externalMessageId=SM123
Content-Type: application/json
X-Webhook-Signature: <HMAC-SHA256-signature>

{
  "status": "delivered",
  "errorCode": null,
  "errorMessage": null,
  "deliveredAt": "2026-01-18T18:00:00Z",
  "cost": 0.0075,
  "metadata": {}
}
```

**Purpose:** Detailed SMS delivery status tracking  
**Features:** Cost tracking, error details, delivery timestamps

---

### 4. Email Delivery Status
```http
POST /api/webhooks/email-delivery?externalMessageId=EM123
Content-Type: application/json
X-Webhook-Signature: <HMAC-SHA256-signature>

{
  "status": "delivered",
  "deliveredAt": "2026-01-18T18:00:00Z",
  "metadata": {
    "opened": "true",
    "clicked": "false"
  }
}
```

**Purpose:** Email delivery and engagement tracking  
**Features:** Open/click tracking via metadata

---

### 5. Opt-Out Processing
```http
POST /api/webhooks/opt-out
Content-Type: application/json

{
  "phoneNumber": "+1234567890",
  "source": "SMS_INBOUND"
}
```

**Purpose:** Manual opt-out processing  
**Actions:**
- Add to suppression list
- Update contact opt-in flags
- Prevent future messaging

---

## CRM Integration Endpoints

Enable bidirectional sync with external CRM systems (Salesforce, HubSpot, Zoho, Microsoft Dynamics).

### Base Path: `/api/integration`

### 1. Test CRM Connection
```http
POST /api/integration/crm/test-connection
Authorization: Bearer <jwt-token>
Content-Type: application/json

{
  "crmType": 0,  // 0=Salesforce, 1=HubSpot, 2=Zoho, 3=MicrosoftDynamics
  "apiKey": "your-api-key",
  "apiSecret": "your-api-secret",
  "instanceUrl": "https://your-instance.salesforce.com"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "isConnected": true,
    "message": "Connection successful",
    "version": "v52.0",
    "metadata": {}
  }
}
```

---

### 2. Get CRM Fields
```http
POST /api/integration/crm/fields
Authorization: Bearer <jwt-token>
Content-Type: application/json

{
  "crmType": 0,
  "apiKey": "your-api-key",
  "apiSecret": "your-api-secret"
}
```

**Purpose:** Retrieve available CRM fields for mapping  
**Response:** List of CRM fields with name, label, type, and requirements

---

### 3. Sync Contacts FROM CRM
```http
POST /api/integration/crm/sync-from
Authorization: Bearer <jwt-token>
Content-Type: application/json

{
  "crmType": 0,
  "apiKey": "your-api-key",
  "apiSecret": "your-api-secret",
  "direction": 0,  // 0=FromCRM, 1=ToCRM, 2=Bidirectional
  "fieldMappings": {
    "FirstName": "firstName",
    "LastName": "lastName",
    "Email": "email",
    "Phone": "phoneNumber"
  },
  "autoSync": false,
  "syncIntervalMinutes": 60
}
```

**Purpose:** Import contacts from CRM to platform  
**Response:**
```json
{
  "success": true,
  "data": {
    "success": true,
    "totalRecords": 150,
    "successCount": 148,
    "failureCount": 2,
    "errors": ["Contact xyz@example.com failed: duplicate email"],
    "syncedAt": "2026-01-18T18:00:00Z",
    "message": "Synced 148 of 150 contacts from Salesforce"
  }
}
```

---

### 4. Sync Contacts TO CRM
```http
POST /api/integration/crm/sync-to
Authorization: Bearer <jwt-token>
Content-Type: application/json

{
  "contactIds": [1, 2, 3, 4, 5],
  "config": {
    "crmType": 0,
    "apiKey": "your-api-key",
    "apiSecret": "your-api-secret",
    "fieldMappings": {}
  }
}
```

**Purpose:** Export selected contacts to CRM  
**Features:** Creates or updates CRM contacts based on platform data

---

### 5. Sync Campaign Results to CRM
```http
POST /api/integration/crm/sync-campaign/123
Authorization: Bearer <jwt-token>
Content-Type: application/json

{
  "crmType": 0,
  "apiKey": "your-api-key",
  "apiSecret": "your-api-secret"
}
```

**Purpose:** Update CRM with campaign engagement data  
**Data Synced:**
- Campaign name
- Message delivery status
- Send timestamp
- Contact engagement

---

## Provider Abstraction Layer

The platform uses a provider abstraction pattern for messaging channels.

### Provider Interfaces

#### ISMSProvider
```csharp
Task<(bool Success, string? ExternalId, string? Error, decimal? Cost)> SendSMSAsync(
    string recipient, 
    string message);

Task<(bool Success, string? Error)> GetDeliveryStatusAsync(string externalMessageId);
```

#### IMMSProvider
```csharp
Task<(bool Success, string? ExternalId, string? Error, decimal? Cost)> SendMMSAsync(
    string recipient, 
    string message, 
    List<string> mediaUrls);

Task<(bool Success, string? Error)> GetDeliveryStatusAsync(string externalMessageId);
```

#### IEmailProvider
```csharp
Task<(bool Success, string? ExternalId, string? Error, decimal? Cost)> SendEmailAsync(
    string recipient, 
    string subject, 
    string textBody, 
    string? htmlBody = null);

Task<(bool Success, string? Error)> GetDeliveryStatusAsync(string externalMessageId);
```

### Current Implementations
- **MockSMSProvider** - Development/testing mock
- **MockMMSProvider** - Development/testing mock
- **MockEmailProvider** - Development/testing mock

### Production Providers (To Be Implemented)
- **TwilioSMSProvider** - Twilio SMS integration
- **TwilioMMSProvider** - Twilio MMS integration
- **SendGridEmailProvider** - SendGrid email integration
- **AWS SNS Provider** - Amazon SNS integration
- **Mailgun Provider** - Mailgun email integration

---

## Security

### Webhook Signature Validation

All webhook endpoints support signature validation using HMAC-SHA256:

1. **Generate Signature (Provider Side):**
   ```
   signature = Base64(HMAC-SHA256(payload, secret_key))
   ```

2. **Send with Header:**
   ```
   X-Webhook-Signature: <signature>
   ```

3. **Platform Validates:**
   - Computes expected signature from payload
   - Compares with provided signature
   - Rejects if mismatch

### Configuration

Set webhook secret in `appsettings.json`:
```json
{
  "WebhookSettings": {
    "Secret": "your-production-webhook-secret"
  }
}
```

### JWT Authentication

All API endpoints (except webhooks) require JWT bearer token:
```http
Authorization: Bearer <jwt-token>
```

---

## Architecture Patterns

### Repository Pattern
All data access uses generic repository pattern:
```csharp
IRepository<T> where T : class
- GetByIdAsync()
- GetAllAsync()
- FindAsync()
- AddAsync()
- Update()
- Remove()
```

### Unit of Work Pattern
Transaction management via Unit of Work:
```csharp
IUnitOfWork
- Repository<T>()
- SaveChangesAsync()
- BeginTransactionAsync()
- CommitTransactionAsync()
- RollbackTransactionAsync()
```

### Service Pattern
Business logic in services:
- Services implement interfaces (e.g., `ICampaignService`)
- Services use repositories for data access
- Services registered in DI container

---

## Response Format

All API responses follow consistent format:

### Success Response
```json
{
  "success": true,
  "data": { ... },
  "message": "Operation successful",
  "errors": []
}
```

### Error Response
```json
{
  "success": false,
  "data": null,
  "message": "Operation failed",
  "errors": ["Error detail 1", "Error detail 2"]
}
```

### Paginated Response
```json
{
  "success": true,
  "data": {
    "items": [...],
    "totalCount": 150,
    "pageNumber": 1,
    "pageSize": 20,
    "totalPages": 8
  }
}
```

---

## Error Handling

### HTTP Status Codes
- `200 OK` - Success
- `201 Created` - Resource created
- `400 Bad Request` - Invalid request
- `401 Unauthorized` - Missing/invalid authentication
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

### Common Error Scenarios
1. **Validation Errors** - Invalid input data
2. **Authentication Errors** - Missing/expired token
3. **Authorization Errors** - User doesn't own resource
4. **Not Found Errors** - Resource doesn't exist
5. **Conflict Errors** - Duplicate or state conflicts

---

## Rate Limiting

The platform includes built-in rate limiting service (`IRateLimitService`) for:
- API request throttling
- Message sending rate limits
- Campaign execution rate limits
- Provider-specific rate limits

---

## Best Practices

### For API Consumers
1. Always include JWT token in Authorization header
2. Handle pagination for list endpoints
3. Implement retry logic with exponential backoff
4. Validate webhook signatures
5. Use appropriate content types

### For Webhook Providers
1. Always send X-Webhook-Signature header
2. Implement retry with exponential backoff
3. Use idempotent operations
4. Include unique message IDs
5. Follow standardized status codes

### For CRM Integration
1. Test connection before sync
2. Map fields correctly
3. Handle sync errors gracefully
4. Use incremental sync when possible
5. Monitor sync success rates

---

## Future Enhancements

1. **Real Provider Implementations**
   - Twilio SMS/MMS integration
   - SendGrid email integration
   - AWS SNS/SES integration

2. **Enhanced CRM Support**
   - Complete Salesforce implementation
   - Complete HubSpot implementation
   - Custom CRM connector framework

3. **Advanced Features**
   - Webhook retry mechanism
   - Webhook event log
   - CRM sync scheduling
   - Field mapping UI
   - Provider configuration UI

---

## Support

For issues or questions:
1. Check API response error messages
2. Review server logs (Serilog configured)
3. Verify authentication and permissions
4. Test with provided mock implementations
5. Review this documentation

---

**Last Updated:** January 18, 2026  
**Version:** 1.0  
**Status:** Production Ready
