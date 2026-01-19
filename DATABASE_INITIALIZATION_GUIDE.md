# Database Initialization and Seeding Guide

## Overview

This guide explains the enhanced database initialization and seeding process for the Marketing Platform. The system now includes comprehensive error handling, detailed logging, and automatic validation to ensure database integrity.

## Key Improvements

### 1. Comprehensive Error Handling
- Database connection validation before migrations
- Detailed error logging with inner exception details
- Graceful error recovery for non-critical failures
- Transaction support for seed operations

### 2. Enhanced Logging
- Step-by-step initialization logging
- Migration status tracking
- Seed data creation logging
- Table verification logging

### 3. Automatic Validation
- Pending migrations detection
- Applied migrations verification
- Required tables existence checks
- Seed data idempotency (prevents duplicates)

### 4. Dual Project Support
- **API Project**: Full migration and seeding
- **Web Project**: Migration sync and page content seeding

## Database Initialization Flow

### API Project (Program.cs)

```
1. Test Database Connection
   ├─ Validates connection to SQL Server
   └─ Logs connection status

2. Apply Migrations
   ├─ Detects pending migrations
   ├─ Applies migrations sequentially
   └─ Verifies successful application

3. Verify Tables
   ├─ Checks for required tables
   └─ Logs any missing tables

4. Seed Data
   ├─ Identity Roles (Admin, User, Manager, SuperAdmin)
   ├─ Custom Roles (SuperAdmin, Admin, Manager, User, Analyst, Viewer)
   ├─ Default Users (admin, manager, user, analyst, viewer)
   ├─ Subscription Plans (Free, Pro, Enterprise)
   ├─ Message Providers (Twilio SMS, SendGrid Email)
   ├─ Channel Routing Configurations
   ├─ Pricing Models (Starter, Professional, Enterprise)
   ├─ Landing Page Settings (47 settings)
   └─ Page Content (Privacy Policy, Terms of Service)
```

### Web Project (Program.cs)

```
1. Test Database Connection
   ├─ Validates connection to SQL Server
   └─ Logs connection status

2. Apply Migrations
   ├─ Checks for pending migrations
   ├─ Applies if needed
   └─ Ensures database is up to date

3. Seed Page Content
   ├─ Privacy Policy
   └─ Terms of Service
```

## Seeded Data

### Identity Roles
- **Admin**: Administrative access
- **User**: Standard user access
- **Manager**: Management access
- **SuperAdmin**: Full system access

### Custom Roles with Permissions

| Role | Description | Key Permissions |
|------|-------------|-----------------|
| **SuperAdmin** | Full system access | All permissions |
| **Admin** | Administrator with user management | ViewCampaigns, CreateCampaigns, EditCampaigns, DeleteCampaigns, ViewContacts, CreateContacts, EditContacts, DeleteContacts, ViewUsers, CreateUsers, EditUsers, ViewRoles, ManageSettings, ViewAuditLogs |
| **Manager** | Campaign and contact management | ViewCampaigns, CreateCampaigns, EditCampaigns, ViewContacts, CreateContacts, EditContacts, ViewAnalytics, ViewUsers |
| **User** | Standard user access | ViewCampaigns, CreateCampaigns, ViewContacts, CreateContacts, ViewAnalytics |
| **Analyst** | Read access with analytics | ViewCampaigns, ViewContacts, ViewAnalytics, ViewDetailedAnalytics, ExportAnalytics |
| **Viewer** | Read-only access | ViewCampaigns, ViewContacts, ViewAnalytics |

### Default Users

| Email | Password | Role | Purpose |
|-------|----------|------|---------|
| admin@marketingplatform.com | Admin@123456 | SuperAdmin | System administrator |
| manager@marketingplatform.com | Manager@123456 | Manager | Campaign manager |
| user@marketingplatform.com | User@123456 | User | Standard user |
| analyst@marketingplatform.com | Analyst@123456 | Analyst | Data analyst |
| viewer@marketingplatform.com | Viewer@123456 | Viewer | Read-only user |

### Subscription Plans

1. **Free Plan**
   - Price: $0/month
   - SMS: 100, MMS: 10, Email: 500
   - Contacts: 500
   - Features: Basic campaign management, Basic analytics, Email support

2. **Pro Plan**
   - Price: $49.99/month, $499.99/year
   - SMS: 5,000, MMS: 500, Email: 25,000
   - Contacts: 10,000
   - Features: Advanced campaign management, Workflows & automation, Advanced analytics, Priority support, Custom templates

3. **Enterprise Plan**
   - Price: $199.99/month, $1,999.99/year
   - SMS: 50,000, MMS: 5,000, Email: 250,000
   - Contacts: 100,000
   - Features: Unlimited campaigns, Advanced workflows, Premium analytics, 24/7 support, Dedicated account manager, API access, White-label options

### Landing Page Settings

47 pre-configured settings for the landing page including:
- Hero section (banner/slider configuration)
- Navigation menu (items, colors, positioning)
- Theme colors (primary, secondary, accent)
- Company information
- Statistics display
- Footer content
- SEO settings
- Features section
- Pricing section
- Testimonials
- Contact information

