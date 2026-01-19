# Global Configuration Management Implementation

## Overview
This document describes the implementation of platform-wide global configuration management features including platform settings, feature toggles, and compliance rules.

## Features Implemented

### 12.2.1 Platform-wide Settings

Platform settings provide a centralized configuration store for managing platform-wide parameters that affect all subsystems.

#### Key Features:
- **Centralized Configuration**: Store all platform settings in one location
- **Data Type Support**: String, Integer, Boolean, Decimal, and JSON data types
- **Scope Management**: Global, Tenant, and User-level settings
- **Encryption Support**: Sensitive settings can be encrypted
- **Read-Only Settings**: Critical settings can be marked as read-only
- **Category Organization**: Settings can be grouped by category

#### Entities:
- **PlatformSetting**: Main entity storing setting key-value pairs with metadata

#### API Endpoints:

##### Get Settings (Paginated)
```http
GET /api/platformsettings?pageNumber=1&pageSize=20&searchTerm=email
Authorization: Bearer {admin_token}
```

##### Get Setting by ID
```http
GET /api/platformsettings/{id}
Authorization: Bearer {admin_token}
```

##### Get Setting by Key
```http
GET /api/platformsettings/key/{key}
Authorization: Bearer {admin_token}
```

##### Get Settings by Category
```http
GET /api/platformsettings/category/{category}
Authorization: Bearer {admin_token}
```

##### Create Setting
```http
POST /api/platformsettings
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "key": "SMTP_HOST",
  "value": "smtp.example.com",
  "dataType": 0,
  "scope": 0,
  "description": "SMTP server hostname",
  "category": "Email",
  "isEncrypted": false,
  "isReadOnly": false,
  "defaultValue": "localhost"
}
```

##### Update Setting
```http
PUT /api/platformsettings/{id}
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "value": "smtp.newhost.com",
  "description": "Updated SMTP server hostname",
  "category": "Email"
}
```

##### Delete Setting
```http
DELETE /api/platformsettings/{id}
Authorization: Bearer {admin_token}
```

### 12.2.2 Feature Toggles

Feature toggles enable dynamic enabling/disabling of features without requiring deployments.

#### Key Features:
- **Dynamic Feature Control**: Enable/disable features at runtime
- **Role-Based Access**: Enable features for specific roles
- **User-Based Access**: Enable features for specific users
- **Time-Based Control**: Automatically enable/disable features based on time
- **Category Organization**: Group related features together
- **Status Management**: Enabled, Disabled, EnabledForRoles, EnabledForUsers

#### Entities:
- **FeatureToggle**: Main entity managing feature states and access control

#### API Endpoints:

##### Get Feature Toggles (Paginated)
```http
GET /api/featuretoggles?pageNumber=1&pageSize=20
Authorization: Bearer {admin_token}
```

##### Get Feature Toggle by ID
```http
GET /api/featuretoggles/{id}
Authorization: Bearer {admin_token}
```

##### Get Feature Toggle by Name
```http
GET /api/featuretoggles/name/{name}
Authorization: Bearer {token}
```

##### Check if Feature is Enabled (Global)
```http
GET /api/featuretoggles/{name}/enabled
Authorization: Bearer {token}
```

##### Check if Feature is Enabled for Current User
```http
GET /api/featuretoggles/{name}/enabled/me
Authorization: Bearer {token}
```

##### Check if Feature is Enabled for Role
```http
GET /api/featuretoggles/{name}/enabled/role/{roleName}
Authorization: Bearer {token}
```

##### Create Feature Toggle
```http
POST /api/featuretoggles
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "name": "NEW_DASHBOARD",
  "displayName": "New Dashboard UI",
  "description": "Enable new dashboard interface",
  "isEnabled": false,
  "category": "UI",
  "enableAfter": "2026-02-01T00:00:00Z",
  "disableAfter": null
}
```

##### Update Feature Toggle
```http
PUT /api/featuretoggles/{id}
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "displayName": "New Dashboard UI v2",
  "description": "Updated dashboard with new features",
  "isEnabled": true,
  "category": "UI"
}
```

