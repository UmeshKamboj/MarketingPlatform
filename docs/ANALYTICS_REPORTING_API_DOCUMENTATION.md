# Analytics & Reporting API Documentation

## Overview
This document provides comprehensive documentation for the Analytics & Reporting feature implemented in the Marketing Platform. This feature provides campaign performance dashboards, keyword analytics, contact engagement history, conversion tracking, and data export capabilities.

## Features Implemented

### 1. Dashboard Summary
Get an overview of all marketing activities with key metrics.

### 2. Campaign Performance Analytics
Track detailed performance metrics for campaigns including delivery rates, click rates, and costs.

### 3. Contact Engagement History
Monitor individual contact engagement across all campaigns with detailed event timelines.

### 4. Conversion Tracking
Track URL clicks and conversions from campaigns with timeline analysis.

### 5. Data Export
Export reports in CSV or Excel format for offline analysis.

## API Endpoints

### Dashboard

#### GET `/api/Analytics/dashboard`
Get dashboard summary with overall statistics.

**Query Parameters:**
- `StartDate` (DateTime, optional) - Filter from date
- `EndDate` (DateTime, optional) - Filter to date
- `CampaignId` (int, optional) - Filter by campaign
- `ContactId` (int, optional) - Filter by contact
- `Channel` (string, optional) - Filter by channel
- `Status` (string, optional) - Filter by status

**Response:**
```json
{
  "success": true,
  "message": null,
  "data": {
    "totalCampaigns": 10,
    "activeCampaigns": 3,
    "completedCampaigns": 5,
    "scheduledCampaigns": 2,
    "totalMessagesSent": 5000,
    "totalMessagesDelivered": 4800,
    "totalMessagesFailed": 200,
    "overallDeliveryRate": 96.0,
    "totalClicks": 450,
    "overallClickRate": 9.375,
    "totalOptOuts": 25,
    "overallOptOutRate": 0.52,
    "totalContacts": 1000,
    "activeContacts": 950,
    "engagedContacts": 400,
    "totalSpent": 0,
    "averageCostPerMessage": 0,
    "recentCampaigns": [...],
    "topPerformingCampaigns": [...]
  }
}
```

### Campaign Performance

#### GET `/api/Analytics/campaigns/performance`
Get campaign performance analytics with filters.

**Query Parameters:**
- `StartDate` (DateTime, optional) - Filter from date
- `EndDate` (DateTime, optional) - Filter to date
- `CampaignId` (int, optional) - Filter by campaign ID
- `Status` (string, optional) - Filter by status (Draft, Scheduled, Running, Paused, Completed, Failed)

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "campaignId": 1,
      "campaignName": "Summer Sale 2024",
      "campaignType": "SMS",
      "status": "Completed",
      "startedAt": "2024-06-01T10:00:00Z",
      "completedAt": "2024-06-01T12:00:00Z",
      "totalSent": 1000,
      "totalDelivered": 980,
      "totalFailed": 20,
      "totalClicks": 150,
      "totalOptOuts": 5,
      "deliveryRate": 98.0,
      "clickRate": 15.3,
      "optOutRate": 0.51,
      "engagementRate": 15.3,
      "estimatedCost": 10.00,
      "costPerMessage": 0.01,
      "costPerClick": 0.067,
      "duration": "02:00:00",
      "averageDeliveryTime": null
    }
  ]
}
```

#### GET `/api/Analytics/campaigns/{campaignId}/performance`
Get specific campaign performance by ID.

**Path Parameters:**
- `campaignId` (int) - Campaign ID

**Response:** Same structure as single campaign performance object.

#### GET `/api/Analytics/campaigns/performance/export/csv`
Export campaign performance report to CSV.

**Query Parameters:** Same as GET performance endpoint

**Response:** File download (CSV format)

#### GET `/api/Analytics/campaigns/performance/export/excel`
Export campaign performance report to Excel.

**Query Parameters:** Same as GET performance endpoint

**Response:** File download (Excel format)

### Contact Engagement

#### GET `/api/Analytics/contacts/engagement`
Get contact engagement history with filters.

**Query Parameters:**
- `StartDate` (DateTime, optional) - Filter from date
- `EndDate` (DateTime, optional) - Filter to date
- `ContactId` (int, optional) - Filter by contact ID

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "contactId": 1,
      "firstName": "John",
      "lastName": "Doe",
      "email": "john.doe@example.com",
      "phoneNumber": "+1234567890",
      "totalMessagesSent": 50,
      "totalMessagesDelivered": 48,
      "totalClicks": 10,
      "lastEngagementDate": "2024-06-15T14:30:00Z",
      "engagementScore": 85.5,
      "campaignsParticipated": 5,
      "campaignHistory": [
        {
          "campaignId": 1,
          "campaignName": "Summer Sale",
          "participatedAt": "2024-06-01T10:00:00Z",
          "messagesReceived": 10,
          "clicks": 3,
          "optedOut": false
        }
      ],
      "engagementEvents": [
        {
          "eventDate": "2024-06-15T14:30:00Z",
          "eventType": "MessageSent",
          "campaignName": "Newsletter",
          "details": "Message sent via Email"
        }
      ]
    }
  ]
}
```

