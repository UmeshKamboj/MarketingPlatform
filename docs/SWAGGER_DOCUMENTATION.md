# Swagger API Documentation

## Overview
The Marketing Platform API includes comprehensive Swagger/OpenAPI documentation for all endpoints. Swagger provides an interactive interface to explore and test the API.

## Accessing Swagger UI

### Development Environment
When running the API in Development mode:

```bash
cd src/MarketingPlatform.API
dotnet run
```

Access Swagger UI at: **https://localhost:7001/swagger**

### Swagger Configuration

The API is configured with the following Swagger features:

1. **XML Documentation**: All controller actions include XML comments that appear in Swagger
2. **JWT Authentication**: Swagger UI includes an "Authorize" button to add Bearer tokens
3. **API Versioning**: Currently using v1
4. **Detailed Descriptions**: Each endpoint includes:
   - Summary
   - Parameter descriptions
   - Response types
   - Example values

## Configuration Details

### Swagger Setup (Program.cs)
```csharp
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Marketing Platform API",
        Version = "v1",
        Description = "RESTful API for Marketing Platform - Comprehensive SMS, MMS & Email Marketing Solution",
        Contact = new OpenApiContact
        {
            Name = "Marketing Platform Support",
            Email = "support@marketingplatform.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // XML comments included
    options.IncludeXmlComments(xmlPath);
    
    // JWT Authentication
    options.AddSecurityDefinition("Bearer", ...);
    
    // Annotations enabled
    options.EnableAnnotations();
});
```

### XML Documentation
XML documentation is enabled in `MarketingPlatform.API.csproj`:

