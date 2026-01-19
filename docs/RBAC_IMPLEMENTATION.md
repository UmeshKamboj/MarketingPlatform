# Role-Based Access Control (RBAC) Implementation

## Overview

This document describes the implementation of a role-based access control (RBAC) system for the Marketing Platform. The system provides fine-grained permission control with predefined roles (SuperAdmin, Admin, Manager, Analyst, Viewer) and supports custom role creation.

## Features

- **Predefined Roles**: SuperAdmin, Admin, Manager, Analyst, Viewer
- **Fine-Grained Permissions**: 30+ permissions across different resource types
- **Permission-Based Authorization**: Attribute-based authorization for controllers and actions
- **JWT Integration**: Permissions included in JWT tokens for stateless authorization
- **User Activity Logging**: All role assignments and changes are logged with audit trail
- **Menu Visibility**: UI elements are automatically shown/hidden based on user permissions
- **Repository Pattern**: Clean architecture with repository and service layers

## Architecture

### Core Components

#### 1. Enums

**Permission** (`MarketingPlatform.Core.Enums.Permission`)
- Flags enum with 30+ permissions
- Categories: Campaigns, Contacts, Templates, Analytics, Users, Roles, Settings, Compliance, Workflows, Audit Logs
- Example permissions: `ViewCampaigns`, `CreateCampaigns`, `ManageRoles`, etc.

**UserRole** (`MarketingPlatform.Core.Enums.UserRole`)
- Enum defining standard roles: SuperAdmin, Admin, Manager, Analyst, Viewer

#### 2. Entities

**Role** (`MarketingPlatform.Core.Entities.Role`)
- `Id`: Primary key
- `Name`: Role name (unique)
- `Description`: Role description
- `Permissions`: Long integer storing permission flags
- `IsSystemRole`: Indicates if role is system-defined
- `IsActive`: Role status
- Helper methods: `HasPermission()`, `AddPermission()`, `RemovePermission()`

**UserRole** (`MarketingPlatform.Core.Entities.UserRole`)
- Junction table linking users to roles
- `UserId`: Foreign key to ApplicationUser
- `RoleId`: Foreign key to Role
- `AssignedAt`: Timestamp of assignment
- `AssignedBy`: UserId of assigner

#### 3. Repositories

**IRoleRepository** / **RoleRepository**
- CRUD operations for roles
- `GetByIdAsync()`, `GetByNameAsync()`, `GetAllAsync()`, `GetActiveRolesAsync()`
- `CreateAsync()`, `UpdateAsync()`, `DeleteAsync()`

**IUserRoleRepository** / **UserRoleRepository**
- User-role assignment operations
- `GetUserRolesAsync()`, `GetUsersInRoleAsync()`
- `AssignRoleToUserAsync()`, `RemoveRoleFromUserAsync()`
- `GetUserPermissionsAsync()`: Combines all permissions from user's roles

#### 4. Services

**IRoleService** / **RoleService**
- Business logic for role management
- Role CRUD with validation
- User-role assignment with audit logging
- Permission checking: `UserHasPermissionAsync()`
- Permission aggregation: `GetUserPermissionsAsync()`

#### 5. Authorization

**RequirePermissionAttribute**
- Custom authorization attribute for controllers/actions
- Usage: `[RequirePermission(Permission.ViewCampaigns)]`

**PermissionAuthorizationHandler**
- Authorization handler that checks user permissions from JWT claims
- SuperAdmin bypasses all permission checks
- Validates permission flags using bitwise operations

#### 6. Web UI

**PermissionTagHelper**
- Razor Tag Helper for conditional rendering
- Usage: `<li require-permission="ViewCampaigns">...</li>`
- Automatically hides elements if user lacks permission

## Default Roles and Permissions

### SuperAdmin
- **Description**: Full system access with all permissions
- **Permissions**: All (Permission.All)
- **Use Case**: System administrators, platform owners

### Admin
- **Description**: Administrator with most permissions except user/role management
- **Permissions**: 
  - All Campaign permissions (View, Create, Edit, Delete)
  - All Contact permissions
  - All Template permissions
  - All Analytics permissions (including Export)
  - All Workflow permissions
  - Settings management
  - Compliance management
  - View audit logs
- **Use Case**: Department managers, team leads

### Manager
- **Description**: Campaign and contact management with analytics access
- **Permissions**:
  - Campaigns (View, Create, Edit)
  - Contacts (View, Create, Edit)
  - Templates (View, Create, Edit)
  - Analytics (View, Detailed)
  - Workflows (View, Create, Edit)
  - Compliance (View only)
- **Use Case**: Marketing managers, campaign managers

### Analyst
- **Description**: Read access with detailed analytics capabilities
- **Permissions**:
  - Campaigns (View only)
  - Contacts (View only)
  - Templates (View only)
  - Analytics (View, Detailed, Export)
  - Workflows (View only)
  - Compliance (View only)
- **Use Case**: Data analysts, reporting staff

### Viewer
- **Description**: Read-only access to campaigns and basic analytics
- **Permissions**:
  - Campaigns (View only)
  - Contacts (View only)
  - Templates (View only)
  - Analytics (View basic)
- **Use Case**: Stakeholders, observers

## API Endpoints

### Role Management

```
GET    /api/roles                          - Get all roles
GET    /api/roles/active                   - Get active roles
GET    /api/roles/{id}                     - Get role by ID
GET    /api/roles/name/{name}              - Get role by name
POST   /api/roles                          - Create new role
PUT    /api/roles/{id}                     - Update role
DELETE /api/roles/{id}                     - Delete role
GET    /api/roles/{id}/users               - Get users in role
POST   /api/roles/assign                   - Assign role to user
DELETE /api/roles/remove                   - Remove role from user
GET    /api/roles/permissions              - Get all available permissions
GET    /api/roles/user/{userId}/permissions - Get user's permissions
```

