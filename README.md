# Marketing Platform - SMS, MMS & Email Marketing Solution

## Overview
A robust, enterprise-grade SMS, MMS & Email Marketing Platform built with ASP.NET Core 8.0, SQL Server, and Bootstrap 5.

## Technology Stack
- **Framework**: ASP.NET Core 8.0
- **Database**: SQL Server with Entity Framework Core
- **Frontend**: Bootstrap 5, jQuery, Font Awesome
- **Authentication**: ASP.NET Core Identity with JWT
- **Background Jobs**: Hangfire
- **Logging**: Serilog
- **Payments**: Stripe
- **API Documentation**: Swagger/OpenAPI

## Projects

### MarketingPlatform.API
RESTful API for all platform operations.

### MarketingPlatform.Web
Web application (MVC) consuming the API.

### MarketingPlatform.Core
Domain entities, interfaces, enums, and constants.

### MarketingPlatform.Infrastructure
Data access, repositories, and external service integrations.

### MarketingPlatform.Application
Business logic, services, DTOs, and validators.

### MarketingPlatform.Shared
Shared utilities and helpers.

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB, Express, or Full)
- Visual Studio 2022 or VS Code
- Node.js (optional, for frontend tooling)

### Setup Instructions

1. **Clone the repository**
   ```bash
   git clone https://github.com/UmeshKamboj/MarketingPlatform.git
   cd MarketingPlatform
   ```

2. **Update connection strings**
   - Update `appsettings.json` in both API and Web projects with your SQL Server connection string

3. **Apply database migrations**
   ```bash
   cd src/MarketingPlatform.Infrastructure
   dotnet ef database update --startup-project ../MarketingPlatform.API
   ```
   
   Or run the API which will automatically migrate and seed the database:
   ```bash
   cd src/MarketingPlatform.API
   dotnet run
   ```

4. **Run the API**
   ```bash
   cd src/MarketingPlatform.API
   dotnet run
   ```
   API will be available at: https://localhost:7001

5. **Run the Web Application**
   ```bash
   cd src/MarketingPlatform.Web
   dotnet run
   ```
   Web app will be available at: https://localhost:7002

## Configuration

### API Settings (appsettings.json)
- **ConnectionStrings**: Database connection
- **JwtSettings**: JWT authentication configuration
- **Stripe**: Payment gateway keys
- **Serilog**: Logging configuration

### Web Settings (appsettings.json)
- **ConnectionStrings**: Database connection
- **ApiSettings**: API base URL
- **Stripe**: Publishable key

## Database Setup

The database is automatically migrated and seeded when you run the API. However, you can also manually manage migrations:

### Create Migration
```bash
cd src/MarketingPlatform.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../MarketingPlatform.API
```

### Update Database
```bash
cd src/MarketingPlatform.Infrastructure
dotnet ef database update --startup-project ../MarketingPlatform.API
```

### Verify Setup
After running the API, verify the following:
- Database `MarketingPlatformDb` is created
- All tables are created successfully
- Seed data is inserted:
  - 3 roles: Admin, User, Manager
  - Admin user: admin@marketingplatform.com / Admin@123456
  - 3 subscription plans: Free, Pro, Enterprise
  - 2 message providers: Twilio SMS, SendGrid Email

## Testing Authentication

### Register User
```bash
POST /api/auth/register
{
  "email": "test@example.com",
  "password": "Test@12345",
  "confirmPassword": "Test@12345",
  "firstName": "Test",
  "lastName": "User"
}
```

### Login
```bash
POST /api/auth/login
{
  "email": "test@example.com",
  "password": "Test@12345"
}
```

### Get Current User (requires Bearer token)
```bash
GET /api/auth/me
Authorization: Bearer {token}
```

### Refresh Token
```bash
POST /api/auth/refresh-token
{
  "token": "{expired_token}",
  "refreshToken": "{refresh_token}"
}
```

### Change Password (requires Bearer token)
```bash
POST /api/auth/change-password
Authorization: Bearer {token}
{
  "currentPassword": "Test@12345",
  "newPassword": "NewTest@12345",
  "confirmNewPassword": "NewTest@12345"
}
```

### Logout (requires Bearer token)
```bash
POST /api/auth/logout
Authorization: Bearer {token}
```

## Testing User Management

### Get Current User Profile (requires Bearer token)
```bash
GET /api/users/profile
Authorization: Bearer {token}
```

### Update Profile
```bash
PUT /api/users/profile
Authorization: Bearer {token}
{
  "firstName": "Updated",
  "lastName": "Name",
  "phoneNumber": "+1234567890"
}
```

