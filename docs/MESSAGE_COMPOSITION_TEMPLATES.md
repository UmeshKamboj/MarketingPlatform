# Message Composition & Templates - Implementation Guide

## Overview

This document provides comprehensive information about the Message Composition & Templates feature (Task 4.1) implemented in the Marketing Platform. The feature includes SMS, MMS, and Email editors with character count, personalization tokens, template management, and URL tracking.

## Features

### 1. Template Management

#### Core Capabilities
- **Multi-Channel Support**: Create templates for SMS, MMS, and Email
- **Template Categories**: Promotional, Transactional, Reminder, Alert, Custom
- **Template Lifecycle**: Create, Update, Delete, Duplicate, Activate/Deactivate
- **Default Templates**: Set default templates per channel and category
- **Usage Tracking**: Track template usage across campaigns and messages

#### Template Fields
- **Name**: Unique identifier for the template
- **Description**: Optional description
- **Channel**: SMS, MMS, or Email (ChannelType enum: 0=SMS, 1=MMS, 2=Email)
- **Category**: Template category (TemplateCategory enum: 0=Promotional, 1=Transactional, 2=Reminder, 3=Alert, 4=Custom)
- **Subject**: Email subject line (Email only)
- **MessageBody**: Plain text message content (all channels)
- **HTMLContent**: HTML email content (Email only)
- **DefaultMediaUrls**: Array of media URLs (MMS only)
- **Variables**: Array of personalization tokens with metadata

### 2. Personalization Tokens

#### Token Syntax
Templates support variable substitution using double curly braces: `{{VariableName}}`

#### Supported Variables
- **Contact Fields**: {{FirstName}}, {{LastName}}, {{Email}}, {{Phone}}, {{PhoneNumber}}
- **Custom Variables**: Any user-defined variable (e.g., {{OrderNumber}}, {{CompanyName}})
- **Case Insensitive**: Variable names are matched case-insensitively

#### Auto-Extraction
Variables are automatically extracted from template content when creating or updating templates.

### 3. Character Count

#### SMS/MMS Segmentation
- **GSM-7 Encoding**: 160 characters per segment (single), 153 per segment (concatenated)
- **Unicode Encoding**: 70 characters per segment (single), 67 per segment (concatenated)
- **Automatic Detection**: System automatically detects Unicode characters

#### Email Guidelines
- **Subject Line**: 60 characters recommended maximum
- **Body**: No strict limit

#### Character Count Information
- Total character count
- Number of SMS segments (for SMS/MMS)
- Unicode detection flag
- Recommended maximum length
- Exceeds recommended length warning

### 4. URL Tracking