### Page Content

1. **Privacy Policy**
   - Comprehensive privacy policy template
   - Covers data collection, usage, security, retention
   - Includes user rights and contact information

2. **Terms of Service**
   - Complete terms of service template
   - Covers acceptance, licensing, account terms, prohibited uses
   - Includes liability limitations and contact information

## Running Database Initialization

### First Time Setup

1. **Update Connection String**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=MarketingPlatform;Trusted_Connection=true;TrustServerCertificate=true;"
     }
   }
   ```

2. **Run API Project**
   ```bash
   cd src/MarketingPlatform.API
   dotnet run
   ```
   
   The API will automatically:
   - Test database connection
   - Apply all pending migrations
   - Verify table creation
   - Seed all data
   - Log each step

3. **Run Web Project** (optional, after API)
   ```bash
   cd src/MarketingPlatform.Web
   dotnet run
   ```
   
   The Web project will:
   - Test database connection
   - Apply any remaining migrations
   - Seed page content

### Manual Migration

If you need to manually apply migrations:

```bash
cd src/MarketingPlatform.Infrastructure
dotnet ef database update --startup-project ../MarketingPlatform.API
```

### Clean Database Reset

To reset and reseed the database:

```bash
# Drop the database
dotnet ef database drop --startup-project ../MarketingPlatform.API

# Apply migrations and seed
cd ../MarketingPlatform.API
dotnet run
```

## Troubleshooting

### Connection Errors

**Problem**: Cannot connect to database
```
Cannot connect to the database. Please check your connection string...
```

**Solution**:
1. Verify SQL Server is running
2. Check connection string in `appsettings.json`
3. Ensure database firewall rules allow connection
4. Test connection with SQL Server Management Studio

### Migration Errors

**Problem**: Migration fails to apply
```
An error occurred while applying migrations...
```

**Solution**:
1. Check the error logs for specific migration name
2. Verify database schema compatibility
3. Check for conflicting migrations
4. Review migration code for errors
5. Consider dropping and recreating database

### Seeding Errors

**Problem**: Seed data fails to insert
```
An error occurred during database seeding...
```

**Solution**:
1. Check if data already exists (seeds are idempotent)
2. Verify foreign key constraints
3. Check for duplicate key violations
4. Review entity relationships
5. Examine inner exception details in logs

### Duplicate Key Errors

**Problem**: Unique constraint violation
```
Cannot insert duplicate key in object 'dbo.CustomRoles'...
```

**Solution**:
- Seeds are idempotent - they check for existing data
- If error persists, check for manual data modifications
- Verify seed data doesn't have duplicates
- Consider cleaning up test data

## Monitoring and Logs

### Log Levels

- **Information**: Normal operation steps
- **Warning**: Non-critical issues (e.g., data already exists)
- **Error**: Failed operations with recovery
- **Critical**: Fatal errors preventing startup

### Sample Log Output

```
[12:00:00 INF] Starting database initialization...
[12:00:00 INF] Testing database connection...
[12:00:01 INF] Database connection successful.
[12:00:01 INF] Applying database migrations...
[12:00:01 INF] Found 13 pending migrations: 20260117211155_InitialCreate, ...
[12:00:05 INF] All migrations applied successfully.
[12:00:05 INF] Total applied migrations: 13
[12:00:05 INF] Verifying required tables exist...
[12:00:06 INF] Table verification completed.
[12:00:06 INF] Starting data seeding...
[12:00:06 INF] Seeding Identity roles...
[12:00:07 INF] Identity role 'Admin' created successfully.
[12:00:07 INF] Identity role 'User' created successfully.
...
[12:00:10 INF] Database initialization completed successfully.
```

## Best Practices

1. **Always run API first**: API handles full migration and seeding
2. **Check logs**: Review initialization logs for any warnings
3. **Backup before reset**: Always backup before dropping database
4. **Test connection**: Verify SQL Server is accessible
5. **Review errors**: Check inner exceptions for root cause
6. **Idempotent operations**: Seeds can be run multiple times safely

## Security Considerations

1. **Default Credentials**: Change default user passwords in production
2. **Connection Strings**: Use environment variables for sensitive data
3. **SQL Injection**: All queries use parameterized statements
4. **Permissions**: Review role permissions before deployment
5. **Logging**: Sensitive data is not logged

## Additional Resources

- [Entity Framework Core Migrations](https://docs.microsoft.com/ef/core/managing-schemas/migrations/)
- [ASP.NET Core Identity](https://docs.microsoft.com/aspnet/core/security/authentication/identity)
- [Serilog Logging](https://serilog.net/)
- [SQL Server Connection Strings](https://www.connectionstrings.com/sql-server/)

## Support

For issues or questions:
- Check application logs in `Logs/` directory
- Review this documentation
- Contact development team
- Open an issue on GitHub

---

**Last Updated**: January 2024  
**Version**: 1.0