#### GET `/api/Analytics/contacts/{contactId}/engagement`
Get specific contact engagement history by ID.

**Path Parameters:**
- `contactId` (int) - Contact ID

**Query Parameters:**
- `StartDate` (DateTime, optional) - Filter from date
- `EndDate` (DateTime, optional) - Filter to date

**Response:** Same structure as single contact engagement object.

#### GET `/api/Analytics/contacts/engagement/export/csv`
Export contact engagement report to CSV.

**Query Parameters:** Same as GET engagement endpoint

**Response:** File download (CSV format)

#### GET `/api/Analytics/contacts/engagement/export/excel`
Export contact engagement report to Excel.

**Query Parameters:** Same as GET engagement endpoint

**Response:** File download (Excel format)

### Conversion Tracking

#### GET `/api/Analytics/campaigns/{campaignId}/conversions`
Get conversion tracking for a specific campaign.

**Path Parameters:**
- `campaignId` (int) - Campaign ID

**Query Parameters:**
- `StartDate` (DateTime, optional) - Filter from date
- `EndDate` (DateTime, optional) - Filter to date

**Response:**
```json
{
  "success": true,
  "data": {
    "campaignId": 1,
    "campaignName": "Summer Sale",
    "totalRecipients": 1000,
    "totalClicks": 150,
    "totalConversions": 150,
    "clickThroughRate": 15.0,
    "conversionRate": 15.0,
    "clickToConversionRate": 100.0,
    "urlConversions": [
      {
        "shortCode": "abc123",
        "originalUrl": "https://example.com/sale",
        "totalClicks": 100,
        "uniqueClicks": 85,
        "firstClickedAt": "2024-06-01T10:05:00Z",
        "lastClickedAt": "2024-06-01T23:55:00Z"
      }
    ],
    "conversionTimeline": [
      {
        "date": "2024-06-01",
        "clicks": 100,
        "conversions": 100,
        "conversionRate": 100.0
      }
    ]
  }
}
```

#### GET `/api/Analytics/conversions`
Get conversion tracking for multiple campaigns with filters.

**Query Parameters:**
- `StartDate` (DateTime, optional) - Filter from date
- `EndDate` (DateTime, optional) - Filter to date
- `CampaignId` (int, optional) - Filter by campaign ID

**Response:** Array of conversion tracking objects.

#### GET `/api/Analytics/conversions/export/csv`
Export conversion tracking report to CSV.

**Query Parameters:** Same as GET conversions endpoint

**Response:** File download (CSV format)

#### GET `/api/Analytics/conversions/export/excel`
Export conversion tracking report to Excel.

**Query Parameters:** Same as GET conversions endpoint

