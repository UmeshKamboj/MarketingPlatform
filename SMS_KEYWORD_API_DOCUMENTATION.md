# SMS Keyword Management System - API Documentation

## Overview
The SMS Keyword Management System allows users to create and manage SMS keywords that can trigger automated responses and opt-in contacts to specific groups. This feature is fully integrated with the existing campaign and contact management systems.

## Features Implemented

### 1. Keyword CRUD Operations
- Create new keywords with custom response messages
- Update existing keywords (text, status, response message, linked campaign, opt-in group)
- Delete keywords (soft delete)
- List all keywords with pagination and search
- Get keyword by ID
- Filter keywords by status (Active, Inactive, Reserved)
- Check keyword availability

### 2. Keyword Campaign Integration
- Link keywords to existing campaigns
- Automatically associate inbound keyword responses with campaigns
- Track campaign performance through keyword activities

### 3. Keyword Activity Tracking
- Log all inbound SMS messages matching keywords
- Track phone numbers, messages, and responses sent
- View activity history per keyword with pagination
- Activity count per keyword

### 4. Inbound SMS Processing
- Webhook endpoint for SMS providers (Twilio format)
- Automatic keyword detection and matching
- Auto-response sending based on keyword configuration
- Automatic contact opt-in to groups when keyword is used

### 5. Keyword Analytics & Engagement Metrics
- Comprehensive analytics endpoint for keyword performance
- Opt-in conversion tracking (successful vs failed opt-ins)
- Response success rate monitoring
- Unique contact engagement tracking
- Repeat usage analysis
- Time-based activity metrics (24h, 7d, 30d)
- Campaign-related activity analytics

## API Endpoints

### Keywords Management

#### GET /api/keywords
Get paginated list of keywords for the authenticated user.

**Query Parameters:**
- `pageNumber` (int, default: 1)
- `pageSize` (int, default: 10)
- `searchTerm` (string, optional)
- `sortBy` (string, optional: "keywordtext", "status", "createdat")
- `sortDescending` (bool, default: false)

**Response:**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": 1,
        "keywordText": "JOIN",
        "description": "Join our mailing list",
        "isGloballyReserved": false,
        "status": "Active",
        "responseMessage": "Thanks for joining! You'll receive updates from us.",
        "linkedCampaignId": 5,
        "linkedCampaignName": "Newsletter Campaign",
        "optInGroupId": 3,
        "optInGroupName": "Newsletter Subscribers",
        "activityCount": 142,
        "createdAt": "2026-01-15T10:00:00Z",
        "updatedAt": "2026-01-15T10:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 5,
    "totalPages": 1
  }
}
```

#### GET /api/keywords/{id}
Get a specific keyword by ID.

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "keywordText": "JOIN",
    "description": "Join our mailing list",
    "isGloballyReserved": false,
    "status": "Active",
    "responseMessage": "Thanks for joining! You'll receive updates from us.",
    "linkedCampaignId": 5,
    "linkedCampaignName": "Newsletter Campaign",
    "optInGroupId": 3,
    "optInGroupName": "Newsletter Subscribers",
    "activityCount": 142,
    "createdAt": "2026-01-15T10:00:00Z",
    "updatedAt": "2026-01-15T10:00:00Z"
  }
}
```

#### GET /api/keywords/status/{status}
Get keywords by status (Active, Inactive, Reserved).

**Response:** Same as GET /api/keywords but filtered by status.

#### GET /api/keywords/check-availability?keywordText=JOIN
Check if a keyword is available for use.

**Response:**
```json
{
  "success": true,
  "data": true
}
```

#### POST /api/keywords
Create a new keyword.

**Request Body:**
```json
{
  "keywordText": "JOIN",
  "description": "Join our mailing list",
  "responseMessage": "Thanks for joining! You'll receive updates from us.",
  "linkedCampaignId": 5,
  "optInGroupId": 3
}
```

**Validation Rules:**
- `keywordText`: Required, max 50 characters, alphanumeric only (no spaces/special chars)
- `description`: Optional, max 500 characters
- `responseMessage`: Optional, max 1000 characters
- Keyword text is automatically converted to uppercase
- Keyword must be unique per user (unless globally reserved)