### Get User Stats
```bash
GET /api/users/stats
Authorization: Bearer {token}
```

### List All Users (Admin only)
```bash
GET /api/users?pageNumber=1&pageSize=10&searchTerm=test
Authorization: Bearer {admin_token}
```

### Get Specific User
```bash
GET /api/users/{userId}
Authorization: Bearer {token}
```

### Deactivate User (Admin only)
```bash
POST /api/users/{userId}/deactivate
Authorization: Bearer {admin_token}
```

### Activate User (Admin only)
```bash
POST /api/users/{userId}/activate
Authorization: Bearer {admin_token}
```

## Testing Contact Management

### Create Contact
```bash
POST /api/contacts
Authorization: Bearer {token}
Content-Type: application/json
{
  "phoneNumber": "+1234567890",
  "email": "john@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "country": "USA"
}
```

### Get Contacts (paginated)
```bash
GET /api/contacts?pageNumber=1&pageSize=20&searchTerm=john
Authorization: Bearer {token}
```

### Get Single Contact
```bash
GET /api/contacts/{id}
Authorization: Bearer {token}
```

### Update Contact
```bash
PUT /api/contacts/{id}
Authorization: Bearer {token}
Content-Type: application/json
{
  "phoneNumber": "+1234567890",
  "email": "john.updated@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "isActive": true
}
```

### Delete Contact
```bash
DELETE /api/contacts/{id}
Authorization: Bearer {token}
```

### Import Contacts from CSV
```bash
POST /api/contacts/import/csv?groupId=1
Authorization: Bearer {token}
Content-Type: multipart/form-data
file: contacts.csv
```

CSV Format:
```csv
PhoneNumber,Email,FirstName,LastName,Country,City,PostalCode
+1234567890,john@example.com,John,Doe,USA,New York,10001
+9876543210,jane@example.com,Jane,Smith,USA,Los Angeles,90001
```

### Import Contacts from Excel
```bash
POST /api/contacts/import/excel
Authorization: Bearer {token}
Content-Type: multipart/form-data
file: contacts.xlsx
```

Excel Format: Same columns as CSV (PhoneNumber, Email, FirstName, LastName, Country, City, PostalCode)

### Export Contacts to CSV (all)
```bash
POST /api/contacts/export/csv
Authorization: Bearer {token}
Content-Type: application/json
Body: null
```

### Export Contacts to CSV (selected)
```bash
POST /api/contacts/export/csv
Authorization: Bearer {token}
Content-Type: application/json
Body: [1, 2, 3, 4, 5]
```

### Export Contacts to Excel
```bash
POST /api/contacts/export/excel
Authorization: Bearer {token}
Content-Type: application/json
Body: null
```

### Search Contacts
```bash
GET /api/contacts/search?searchTerm=john
Authorization: Bearer {token}
```

### Create Contact Group
```bash
POST /api/contactgroups
Authorization: Bearer {token}
Content-Type: application/json
{
  "name": "VIP Customers",
  "description": "High value customers",
  "isStatic": true
}
```

### Get Contact Groups (paginated)
```bash
GET /api/contactgroups?pageNumber=1&pageSize=20
Authorization: Bearer {token}
```

### Get Single Contact Group
```bash
GET /api/contactgroups/{id}
Authorization: Bearer {token}
```

### Update Contact Group
```bash
PUT /api/contactgroups/{id}
Authorization: Bearer {token}
Content-Type: application/json
{
  "name": "VIP Customers Updated",
  "description": "Updated description",
  "isStatic": true
}
```

### Delete Contact Group
```bash
DELETE /api/contactgroups/{id}
Authorization: Bearer {token}
```

### Add Contact to Group
```bash
POST /api/contactgroups/{groupId}/contacts/{contactId}
Authorization: Bearer {token}
```

### Remove Contact from Group
```bash
DELETE /api/contactgroups/{groupId}/contacts/{contactId}
Authorization: Bearer {token}
```

### Get Group Contacts
```bash
GET /api/contactgroups/{id}/contacts?pageNumber=1&pageSize=20
Authorization: Bearer {token}
```

## Testing Template Management