### Required Permissions

- View role data: `Permission.ViewRoles`
- Create/Update/Delete roles: `Permission.ManageRoles`
- Assign/Remove roles: `Permission.ManageRoles`

## Usage Examples

### 1. Protect API Endpoint

```csharp
[HttpGet]
[RequirePermission(Permission.ViewCampaigns)]
public async Task<ActionResult<IEnumerable<CampaignDto>>> GetCampaigns()
{
    // Only users with ViewCampaigns permission can access
    var campaigns = await _campaignService.GetAllAsync();
    return Ok(campaigns);
}
```

### 2. Check Multiple Permissions

```csharp
[HttpPost]
[RequirePermission(Permission.CreateCampaigns)]
[RequirePermission(Permission.ViewContacts)]
public async Task<ActionResult<CampaignDto>> CreateCampaign(CreateCampaignDto dto)
{
    // User needs both permissions
    var campaign = await _campaignService.CreateAsync(dto);
    return CreatedAtAction(nameof(GetCampaign), new { id = campaign.Id }, campaign);
}
```

### 3. Conditional UI Rendering

```html
<!-- Only show for users with ViewCampaigns permission -->
<li class="nav-item" require-permission="ViewCampaigns">
    <a class="nav-link" asp-controller="Campaigns" asp-action="Index">Campaigns</a>
</li>

<!-- Only show for users with ManageRoles permission -->
<div require-permission="ManageRoles">
    <button class="btn btn-primary">Manage Roles</button>
</div>
```

### 4. Create Custom Role via API

```csharp
POST /api/roles
Content-Type: application/json

{
    "name": "ContentEditor",
    "description": "Can manage campaigns and templates",
    "permissions": [
        "ViewCampaigns",
        "CreateCampaigns",
        "EditCampaigns",
        "ViewTemplates",
        "CreateTemplates",
        "EditTemplates"
    ]
}
```

### 5. Assign Role to User

```csharp
POST /api/roles/assign
Content-Type: application/json

{
    "userId": "user-guid-here",
    "roleId": 3
}
```

### 6. Check User Permission in Code

```csharp
var hasPermission = await _roleService.UserHasPermissionAsync(userId, Permission.EditCampaigns);
if (hasPermission)
{
    // Allow edit
}
```

## JWT Token Structure

When a user logs in, their JWT token includes:

```json
{
  "nameid": "user-id",
  "email": "user@example.com",
  "name": "John Doe",
  "Permissions": "1073741823",  // Combined permission flags
  "role": ["SuperAdmin"],        // Identity roles
  "CustomRole": ["SuperAdmin"],  // Custom roles
  "jti": "unique-token-id",
  "exp": 1234567890
}
```

The `Permissions` claim contains a long integer with all user's permissions combined using bitwise OR.

## Database Schema

### CustomRoles Table
- `Id` (int, PK)
- `Name` (nvarchar(50), unique)
- `Description` (nvarchar(500))
- `Permissions` (bigint)
- `IsSystemRole` (bit)
- `IsActive` (bit)
- `CreatedAt` (datetime2)
- `UpdatedAt` (datetime2, nullable)

### CustomUserRoles Table
- `UserId` (nvarchar(450), FK to AspNetUsers)
- `RoleId` (int, FK to CustomRoles)
- `AssignedAt` (datetime2)
- `AssignedBy` (nvarchar(450), nullable)
- Composite PK on (UserId, RoleId)

## Security Considerations

1. **SuperAdmin Protection**: SuperAdmin role bypasses all permission checks. Assign carefully.
2. **System Roles**: Roles with `IsSystemRole = true` cannot be modified or deleted.
3. **Audit Trail**: All role assignments include `AssignedBy` and `AssignedAt` for accountability.
4. **Token Expiry**: Permissions are cached in JWT. Changes take effect after token refresh.
5. **Role Deletion**: Roles with assigned users cannot be deleted.
6. **Permission Flags**: Using bitwise operations ensures efficient permission checking.

## Migration

A migration named `AddRBACSystem` has been created. Apply it using:

```bash
dotnet ef database update --project src/MarketingPlatform.Infrastructure --startup-project src/MarketingPlatform.API
```

## Testing

### Default Admin Account

- **Email**: admin@marketingplatform.com
- **Password**: Admin@123456
- **Role**: SuperAdmin (all permissions)

This account is automatically created during database seeding.

## Future Enhancements

1. **Dynamic Permissions**: Allow defining custom permissions at runtime
2. **Permission Groups**: Group related permissions for easier management
3. **Role Hierarchies**: Support role inheritance
4. **Time-Based Permissions**: Temporary permission grants
5. **Resource-Level Permissions**: Permission checks on specific resources (e.g., only own campaigns)
6. **Permission Delegation**: Allow users to delegate specific permissions temporarily
7. **Multi-Tenant Support**: Tenant-specific roles and permissions

## Troubleshooting

### User doesn't see menu items after role assignment
- **Cause**: JWT token contains old permissions
- **Solution**: User needs to log out and log back in to refresh token

### Permission check always fails
- **Cause**: Permission name mismatch or not in enum
- **Solution**: Verify permission exists in `Permission` enum and spelling is correct

### Cannot delete role
- **Cause**: Role is system role or has assigned users
- **Solution**: Remove all users from role first, or don't delete system roles

## Conclusion

This RBAC implementation provides a flexible, secure, and scalable authorization system for the Marketing Platform. It follows clean architecture principles with clear separation of concerns and supports both API and Web UI scenarios.