**Response:** Returns created keyword with ID.

#### PUT /api/keywords/{id}
Update an existing keyword.

**Request Body:**
```json
{
  "keywordText": "JOIN",
  "description": "Updated description",
  "status": "Active",
  "responseMessage": "Updated response message",
  "linkedCampaignId": 5,
  "optInGroupId": 3
}
```

**Response:**
```json
{
  "success": true,
  "data": true,
  "message": "Keyword updated successfully"
}
```

**Note:** Cannot update globally reserved keywords.

#### DELETE /api/keywords/{id}
Soft delete a keyword.

**Response:**
```json
{
  "success": true,
  "data": true,
  "message": "Keyword deleted successfully"
}
```

**Note:** Cannot delete globally reserved keywords.

### Keyword Activities

#### GET /api/keywords/{id}/activities
Get activity history for a specific keyword.

**Query Parameters:**
- `pageNumber` (int, default: 1)
- `pageSize` (int, default: 10)

**Response:**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": 1,
        "keywordId": 1,
        "keywordText": "JOIN",
        "phoneNumber": "+1234567890",
        "incomingMessage": "JOIN",
        "responseSent": "Thanks for joining! You'll receive updates from us.",
        "receivedAt": "2026-01-18T14:30:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 142,
    "totalPages": 15
  }
}
```

### Keyword Analytics

#### GET /api/keywords/{id}/analytics
Get comprehensive analytics and engagement metrics for a specific keyword.

**Response:**
```json
{
  "success": true,
  "data": {
    "keywordId": 1,
    "keywordText": "JOIN",
    "totalResponses": 142,
    "uniqueContacts": 95,
    "repeatUsageCount": 47,
    "totalOptIns": 95,
    "successfulOptIns": 92,
    "failedOptIns": 3,
    "optInConversionRate": 64.79,
    "responsesSent": 140,
    "responsesFailed": 2,
    "responseSuccessRate": 98.59,
    "linkedCampaignId": 5,
    "linkedCampaignName": "Newsletter Campaign",
    "campaignRelatedActivities": 142,
    "firstUsedAt": "2026-01-10T08:15:00Z",
    "lastUsedAt": "2026-01-18T14:30:00Z",
    "activitiesLast24Hours": 12,
    "activitiesLast7Days": 78,
    "activitiesLast30Days": 142
  }
}
```

**Analytics Metrics Explained:**
- **totalResponses**: Total number of times the keyword was used
- **uniqueContacts**: Number of unique phone numbers that used the keyword
- **repeatUsageCount**: Number of times the keyword was used by returning contacts (totalResponses - uniqueContacts)
- **totalOptIns**: Number of contacts that exist in the system (from those who used the keyword)
- **successfulOptIns**: Number of contacts successfully added to the opt-in group
- **failedOptIns**: Number of contacts that could not be added to the opt-in group
- **optInConversionRate**: Percentage of keyword uses that resulted in successful opt-ins
- **responsesSent**: Number of auto-responses successfully sent
- **responsesFailed**: Number of auto-responses that failed to send
- **responseSuccessRate**: Percentage of successful auto-response deliveries
- **campaignRelatedActivities**: Number of activities linked to the campaign (if linkedCampaignId is set)
- **activitiesLast24Hours/7Days/30Days**: Time-based activity counts

### Inbound SMS Processing

#### POST /api/keywords/process-inbound
Process an inbound SMS message (for direct integration).

**Request Body:**
```json
{
  "phoneNumber": "+1234567890",
  "message": "JOIN"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "keywordId": 1,
    "keywordText": "JOIN",
    "phoneNumber": "+1234567890",
    "incomingMessage": "JOIN",
    "responseSent": "Thanks for joining! You'll receive updates from us.",
    "receivedAt": "2026-01-18T14:30:00Z"
  },
  "message": "Inbound keyword processed successfully"
}
```

**Note:** This endpoint is `[AllowAnonymous]` for webhook access.

#### POST /api/webhooks/sms-inbound
Process inbound SMS from SMS provider webhook (Twilio format).

**Request Body:**
```json
{
  "from": "+1234567890",
  "to": "+0987654321",
  "body": "JOIN",
  "messageSid": "SM1234567890abcdef"
}
```

**Response:**
```json
{
  "success": true
}
```

**Note:** This endpoint is for SMS provider webhooks. Automatically processes keywords and sends responses.

## Business Logic

### Keyword Processing Flow
1. Inbound SMS received via webhook
2. Extract first word as keyword (case-insensitive)
3. Look up keyword in database (must be Active status)
4. If found:
   - Log activity in KeywordActivities table
   - Send auto-response if configured
   - Add contact to opt-in group if configured
   - Link to campaign if configured
5. If not found:
   - Log as unknown keyword activity (keywordId = 0)

### Keyword Text Normalization
- All keywords are stored and matched in UPPERCASE
- Whitespace is trimmed
- Only alphanumeric characters allowed
- Maximum length: 50 characters

### Keyword Availability Rules
- Keywords must be unique per user
- Globally reserved keywords cannot be created by users
- Deleted keywords free up the keyword text for reuse

### Auto Opt-In Logic
When a contact sends a keyword with an `optInGroupId`:
1. System looks up contact by phone number
2. If contact exists and not already in group, adds them
3. If contact doesn't exist, logs warning (contact must be created first)

## Database Schema

### Keywords Table
- `Id`: Primary key
- `UserId`: Foreign key to user
- `KeywordText`: Unique keyword (max 50 chars, uppercase)
- `Description`: Optional description (max 500 chars)
- `IsGloballyReserved`: System-reserved keyword flag
- `Status`: Active, Inactive, or Reserved
- `ResponseMessage`: Auto-response text (max 1000 chars)
- `LinkedCampaignId`: Optional foreign key to Campaign
- `OptInGroupId`: Optional foreign key to ContactGroup
- `CreatedAt`, `UpdatedAt`, `IsDeleted`: Audit fields

### KeywordActivities Table
- `Id`: Primary key
- `KeywordId`: Foreign key to Keyword
- `PhoneNumber`: Sender phone number
- `IncomingMessage`: Full incoming SMS text
- `ResponseSent`: Response message sent (if any)
- `ReceivedAt`: Timestamp of received message

## Integration Points

### Campaign Integration
- Keywords can be linked to campaigns via `LinkedCampaignId`
- Keyword activities count toward campaign analytics
- Campaign performance can be tracked through keyword usage

### Contact Group Integration
- Keywords can auto-opt-in contacts to groups via `OptInGroupId`
- Existing contacts are added to the group automatically
- Group membership is tracked in ContactGroupMembers table

### SMS Provider Integration
- Works with ISMSProvider interface
- Currently uses MockSMSProvider for testing
- Ready for Twilio, Plivo, or other provider integration
- Webhook endpoints support Twilio format

## Security

### Authentication
- All keyword management endpoints require JWT authentication
- Users can only access their own keywords
- Webhook endpoints use `[AllowAnonymous]` for provider access

### Validation
- FluentValidation rules for all DTOs
- Keyword uniqueness validation
- Campaign and group ownership validation
- Globally reserved keyword protection

### Soft Delete
- Keywords are soft-deleted (IsDeleted flag)
- Maintains data integrity and audit trail
- Deleted keywords can be recreated with same text

## Token Encryption

The system uses JWT tokens for authentication, managed by the TokenService:
- Tokens are signed with HS256 algorithm
- Secret key stored in configuration (JwtSettings:Secret)
- Tokens include user ID, email, roles in claims
- Refresh tokens supported for long-lived sessions

## Future Enhancements

1. **Bulk Keyword Import**: CSV import for multiple keywords
2. **Keyword Analytics Dashboard**: Usage statistics and trends
3. **Multi-language Keywords**: Support for non-English keywords
4. **Keyword Aliases**: Multiple keywords triggering same action
5. **Advanced Auto-Responses**: Template variables, conditional logic
6. **Stop/Unsubscribe Keywords**: Automatic opt-out handling
7. **Webhook Signature Validation**: Enhanced security for provider webhooks
8. **Rate Limiting**: Prevent keyword spam/abuse