### Create SMS Template
```bash
POST /api/templates
Authorization: Bearer {token}
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

### Create Email Template
```bash
POST /api/templates
Authorization: Bearer {token}
{
  "name": "Newsletter Template",
  "description": "Monthly newsletter",
  "channel": 2,
  "category": 1,
  "subject": "{{Month}} Newsletter - {{CompanyName}}",
  "messageBody": "Plain text version for {{FirstName}} {{LastName}}",
  "htmlContent": "<html><body><h1>Hello {{FirstName}}!</h1><p>{{NewsContent}}</p></body></html>",
  "variables": [
    {
      "name": "FirstName",
      "isRequired": true
    },
    {
      "name": "LastName",
      "isRequired": true
    },
    {
      "name": "Month",
      "defaultValue": "January",
      "isRequired": false
    },
    {
      "name": "CompanyName",
      "defaultValue": "Marketing Platform",
      "isRequired": false
    },
    {
      "name": "NewsContent",
      "isRequired": true
    }
  ]
}
```

### List All Templates
```bash
GET /api/templates?pageNumber=1&pageSize=20&searchTerm=welcome
Authorization: Bearer {token}
```

### Get Single Template
```bash
GET /api/templates/{id}
Authorization: Bearer {token}
```

### Get Templates by Channel
```bash
GET /api/templates/channel/0
Authorization: Bearer {token}
```

Channel values:
- 0 = SMS
- 1 = MMS
- 2 = Email

### Get Templates by Category
```bash
GET /api/templates/category/0
Authorization: Bearer {token}
```

Category values:
- 0 = Promotional
- 1 = Transactional
- 2 = Reminder
- 3 = Alert
- 4 = Custom

### Update Template
```bash
PUT /api/templates/{id}
Authorization: Bearer {token}
{
  "name": "Updated Welcome SMS",
  "description": "Updated description",
  "messageBody": "Hello {{FirstName}}! Thanks for joining {{CompanyName}}!"
}
```

### Delete Template
```bash
DELETE /api/templates/{id}
Authorization: Bearer {token}
```

Note: Cannot delete templates that are in use (UsageCount > 0)

### Duplicate Template
```bash
POST /api/templates/{id}/duplicate
Authorization: Bearer {token}
```

### Set as Default Template
```bash
POST /api/templates/{id}/set-default
Authorization: Bearer {token}
```

Note: Only one default template per Channel + Category combination

### Activate Template
```bash
POST /api/templates/{id}/activate
Authorization: Bearer {token}
```

### Deactivate Template
```bash
POST /api/templates/{id}/deactivate
Authorization: Bearer {token}
```

### Preview Template with Variables
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

Response:
```json
{
  "success": true,
  "data": {
    "subject": null,
    "messageBody": "Hello John! Thanks for joining Acme Corp!",
    "htmlContent": null,
    "mediaUrls": null,
    "missingVariables": []
  }
}
```

### Preview with Contact Data
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

Note: Contact data (FirstName, LastName, Email, Phone) is automatically loaded

### Extract Variables from Content
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

### Get Template Usage Statistics
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

### Template Variable Syntax
Templates support variable substitution using `{{VariableName}}` syntax:
- Variable names must be alphanumeric with underscores
- Variable matching is case-insensitive
- Variables can be in subject, messageBody, or htmlContent
- Common contact variables: FirstName, LastName, Email, Phone, PhoneNumber

### Template Examples

**SMS Template:**
```
Hi {{FirstName}}! Your appointment with {{DoctorName}} is on {{AppointmentDate}} at {{AppointmentTime}}. Reply YES to confirm.
```

**Email Template:**
```
Subject: {{Subject}}
Body HTML:
<html>
  <body>
    <h1>Hello {{FirstName}} {{LastName}}!</h1>
    <p>Thank you for your order #{{OrderNumber}}.</p>
    <p>Total: ${{OrderTotal}}</p>
    <a href="{{TrackingLink}}">Track Your Order</a>
  </body>