##### Toggle Feature Status
```http
POST /api/featuretoggles/{id}/toggle
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "isEnabled": true,
  "enabledForRoles": "Admin,Manager",
  "enabledForUsers": ""
}
```

##### Delete Feature Toggle
```http
DELETE /api/featuretoggles/{id}
Authorization: Bearer {admin_token}
```

### 12.2.3 Global Compliance Rules

Compliance rules enforce regulatory and business requirements across all services with complete audit trail.

#### Key Features:
- **Comprehensive Rule Management**: Define and manage compliance rules
- **Complete Audit Trail**: Track all changes to compliance rules
- **Auto-Apply**: Rules automatically apply to all workflows
- **Priority System**: Higher priority rules take precedence
- **Time-Based Activation**: Rules activate/deactivate based on effective dates
- **Regional Scope**: Apply rules to specific regions
- **Service Scope**: Apply rules to specific services
- **Rule Types**: DataRetention, ConsentManagement, MessageContent, RateLimiting, OptOutEnforcement, RegionalCompliance, Custom

#### Entities:
- **ComplianceRule**: Main entity defining compliance rules
- **ComplianceRuleAudit**: Audit trail for all rule changes

#### API Endpoints:

##### Get Compliance Rules (Paginated)
```http
GET /api/compliancerules?pageNumber=1&pageSize=20
Authorization: Bearer {admin_token}
```

##### Get Compliance Rule by ID
```http
GET /api/compliancerules/{id}
Authorization: Bearer {admin_token}
```

##### Get Active Compliance Rules
```http
GET /api/compliancerules/active
Authorization: Bearer {token}
```

##### Get Compliance Rules by Type
```http
GET /api/compliancerules/type/{ruleType}
Authorization: Bearer {admin_token}
```

##### Get Applicable Rules
```http
GET /api/compliancerules/applicable?region=US&service=SMS
Authorization: Bearer {token}
```

##### Get Audit Trail
```http
GET /api/compliancerules/{id}/audit
Authorization: Bearer {admin_token}
```

##### Create Compliance Rule
```http
POST /api/compliancerules
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "name": "GDPR_DATA_RETENTION",
  "description": "EU GDPR data retention policy - 30 days",
  "ruleType": 0,
  "configuration": "{\"retentionDays\": 30, \"autoDelete\": true}",
  "priority": 100,
  "isMandatory": true,
  "effectiveFrom": "2026-01-01T00:00:00Z",
  "effectiveTo": null,
  "applicableRegions": "EU,UK",
  "applicableServices": "SMS,Email"
}
```

##### Update Compliance Rule
```http
PUT /api/compliancerules/{id}
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "name": "GDPR_DATA_RETENTION_UPDATED",
  "description": "Updated EU GDPR policy - 60 days",
  "configuration": "{\"retentionDays\": 60, \"autoDelete\": true}",
  "priority": 100,
  "reason": "Policy update per legal requirements"
}
```

##### Activate Compliance Rule
```http
POST /api/compliancerules/{id}/activate
Authorization: Bearer {admin_token}
```

##### Deactivate Compliance Rule
```http
POST /api/compliancerules/{id}/deactivate
Authorization: Bearer {admin_token}
Content-Type: application/json

"Rule temporarily suspended for review"
```

##### Delete Compliance Rule
```http
DELETE /api/compliancerules/{id}?reason=Rule no longer applicable
Authorization: Bearer {admin_token}
```

## Data Models

### Platform Setting Enums

```csharp
public enum SettingScope
{
    Global = 0,
    Tenant = 1,
    User = 2
}

public enum SettingDataType
{
    String = 0,
    Integer = 1,
    Boolean = 2,
    Decimal = 3,
    Json = 4
}
```

### Feature Toggle Enums

```csharp
public enum FeatureToggleStatus
{
    Disabled = 0,
    Enabled = 1,
    EnabledForRoles = 2,
    EnabledForUsers = 3
}
```

### Compliance Rule Enums

