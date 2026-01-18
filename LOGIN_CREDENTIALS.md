# Login Credentials for Marketing Platform

This document contains the default login credentials for testing and development purposes. **These credentials should be changed in production environments.**

## Default User Accounts

### Super Admin Account
**Purpose:** Full system access with all permissions, including platform configuration and user management.

- **Email:** `admin@marketingplatform.com`
- **Password:** `Admin@123456`
- **Role:** SuperAdmin
- **Permissions:** All permissions (manage users, roles, subscriptions, platform settings, view audit logs, etc.)

### Manager Account
**Purpose:** Campaign and contact management with analytics access.

- **Email:** `manager@marketingplatform.com`
- **Password:** `Manager@123456`
- **Role:** Manager
- **Permissions:**
  - View, create, edit campaigns
  - View, create, edit contacts
  - View, create, edit templates
  - View analytics and detailed analytics
  - View, create, edit workflows
  - View compliance

### Analyst Account
**Purpose:** Read access with detailed analytics capabilities.

- **Email:** `analyst@marketingplatform.com`
- **Password:** `Analyst@123456`
- **Role:** Analyst
- **Permissions:**
  - View campaigns
  - View contacts
  - View templates
  - View analytics and detailed analytics
  - Export analytics
  - View workflows
  - View compliance

### Regular User Account
**Purpose:** Basic user with limited permissions.

- **Email:** `user@marketingplatform.com`
- **Password:** `User@123456`
- **Role:** User
- **Permissions:**
  - View campaigns
  - View contacts
  - View templates
  - View basic analytics

### Viewer Account
**Purpose:** Read-only access to campaigns and basic analytics.

- **Email:** `viewer@marketingplatform.com`
- **Password:** `Viewer@123456`
- **Role:** Viewer
- **Permissions:**
  - View campaigns
  - View contacts
  - View templates
  - View basic analytics

## Subscription Plans

The following subscription plans are seeded in the database and are available on the landing page (ShowOnLanding = true):

### Free Plan
- **Price:** $0/month
- **SMS Limit:** 100/month
- **MMS Limit:** 10/month
- **Email Limit:** 500/month
- **Contact Limit:** 500
- **Features:**
  - Basic campaign management
  - Basic analytics
  - Email support

### Pro Plan
- **Price:** $49.99/month or $499.99/year (Save 20%)
- **SMS Limit:** 5,000/month
- **MMS Limit:** 500/month
- **Email Limit:** 25,000/month
- **Contact Limit:** 10,000
- **Features:**
  - Advanced campaign management
  - Workflows & automation
  - Advanced analytics
  - Priority support
  - Custom templates

### Enterprise Plan
- **Price:** $199.99/month or $1,999.99/year (Save 20%)
- **SMS Limit:** 50,000/month
- **MMS Limit:** 5,000/month
- **Email Limit:** 250,000/month
- **Contact Limit:** 100,000
- **Features:**
  - Unlimited campaigns
  - Advanced workflows
  - Premium analytics
  - 24/7 support
  - Dedicated account manager
  - API access
  - White-label options

## How to Access

### Web Application
1. Navigate to: `https://localhost:7002`
2. Click on "Login" or navigate to `/Auth/Login`
3. Enter the email and password from above
4. You will be redirected to the dashboard

### API Access
1. Base URL: `https://localhost:7001/api`
2. Swagger Documentation: `https://localhost:7001/swagger`
3. Authenticate using POST `/api/auth/login` with credentials
4. Use the returned JWT token in Authorization header: `Bearer {token}`

## First-Time Setup

1. **Run Migrations:**
   ```bash
   cd src/MarketingPlatform.API
   dotnet run
   ```
   The application will automatically apply migrations and seed the database on first run.

2. **Verify Seed Data:**
   - Login with the Super Admin account
   - Navigate to Users Management to see all seeded users
   - Navigate to Subscription Plans to see available plans
   - Check the landing page to see subscription plans displayed

3. **Test Functionality:**
   - Create a campaign as Manager
   - View analytics as Analyst
   - Subscribe to a plan as User
   - View landing page to see available subscription plans

## Security Notes

⚠️ **IMPORTANT:** These are default credentials for development and testing only.

- Change all default passwords before deploying to production
- Enable two-factor authentication for admin accounts
- Implement password policies (complexity, expiration, history)
- Monitor login attempts and implement rate limiting
- Use environment variables for sensitive configuration
- Enable audit logging for all privileged actions

## Password Policy

Default password requirements:
- Minimum 8 characters
- At least one uppercase letter
- At least one lowercase letter
- At least one digit
- At least one special character (@, #, $, etc.)

## Role Hierarchy

```
SuperAdmin (Highest)
    ├── Admin
    ├── Manager
    ├── Analyst
    ├── Viewer
    └── User (Lowest)
```

## API Endpoints by Role

### SuperAdmin Only
- `/api/users` (full CRUD)
- `/api/roles` (full CRUD)
- `/api/subscriptionplans` (full CRUD)
- `/api/superadmin/*` (all super admin endpoints)
- `/api/platformsettings/*` (platform configuration)

### Admin
- `/api/campaigns/*` (full CRUD)
- `/api/contacts/*` (full CRUD)
- `/api/templates/*` (full CRUD)
- `/api/analytics/*` (all analytics)

### Manager
- `/api/campaigns/*` (create, read, update)
- `/api/contacts/*` (create, read, update)
- `/api/templates/*` (create, read, update)
- `/api/analytics/*` (view and export)

### Analyst
- `/api/campaigns` (read only)
- `/api/contacts` (read only)
- `/api/analytics/*` (view and export)

### Viewer & User
- `/api/campaigns` (read only)
- `/api/contacts` (read only)
- `/api/analytics` (basic view only)

## Support

For questions or issues:
- Email: support@marketingplatform.com
- Documentation: See README.md and other documentation files
- API Documentation: Available at `/swagger` endpoint

## Changelog

- **2026-01-18:** Initial seed data with 5 user accounts and 3 subscription plans
- **2026-01-18:** Added ShowOnLanding flag to subscription plans
- **2026-01-18:** Integrated subscription plans with landing page display