</html>
```

**MMS Template:**
```
Check out our new {{ProductName}}! {{Description}} Price: ${{Price}}
MediaUrls: ["https://example.com/product-image.jpg"]
```

## Testing Campaign Management

### Create SMS Campaign
```bash
POST /api/campaigns
Authorization: Bearer {token}
Content-Type: application/json
{
  "name": "Summer Sale 2026",
  "description": "Promotional SMS campaign",
  "type": 0,
  "content": {
    "channel": 0,
    "messageBody": "Get 50% off all summer items! Use code SUMMER50",
    "personalizationTokens": {
      "FirstName": "Customer",
      "DiscountCode": "SUMMER50"
    }
  },
  "audience": {
    "targetType": 1,
    "groupIds": [1, 2, 3]
  },
  "schedule": {
    "scheduleType": 0,
    "scheduledDate": "2026-07-01T10:00:00Z",
    "timeZoneAware": true,
    "preferredTimeZone": "America/New_York"
  }
}
```

### Create Email Campaign
```bash
POST /api/campaigns
Authorization: Bearer {token}
{
  "name": "Newsletter - July 2026",
  "type": 2,
  "content": {
    "channel": 2,
    "subject": "Your Monthly Newsletter",
    "messageBody": "Plain text version",
    "htmlContent": "<html><body><h1>Hello!</h1></body></html>"
  },
  "audience": {
    "targetType": 0
  }
}
```

### List All Campaigns
```bash
GET /api/campaigns?pageNumber=1&pageSize=20&searchTerm=Summer
Authorization: Bearer {token}
```

### Get Single Campaign
```bash
GET /api/campaigns/{id}
Authorization: Bearer {token}
```

### Get Campaigns by Status
```bash
GET /api/campaigns/status/1
Authorization: Bearer {token}
```

Status values:
- 0 = Draft
- 1 = Scheduled
- 2 = Running
- 3 = Paused
- 4 = Completed
- 5 = Failed

### Update Campaign (Draft only)
```bash
PUT /api/campaigns/{id}
Authorization: Bearer {token}
{
  "name": "Updated Campaign Name",
  "description": "Updated description",
  "content": {
    "channel": 0,
    "messageBody": "Updated message content"
  }
}
```

### Delete Campaign
```bash
DELETE /api/campaigns/{id}
Authorization: Bearer {token}
```

Note: Cannot delete Running campaigns

### Duplicate Campaign
```bash
POST /api/campaigns/{id}/duplicate
Authorization: Bearer {token}
```

### Schedule Campaign
```bash
POST /api/campaigns/{id}/schedule
Authorization: Bearer {token}
Content-Type: application/json
"2026-07-01T10:00:00Z"
```

### Start Campaign Immediately
```bash
POST /api/campaigns/{id}/start
Authorization: Bearer {token}
```

### Pause Running Campaign
```bash
POST /api/campaigns/{id}/pause
Authorization: Bearer {token}
```

### Resume Paused Campaign
```bash
POST /api/campaigns/{id}/resume
Authorization: Bearer {token}
```

### Cancel Campaign
```bash
POST /api/campaigns/{id}/cancel
Authorization: Bearer {token}
```

### Get Campaign Statistics
```bash
GET /api/campaigns/{id}/stats
Authorization: Bearer {token}
```

Response:
```json
{
  "success": true,
  "data": {
    "totalSent": 1500,
    "delivered": 1450,
    "failed": 30,
    "bounced": 20,
    "deliveryRate": 96.67,
    "failureRate": 2.00,
    "estimatedCost": 75.50
  }
}
```

### Calculate Audience Size
```bash
POST /api/campaigns/calculate-audience
Authorization: Bearer {token}
{
  "targetType": 1,
  "groupIds": [1, 2, 3, 4, 5]
}
```

Response:
```json
{
  "success": true,
  "data": 2584,
  "message": "Success"
}
```

### Campaign Enum Reference

**CampaignType:**
- 0 = SMS
- 1 = MMS
- 2 = Email
- 3 = Multi

**CampaignStatus:**
- 0 = Draft
- 1 = Scheduled
- 2 = Running
- 3 = Paused
- 4 = Completed
- 5 = Failed

**ChannelType:**
- 0 = SMS
- 1 = MMS
- 2 = Email

**TargetType:**
- 0 = All
- 1 = Groups
- 2 = Segments

**ScheduleType:**
- 0 = OneTime
- 1 = Recurring
- 2 = Drip

## Features (Planned)

- ✅ Task 1.1: Solution structure and core projects
- ✅ Task 1.2: Database foundation
- ✅ Task 1.3: Authentication & Authorization Core
- ✅ Task 2.1: API Foundation - Repository Pattern & Core Services
- ✅ Task 2.2: Contact Management - Full CRUD with Import/Export
- ✅ Task 2.3: Campaign Management - Core Campaign CRUD & Scheduling
- ✅ Task 2.5: Template Management - Reusable Message Templates with Variables ← **Current**
- ⏳ Keyword campaigns
- ⏳ Automation & workflows
- ⏳ Analytics & reporting
- ⏳ Billing & subscriptions
- ⏳ Super admin platform

## Project Status
🚧 **In Development** - Task 2.5 Complete

## License
MIT License

## Contact
For questions or support, contact: support@marketingplatform.com