#### URL Shortening
- **Auto-Generated Codes**: 6-character alphanumeric codes using cryptographically secure random generation
- **Custom Codes**: 4-12 alphanumeric characters (user-defined)
- **Short URL Format**: `{BaseUrl}/{ShortCode}` (e.g., https://short.link/abc123)

#### Click Tracking
- **IP Address**: Track visitor IP
- **User Agent**: Track browser/device information
- **Referrer**: Track source of click
- **Timestamp**: UTC timestamp for each click

#### Analytics
- **URL Level**: Total clicks, unique clicks, clicks by date, top referrers
- **Campaign Level**: Total URLs, total clicks, unique clicks, top performing URLs

## API Endpoints

### Template Management

#### Create Template
```bash
POST /api/templates
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Welcome SMS",
  "description": "Welcome message for new customers",
  "channel": 0,
  "category": 0,
  "messageBody": "Hi {{FirstName}}! Welcome to {{CompanyName}}. Reply STOP to unsubscribe.",
  "variables": [
    {
      "name": "FirstName",
      "defaultValue": "Customer",
      "isRequired": true,
      "description": "Customer's first name"
    },
    {
      "name": "CompanyName",
      "defaultValue": "Our Company",
      "isRequired": false,
      "description": "Your company name"
    }
  ]
}
```

#### List Templates
```bash
GET /api/templates?pageNumber=1&pageSize=20&searchTerm=welcome
Authorization: Bearer {token}
```

#### Get Template by ID
```bash
GET /api/templates/{id}
Authorization: Bearer {token}
```

#### Get Templates by Channel
```bash
GET /api/templates/channel/0
Authorization: Bearer {token}
```
Channel values: 0=SMS, 1=MMS, 2=Email

#### Get Templates by Category
```bash
GET /api/templates/category/0
Authorization: Bearer {token}
```
Category values: 0=Promotional, 1=Transactional, 2=Reminder, 3=Alert, 4=Custom

#### Update Template
```bash
PUT /api/templates/{id}
Authorization: Bearer {token}

{
  "name": "Updated Welcome SMS",
  "description": "Updated description",
  "messageBody": "Hello {{FirstName}}! Thanks for joining {{CompanyName}}!"
}
```

#### Delete Template
```bash
DELETE /api/templates/{id}
Authorization: Bearer {token}
```
Note: Cannot delete templates with UsageCount > 0

#### Duplicate Template
```bash
POST /api/templates/{id}/duplicate
Authorization: Bearer {token}
```

#### Set as Default
```bash
POST /api/templates/{id}/set-default
Authorization: Bearer {token}
```

#### Activate/Deactivate Template
```bash
POST /api/templates/{id}/activate
POST /api/templates/{id}/deactivate
Authorization: Bearer {token}
```

#### Preview Template
```bash
POST /api/templates/preview
Authorization: Bearer {token}

{
  "templateId": 1,
  "variableValues": {
    "FirstName": "John",
    "LastName": "Doe",
    "CompanyName": "Acme Corp"
  }
}
```

Response includes character count information:
```json
{
  "success": true,
  "data": {
    "subject": null,
    "messageBody": "Hello John! Thanks for joining Acme Corp!",
    "htmlContent": null,
    "mediaUrls": null,
    "missingVariables": [],
    "subjectCharacterCount": null,
    "messageBodyCharacterCount": {
      "characterCount": 45,
      "smsSegments": 1,
      "containsUnicode": false,
      "recommendedMaxLength": 160,
      "exceedsRecommendedLength": false
    }
  }
}
```

#### Preview with Contact Data
```bash
POST /api/templates/preview
Authorization: Bearer {token}

{
  "templateId": 1,
  "contactId": 100,
  "variableValues": {
    "CompanyName": "Acme Corp"
  }
}
```
Contact data (FirstName, LastName, Email, Phone) is automatically loaded.

#### Extract Variables
```bash
POST /api/templates/extract-variables
Authorization: Bearer {token}
Content-Type: application/json

"Hello {{FirstName}} {{LastName}}! Your order {{OrderNumber}} is ready."
```

Response:
```json
{
  "success": true,
  "data": ["FirstName", "LastName", "OrderNumber"]
}
```

#### Get Template Statistics
```bash
GET /api/templates/{id}/stats
Authorization: Bearer {token}
```

Response:
```json
{
  "success": true,
  "data": {
    "templateId": 1,
    "templateName": "Welcome SMS",
    "totalCampaigns": 15,
    "totalMessages": 12500,
    "lastUsedAt": "2026-01-15T14:30:00Z",
    "successRate": 96.5
  }
}
```

### Character Count

#### Calculate Character Count
```bash
POST /api/templates/calculate-character-count
Authorization: Bearer {token}

{
  "content": "This is a test message with some content",
  "channel": 0,
  "isSubject": false
}
```

Response:
```json
{
  "success": true,
  "data": {
    "characterCount": 42,
    "smsSegments": 1,
    "containsUnicode": false,
    "recommendedMaxLength": 160,
    "exceedsRecommendedLength": false
  }
}
```

### URL Shortening & Tracking

#### Create Shortened URL
```bash
POST /api/urls
Authorization: Bearer {token}

{
  "campaignId": 1,
  "originalUrl": "https://example.com/product/12345?utm_source=sms&utm_campaign=summer",
  "customShortCode": "summer2026"
}
```

Response:
```json
{
  "success": true,
  "data": {
    "id": 1,
    "campaignId": 1,
    "originalUrl": "https://example.com/product/12345?utm_source=sms&utm_campaign=summer",
    "shortCode": "summer2026",
    "shortUrl": "https://short.link/summer2026",
    "clickCount": 0,
    "createdAt": "2026-01-18T10:00:00Z"
  }
}
```

If `customShortCode` is omitted, a 6-character code is auto-generated.

#### List Shortened URLs
```bash
GET /api/urls?pageNumber=1&pageSize=20&searchTerm=summer
Authorization: Bearer {token}
```

#### Get Shortened URL by ID
```bash
GET /api/urls/{id}
Authorization: Bearer {token}
```

#### Get URLs for Campaign
```bash
GET /api/urls/campaign/{campaignId}
Authorization: Bearer {token}
```

#### Get Click Statistics for URL
```bash
GET /api/urls/{id}/stats
Authorization: Bearer {token}
```

Response:
```json
{
  "success": true,
  "data": {
    "urlShortenerId": 1,
    "shortUrl": "https://short.link/summer2026",
    "originalUrl": "https://example.com/product/12345",
    "totalClicks": 1250,
    "uniqueClicks": 987,
    "firstClickAt": "2026-01-18T11:00:00Z",
    "lastClickAt": "2026-01-20T15:30:00Z",
    "clicksByDate": [
      { "date": "2026-01-18", "clickCount": 500 },
      { "date": "2026-01-19", "clickCount": 450 },
      { "date": "2026-01-20", "clickCount": 300 }
    ],
    "topReferrers": [
      { "referrer": "https://facebook.com", "clickCount": 450 },
      { "referrer": "https://twitter.com", "clickCount": 320 }
    ]
  }
}
```

#### Get Campaign URL Statistics
```bash
GET /api/urls/campaign/{campaignId}/stats
Authorization: Bearer {token}
```

Response:
```json
{
  "success": true,
  "data": {
    "campaignId": 1,
    "totalUrls": 5,
    "totalClicks": 3500,
    "uniqueClicks": 2800,
    "topUrls": [
      {
        "id": 1,
        "shortUrl": "https://short.link/summer2026",
        "originalUrl": "https://example.com/product/12345",
        "clickCount": 1250
      }
    ]
  }
}
```

#### Delete Shortened URL
```bash
DELETE /api/urls/{id}
Authorization: Bearer {token}
```

#### Public Redirect (No Authentication Required)
```bash
GET /r/{shortCode}
```

This endpoint:
1. Tracks the click (IP, user agent, referrer)
2. Increments click count
3. Redirects to the original URL

## Configuration

### appsettings.json

```json
{
  "UrlShortener": {
    "BaseUrl": "https://short.link"
  }
}
```

Customize `BaseUrl` to match your domain for shortened URLs.

## Use Cases

### Email Marketing Campaign

1. Create an email template with personalization and tracked URLs:
```json
{
  "name": "Product Launch Email",
  "channel": 2,
  "category": 0,
  "subject": "Introducing {{ProductName}} - {{DiscountPercent}}% Off!",
  "messageBody": "Hi {{FirstName}},\n\nCheck out our new product...",
  "htmlContent": "<html><body><h1>Hi {{FirstName}}!</h1><p>Check out our new <a href='{{ProductLink}}'>{{ProductName}}</a>...</p></body></html>"
}
```

2. Create shortened URL for tracking:
```bash
POST /api/urls
{
  "campaignId": 1,
  "originalUrl": "https://example.com/products/new-launch?utm_source=email"
}
```

3. Use the shortened URL in your template variables when sending.

### SMS Campaign with Segmentation

1. Calculate character count before sending:
```bash
POST /api/templates/calculate-character-count
{
  "content": "Hi {{FirstName}}! Get 50% off with code SAVE50. Shop now: {{ShortUrl}}",
  "channel": 0,
  "isSubject": false
}
```

2. Adjust message if it exceeds 160 characters to avoid multiple segments.

## Security Considerations

1. **URL Short Codes**: Generated using cryptographically secure random number generator
2. **Authorization**: All endpoints require valid JWT token (except public redirect)
3. **User Scoping**: Users can only access their own templates, campaigns, and URLs
4. **Input Validation**: All DTOs are validated
5. **SQL Injection**: Protected via Entity Framework parameterized queries

## Performance Considerations

1. **Template Preview**: Caches contact data during variable substitution
2. **Click Tracking**: Asynchronous processing to minimize redirect latency
3. **Character Count**: O(n) complexity for content parsing
4. **URL Generation**: Maximum 10 attempts to find unique short code

## Troubleshooting

### Template Preview Shows Missing Variables
- Ensure variable names match exactly (case-insensitive)
- Provide all required variables in the request
- Check template variable definitions

### URL Shortening Fails
- Verify campaign exists and user has access
- Check custom short code is 4-12 alphanumeric characters
- Ensure short code is unique

### Character Count Seems Incorrect
- Check if message contains Unicode characters
- Verify channel type (SMS vs Email)
- Unicode messages have lower limits (70 vs 160)

## Future Enhancements

1. **Drag-and-Drop Email Builder**: Visual editor for HTML emails (UI feature)
2. **A/B Testing**: Test multiple template variations
3. **Template Scheduling**: Schedule template activation/deactivation
4. **URL Expiration**: Set expiration dates for shortened URLs
5. **Advanced Analytics**: Heatmaps, conversion tracking
6. **Template Versioning**: Track template changes over time

## Support

For issues or questions:
- Check API documentation at `/swagger`
- Review server logs in `Logs/` directory
- Contact: support@marketingplatform.com