**Response:** File download (Excel format)

## Data Models

### ReportFilterDto
```csharp
public class ReportFilterDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? CampaignId { get; set; }
    public int? ContactId { get; set; }
    public string? Channel { get; set; }
    public string? Status { get; set; }
}
```

### CampaignPerformanceDto
Contains comprehensive campaign metrics including:
- Basic info (ID, name, type, status)
- Message statistics (sent, delivered, failed, clicks, opt-outs)
- Performance rates (delivery, click, opt-out, engagement)
- Financial metrics (estimated cost, cost per message, cost per click)
- Time metrics (duration, average delivery time)

### ContactEngagementHistoryDto
Contains contact engagement data including:
- Contact info (ID, name, email, phone)
- Engagement statistics (messages sent/delivered, clicks, score)
- Campaign participation history
- Engagement event timeline

### ConversionTrackingDto
Contains conversion data including:
- Campaign info
- Conversion metrics (recipients, clicks, conversions)
- Conversion rates
- URL-level performance
- Timeline data

### DashboardSummaryDto
Contains overall platform statistics including:
- Campaign overview (total, active, completed, scheduled)
- Message statistics (sent, delivered, failed, rates)
- Engagement metrics (clicks, opt-outs)
- Contact statistics (total, active, engaged)
- Financial summary
- Recent campaigns and top performers

## Authentication
All endpoints require JWT Bearer token authentication. Include the token in the Authorization header:
```
Authorization: Bearer {your-jwt-token}
```

## Error Responses
All endpoints return consistent error responses:
```json
{
  "success": false,
  "message": "Error message",
  "errors": ["Detailed error 1", "Detailed error 2"]
}
```

## Usage Examples

### Get Dashboard Summary
```bash
curl -X GET "https://api.example.com/api/Analytics/dashboard?StartDate=2024-01-01&EndDate=2024-12-31" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Get Campaign Performance with Filters
```bash
curl -X GET "https://api.example.com/api/Analytics/campaigns/performance?StartDate=2024-06-01&Status=Completed" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Export to Excel
```bash
curl -X GET "https://api.example.com/api/Analytics/campaigns/performance/export/excel?StartDate=2024-01-01" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -o campaign-performance.xlsx
```

### Get Contact Engagement
```bash
curl -X GET "https://api.example.com/api/Analytics/contacts/123/engagement" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Get Conversion Tracking
```bash
curl -X GET "https://api.example.com/api/Analytics/campaigns/456/conversions" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

## Integration with Existing Features

### Keyword Analytics
Existing keyword analytics can be accessed via the Keywords controller at `/api/Keywords/{id}/analytics`. The analytics feature complements this with campaign-wide and contact-level insights.

### Campaign Analytics
Campaign analytics are automatically tracked in the `CampaignAnalytics` entity and surfaced through these endpoints.

### Contact Engagement
Contact engagement is tracked in the `ContactEngagement` entity and enhanced with campaign participation and event timelines.

### URL Tracking
URL clicks are tracked through the `URLShortener` and `URLClick` entities, providing conversion insights.

## Notes

1. **Date Filters**: All date filters use UTC timezone. Dates should be provided in ISO 8601 format.
2. **Pagination**: The current implementation returns all matching records. Consider adding pagination for large datasets in production.
3. **Performance**: For large datasets, consider adding caching or background job processing for report generation.
4. **Cost Calculation**: Current implementation uses simplified cost calculation ($0.01 per message). Update with actual provider costs.
5. **Export Formats**: CSV exports use UTF-8 encoding. Excel exports use EPPlus with non-commercial license.

## Future Enhancements

1. Add real-time analytics updates using SignalR
2. Implement custom report builder
3. Add scheduled report delivery via email
4. Implement data aggregation for faster queries on historical data
5. Add visualization data endpoints (charts, graphs)
6. Implement user-defined metrics and KPIs
7. Add comparison features (compare campaigns, time periods)
8. Implement predictive analytics and forecasting