```xml
<PropertyGroup>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

## API Endpoints Overview

### Authentication & Authorization
- **POST** `/api/auth/register` - Register new user
- **POST** `/api/auth/login` - Login and get JWT token
- **POST** `/api/auth/refresh-token` - Refresh expired token
- **POST** `/api/auth/logout` - Logout user
- **POST** `/api/auth/change-password` - Change user password

### OAuth2/SSO
- **GET** `/api/oauth2/providers` - Get enabled OAuth2 providers
- **GET** `/api/oauth2/authorize/{providerName}` - Get authorization URL
- **POST** `/api/oauth2/callback/{providerName}` - Handle OAuth2 callback
- **POST** `/api/oauth2/link/{providerName}` - Link external account
- **DELETE** `/api/oauth2/unlink/{providerName}` - Unlink external account
- **GET** `/api/oauth2/admin/providers` - Get all providers (Admin)
- **POST** `/api/oauth2/admin/providers` - Create provider (Admin)
- **PUT** `/api/oauth2/admin/providers/{id}` - Update provider (Admin)
- **DELETE** `/api/oauth2/admin/providers/{id}` - Delete provider (Admin)

### User Management
- **GET** `/api/users/profile` - Get current user profile
- **GET** `/api/users/{id}` - Get user by ID
- **GET** `/api/users` - List all users (Admin)
- **PUT** `/api/users/profile` - Update profile
- **DELETE** `/api/users/{id}` - Delete user (Admin)

### Contact Management
- **GET** `/api/contacts` - List contacts (paginated)
- **GET** `/api/contacts/{id}` - Get contact by ID
- **POST** `/api/contacts` - Create new contact
- **PUT** `/api/contacts/{id}` - Update contact
- **DELETE** `/api/contacts/{id}` - Delete contact
- **POST** `/api/contacts/import-csv` - Import from CSV
- **POST** `/api/contacts/import-excel` - Import from Excel

### Contact Groups
- **GET** `/api/contactgroups` - List contact groups
- **GET** `/api/contactgroups/{id}` - Get group by ID
- **POST** `/api/contactgroups` - Create new group
- **PUT** `/api/contactgroups/{id}` - Update group
- **DELETE** `/api/contactgroups/{id}` - Delete group
- **POST** `/api/contactgroups/{groupId}/contacts/{contactId}` - Add contact to group
- **DELETE** `/api/contactgroups/{groupId}/contacts/{contactId}` - Remove contact from group

### Contact Tags
- **GET** `/api/contacttags` - List all tags
- **GET** `/api/contacttags/{id}` - Get tag by ID
- **POST** `/api/contacttags` - Create new tag
- **PUT** `/api/contacttags/{id}` - Update tag
- **DELETE** `/api/contacttags/{id}` - Delete tag
- **POST** `/api/contacttags/assign` - Assign tag to contact
- **POST** `/api/contacttags/remove` - Remove tag from contact

### Campaign Management
- **GET** `/api/campaigns` - List campaigns
- **GET** `/api/campaigns/{id}` - Get campaign by ID
- **POST** `/api/campaigns` - Create new campaign
- **PUT** `/api/campaigns/{id}` - Update campaign
- **DELETE** `/api/campaigns/{id}` - Delete campaign
- **POST** `/api/campaigns/{id}/schedule` - Schedule campaign
- **POST** `/api/campaigns/{id}/start` - Start campaign immediately
- **POST** `/api/campaigns/{id}/pause` - Pause running campaign
- **GET** `/api/campaigns/{id}/variants` - Get A/B test variants

### Templates
- **GET** `/api/templates` - List templates
- **GET** `/api/templates/{id}` - Get template by ID
- **POST** `/api/templates` - Create new template
- **PUT** `/api/templates/{id}` - Update template
- **DELETE** `/api/templates/{id}` - Delete template
- **POST** `/api/templates/preview` - Preview template with variables
- **POST** `/api/templates/extract-variables` - Extract variables from content
- **GET** `/api/templates/stats/{id}` - Get template usage statistics

### Messages
- **GET** `/api/messages` - List messages
- **GET** `/api/messages/{id}` - Get message by ID
- **POST** `/api/messages/create` - Create new message
- **POST** `/api/messages/bulk` - Create bulk messages
- **POST** `/api/messages/send-now` - Send message immediately
- **GET** `/api/messages/delivery-report` - Get delivery report

### Routing Configuration
- **GET** `/api/routingconfig` - Get all routing configs (Admin)
- **GET** `/api/routingconfig/{id}` - Get config by ID (Admin)
- **GET** `/api/routingconfig/channel/{channel}` - Get config by channel (Admin)
- **POST** `/api/routingconfig` - Create routing config (Admin)
- **PUT** `/api/routingconfig/{id}` - Update routing config (Admin)
- **DELETE** `/api/routingconfig/{id}` - Delete routing config (Admin)
- **GET** `/api/routingconfig/delivery-attempts/{messageId}` - Get delivery attempts
- **GET** `/api/routingconfig/stats/channel/{channel}` - Get channel statistics (Admin)
- **GET** `/api/routingconfig/stats/overall` - Get overall statistics (Admin)

### URL Shortening
- **POST** `/api/urls/create` - Create shortened URL
- **GET** `/api/urls` - List all URLs
- **GET** `/api/urls/{id}` - Get URL by ID
- **GET** `/api/urls/campaign/{campaignId}` - Get campaign URLs
- **GET** `/api/urls/stats/{id}` - Get URL click statistics
- **DELETE** `/api/urls/{id}` - Delete URL

### Keywords (SMS)
- **GET** `/api/keywords` - List keywords
- **POST** `/api/keywords/create` - Create keyword
- **PUT** `/api/keywords/{id}` - Update keyword
- **DELETE** `/api/keywords/{id}` - Delete keyword
- **GET** `/api/keywords/activities` - Get keyword activities
- **POST** `/api/keywords/process-inbound` - Process inbound SMS

### Journeys/Workflows
- **GET** `/api/journeys` - List workflows
- **GET** `/api/journeys/{id}` - Get workflow by ID
- **POST** `/api/journeys` - Create workflow
- **PUT** `/api/journeys/{id}` - Update workflow
- **DELETE** `/api/journeys/{id}` - Delete workflow
- **POST** `/api/journeys/execute` - Execute workflow
- **GET** `/api/journeys/executions` - Get workflow executions

### Analytics & Reporting
- **GET** `/api/analytics/dashboard` - Get dashboard data
- **GET** `/api/analytics/campaign-performance` - Campaign performance metrics
- **GET** `/api/analytics/contact-engagement` - Contact engagement metrics
- **POST** `/api/analytics/export-csv` - Export analytics to CSV
- **POST** `/api/analytics/export-excel` - Export analytics to Excel

### Audience Segmentation
- **POST** `/api/audience/evaluate-segment` - Evaluate segment rules
- **POST** `/api/audience/calculate-size` - Calculate audience size
- **POST** `/api/audience/refresh-dynamic-group` - Refresh dynamic group membership

### Compliance
- **GET** `/api/compliance/consent/{contactId}` - Get consent history
- **POST** `/api/compliance/record-consent` - Record consent
- **POST** `/api/compliance/revoke-consent` - Revoke consent
- **GET** `/api/compliance/settings` - Get compliance settings

### Suppression Lists
- **GET** `/api/suppressionlists` - List suppression entries
- **POST** `/api/suppressionlists` - Create suppression entry
- **POST** `/api/suppressionlists/bulk` - Bulk create entries
- **DELETE** `/api/suppressionlists/{id}` - Delete entry
- **POST** `/api/suppressionlists/check-suppressed` - Check if contact is suppressed

### Rate Limits
- **GET** `/api/ratelimits/status` - Get rate limit status
- **GET** `/api/ratelimits` - List rate limits (Admin)
- **POST** `/api/ratelimits` - Create rate limit (Admin)
- **PUT** `/api/ratelimits/{id}` - Update rate limit (Admin)
- **DELETE** `/api/ratelimits/{id}` - Delete rate limit (Admin)

### Integrations
- **POST** `/api/integration/crm/test-connection` - Test CRM connection
- **POST** `/api/integration/crm/fields` - Get CRM fields
- **POST** `/api/integration/crm/sync-from-crm` - Sync contacts from CRM
- **POST** `/api/integration/crm/sync-to-crm` - Sync contacts to CRM

### Webhooks
- **POST** `/api/webhooks/message-status` - Message status webhook
- **POST** `/api/webhooks/sms-inbound` - Inbound SMS webhook
- **POST** `/api/webhooks/sms-delivery` - SMS delivery webhook
- **POST** `/api/webhooks/email-delivery` - Email delivery webhook
- **POST** `/api/webhooks/opt-out` - Opt-out webhook

### Roles & Permissions
- **GET** `/api/roles` - List roles (Admin)
- **POST** `/api/roles` - Create role (Admin)
- **PUT** `/api/roles/{id}` - Update role (Admin)
- **DELETE** `/api/roles/{id}` - Delete role (Admin)
- **POST** `/api/roles/assign-role` - Assign role to user (Admin)

### Health Check
- **GET** `/api/health` - API health status

## Authentication in Swagger

To test authenticated endpoints in Swagger:

1. First, call `/api/auth/login` with credentials:
   ```json
   {
     "email": "admin@marketingplatform.com",
     "password": "Admin@123456"
   }
   ```

2. Copy the `token` from the response

3. Click the "Authorize" button at the top of Swagger UI

4. Enter: `Bearer <your-token>`

5. Click "Authorize" and "Close"

6. Now you can test all authenticated endpoints

## Response Format

All API responses follow a consistent format:

### Success Response
```json
{
  "success": true,
  "data": { ... },
  "message": "Operation successful",
  "errors": null
}
```

### Error Response
```json
{
  "success": false,
  "data": null,
  "message": "Error message",
  "errors": ["Detailed error 1", "Detailed error 2"]
}
```

## Pagination

Paginated endpoints accept these query parameters:
- `pageNumber` (default: 1, min: 1)
- `pageSize` (default: 20, min: 1, max: 100)
- `searchTerm` (optional filter)

Paginated responses include:
```json
{
  "items": [...],
  "pageNumber": 1,
  "pageSize": 20,
  "totalCount": 150,
  "totalPages": 8
}
```

## Enum Values

Common enums used in the API:

### ChannelType
- `0` = SMS
- `1` = MMS
- `2` = Email

### CampaignStatus
- `0` = Draft
- `1` = Scheduled
- `2` = Running
- `3` = Paused
- `4` = Completed
- `5` = Failed

### MessageStatus
- `0` = Pending
- `1` = Queued
- `2` = Sent
- `3` = Delivered
- `4` = Failed
- `5` = Bounced

### Permission (RBAC)
- Various permissions for granular access control
- Check `/api/roles` endpoint for full list

## Additional Resources

- **API Source Code**: `/src/MarketingPlatform.API/Controllers/`
- **Service Layer**: `/src/MarketingPlatform.Application/Services/`
- **Data Models**: `/src/MarketingPlatform.Core/Entities/`
- **DTOs**: `/src/MarketingPlatform.Application/DTOs/`

## Generating swagger.json

To export the Swagger specification as JSON:

1. Run the API in Development mode
2. Visit: `https://localhost:7001/swagger/v1/swagger.json`
3. Save the JSON file for external tools (Postman, API clients, etc.)

Or use the provided script:
```bash
./generate-swagger.sh
```

## Support

For API support or questions:
- Email: support@marketingplatform.com
- Documentation: See README.md and other markdown files in the repository