```csharp
public enum ComplianceRuleType
{
    DataRetention = 0,
    ConsentManagement = 1,
    MessageContent = 2,
    RateLimiting = 3,
    OptOutEnforcement = 4,
    RegionalCompliance = 5,
    Custom = 99
}

public enum ComplianceRuleStatus
{
    Draft = 0,
    Active = 1,
    Inactive = 2,
    Archived = 3
}

public enum ComplianceAuditAction
{
    Created = 0,
    Updated = 1,
    Activated = 2,
    Deactivated = 3,
    Deleted = 4
}
```

## Usage Examples

### Platform Settings Service Usage

```csharp
// Get a setting value with type conversion
var smtpHost = await _platformSettingService.GetSettingValueAsync<string>("SMTP_HOST", "localhost");
var maxRetries = await _platformSettingService.GetSettingValueAsync<int>("MAX_RETRIES", 3);
var enableLogging = await _platformSettingService.GetSettingValueAsync<bool>("ENABLE_LOGGING", true);

// Set a setting value
await _platformSettingService.SetSettingValueAsync("SMTP_HOST", "smtp.newhost.com", userId);
```

### Feature Toggle Service Usage

```csharp
// Check if feature is enabled globally
if (await _featureToggleService.IsFeatureEnabledAsync("NEW_DASHBOARD"))
{
    // Show new dashboard
}

// Check if feature is enabled for current user
if (await _featureToggleService.IsFeatureEnabledForUserAsync("BETA_FEATURES", userId))
{
    // Show beta features
}

// Check if feature is enabled for role
if (await _featureToggleService.IsFeatureEnabledForRoleAsync("ADVANCED_ANALYTICS", "Admin"))
{
    // Show advanced analytics
}
```

### Compliance Rule Service Usage

```csharp
// Get all active compliance rules
var activeRules = await _complianceRuleService.GetActiveComplianceRulesAsync();

// Get applicable rules for a region and service
var applicableRules = await _complianceRuleService.GetApplicableRulesAsync("EU", "SMS");

// Process rules based on priority
foreach (var rule in applicableRules.OrderByDescending(r => r.Priority))
{
    // Apply rule logic
}
```

## Security Considerations

1. **Admin-Only Access**: Most management endpoints require Admin role
2. **IP Address Tracking**: Compliance rule changes log IP addresses
3. **Audit Trail**: All compliance rule modifications are tracked
4. **Encryption Support**: Sensitive platform settings can be encrypted
5. **Read-Only Protection**: Critical settings cannot be modified or deleted

## Database Schema

The migration `20260118195158_AddGlobalConfigurationManagement` creates:
- `PlatformSettings` table with indexes on Key, Category, and Scope
- `FeatureToggles` table with indexes on Name, Category, and Status
- `ComplianceRules` table with indexes on Type, Status, Priority
- `ComplianceRuleAudits` table with foreign key to ComplianceRules

## Testing

All three services include comprehensive business logic for:
- CRUD operations
- Authorization checks
- Data validation
- Audit trail creation
- Time-based activation/deactivation
- Scope-based filtering

Unit tests should cover:
- Service methods
- Controller endpoints
- Validation logic
- Authorization policies
- Audit trail creation

## Future Enhancements

1. **Platform Settings**:
   - Setting versioning
   - Setting templates
   - Bulk import/export

2. **Feature Toggles**:
   - Percentage-based rollout
   - A/B testing integration
   - Feature dependencies

3. **Compliance Rules**:
   - Rule chaining
   - Rule templates
   - Automated compliance reports
   - Rule validation engine

## Summary

This implementation provides a complete global configuration management system with:
- ✅ Platform-wide settings with centralized storage
- ✅ Feature toggles with role-based access control
- ✅ Compliance rules with comprehensive audit trail
- ✅ Repository and Service pattern implementation
- ✅ Complete API endpoints with proper authorization
- ✅ Database migrations and entity configurations
- ✅ AutoMapper profiles for DTOs
- ✅ Dependency injection registration
