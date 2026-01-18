# Authentication Flows Documentation

## Overview

The Marketing Platform supports two authentication methods:
1. **JWT-based Authentication (Default)** - Traditional username/password authentication with JWT tokens
2. **OAuth2/SSO Authentication (Optional)** - External identity provider authentication (Azure AD, Google, Okta, AWS Cognito)

**Important**: JWT authentication is the default and always available. OAuth2/SSO providers are optional and must be explicitly configured and enabled.

---

## 1. JWT Authentication Flow (Default)

JWT (JSON Web Token) is the primary authentication method. All user accounts can authenticate using email and password.

### Registration Flow

```
┌─────────┐                 ┌─────────────┐                ┌──────────────┐
│  Client │                 │   API       │                │   Database   │
└────┬────┘                 └──────┬──────┘                └──────┬───────┘
     │                             │                              │
     │  POST /api/auth/register    │                              │
     │  { email, password, ... }   │                              │
     │────────────────────────────>│                              │
     │                             │                              │
     │                             │  Create User                 │
     │                             │─────────────────────────────>│
     │                             │                              │
     │                             │  Assign Default Role         │
     │                             │─────────────────────────────>│
     │                             │                              │
     │                             │  Generate JWT Token          │
     │                             │<─────────────────────────────│
     │                             │                              │
     │  200 OK                     │                              │
     │  { token, refreshToken }    │                              │
     │<────────────────────────────│                              │
     │                             │                              │
```

**Request:**
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecureP@ssw0rd",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+1234567890"
}
```

**Response:**
```json
{
  "userId": "guid",
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64-encoded-token",
  "tokenExpiration": "2024-01-18T20:00:00Z",
  "roles": ["User"]
}
```

### Login Flow

```
┌─────────┐                 ┌─────────────┐                ┌──────────────┐
│  Client │                 │   API       │                │   Database   │
└────┬────┘                 └──────┬──────┘                └──────┬───────┘
     │                             │                              │
     │  POST /api/auth/login       │                              │
     │  { email, password }        │                              │
     │────────────────────────────>│                              │
     │                             │                              │
     │                             │  Validate Credentials        │
     │                             │─────────────────────────────>│
     │                             │<─────────────────────────────│
     │                             │                              │
     │                             │  Get User Roles/Permissions  │
     │                             │─────────────────────────────>│
     │                             │<─────────────────────────────│
     │                             │                              │
     │                             │  Generate JWT Token          │
     │                             │  Generate Refresh Token      │
     │                             │─────────────────────────────>│
     │                             │                              │
     │  200 OK                     │                              │
     │  { token, refreshToken }    │                              │
     │<────────────────────────────│                              │
     │                             │                              │
```

### Token Refresh Flow

```
┌─────────┐                 ┌─────────────┐                ┌──────────────┐
│  Client │                 │   API       │                │   Database   │
└────┬────┘                 └──────┬──────┘                └──────┬───────┘
     │                             │                              │
     │  POST /api/auth/refresh     │                              │
     │  { token, refreshToken }    │                              │
     │────────────────────────────>│                              │
     │                             │                              │
     │                             │  Validate Refresh Token      │
     │                             │─────────────────────────────>│
     │                             │<─────────────────────────────│
     │                             │                              │
     │                             │  Revoke Old Refresh Token    │
     │                             │─────────────────────────────>│
     │                             │                              │
     │                             │  Generate New JWT Token      │
     │                             │  Generate New Refresh Token  │
     │                             │─────────────────────────────>│
     │                             │                              │
     │  200 OK                     │                              │
     │  { token, refreshToken }    │                              │
     │<────────────────────────────│                              │
     │                             │                              │
```

**Token Lifetimes:**
- JWT Access Token: 60 minutes (configurable via `JwtSettings:ExpiryMinutes`)
- Refresh Token: 7 days (configurable via `JwtSettings:RefreshTokenExpiryDays`)

---

## 2. OAuth2/SSO Authentication Flow (Optional)

OAuth2 providers must be configured and enabled by administrators before use. These are **optional enhancements** to the default JWT authentication.

### Supported Providers
- **Azure AD** (Microsoft 365, Azure Active Directory)
- **Google** (Google Workspace, Gmail)
- **Okta** (Enterprise SSO)
- **AWS Cognito** (Amazon identity service)

### OAuth2 Login Flow

```
┌─────────┐    ┌─────────────┐    ┌──────────────┐    ┌─────────────────┐
│ Client  │    │   API       │    │  OAuth2      │    │   Database      │
│         │    │             │    │  Provider    │    │                 │
└────┬────┘    └──────┬──────┘    └──────┬───────┘    └────────┬────────┘
     │                │                   │                     │
     │  GET /oauth2   │                   │                     │
     │  /authorize    │                   │                     │
     │  /{provider}   │                   │                     │
     │───────────────>│                   │                     │
     │                │                   │                     │
     │                │  Get Provider     │                     │
     │                │  Configuration    │                     │
     │                │──────────────────────────────────────────>
     │                │<─────────────────────────────────────────
     │                │                   │                     │
     │  302 Redirect  │                   │                     │
     │  Authorization │                   │                     │
     │  URL           │                   │                     │
     │<───────────────│                   │                     │
     │                │                   │                     │
     │  User Login &  │                   │                     │
     │  Consent       │                   │                     │
     │───────────────────────────────────>│                     │
     │                │                   │                     │
     │                │                   │  User Authenticates │
     │                │                   │  & Grants Consent   │
     │                │                   │                     │
     │  302 Redirect  │                   │                     │
     │  with Code     │                   │                     │
     │<───────────────────────────────────│                     │
     │                │                   │                     │
     │  POST /oauth2  │                   │                     │
     │  /callback     │                   │                     │
     │  /{provider}   │                   │                     │
     │  { code }      │                   │                     │
     │───────────────>│                   │                     │
     │                │                   │                     │
     │                │  Exchange Code    │                     │
     │                │  for Tokens       │                     │
     │                │──────────────────>│                     │
     │                │<──────────────────│                     │
     │                │                   │                     │
     │                │  Get User Info    │                     │
     │                │──────────────────>│                     │
     │                │<──────────────────│                     │
     │                │                   │                     │
     │                │  Find/Create User │                     │
     │                │──────────────────────────────────────────>
     │                │<─────────────────────────────────────────
     │                │                   │                     │
     │                │  Link External    │                     │
     │                │  Account          │                     │
     │                │──────────────────────────────────────────>
     │                │                   │                     │
     │                │  Generate JWT     │                     │
     │                │  Tokens           │                     │
     │                │──────────────────────────────────────────>
     │                │                   │                     │
     │  200 OK        │                   │                     │
     │  { token,      │                   │                     │
     │   refreshToken}│                   │                     │
     │<───────────────│                   │                     │
     │                │                   │                     │
```

### OAuth2 API Endpoints

#### 1. Get Available Providers
```http
GET /api/oauth2/providers
```

**Response:**
```json
[
  {
    "id": 1,
    "name": "AzureAD",
    "displayName": "Microsoft Azure AD",
    "providerType": "OAuth2",
    "isEnabled": true,
    "isDefault": false
  },
  {
    "id": 2,
    "name": "Google",
    "displayName": "Google",
    "providerType": "OAuth2",
    "isEnabled": true,
    "isDefault": false
  }
]
```

#### 2. Get Authorization URL
```http
GET /api/oauth2/authorize/{providerName}?redirectUri=https://yourapp.com/callback
```

**Response:**
```json
{
  "authorizationUrl": "https://login.microsoftonline.com/common/oauth2/v2.0/authorize?client_id=...&state=...",
  "state": "csrf-token-guid"
}
```

#### 3. Handle OAuth2 Callback
```http
POST /api/oauth2/callback/{providerName}
Content-Type: application/json

{
  "code": "authorization-code-from-provider",
  "state": "csrf-token-guid"
}
```

**Response:** Same as JWT login response with JWT tokens

---

## 3. Token Management

### Access Token Structure

JWT tokens contain the following claims:
```json
{
  "nameid": "user-id-guid",
  "email": "user@example.com",
  "name": "John Doe",
  "role": ["User", "Admin"],
  "CustomRole": ["Marketing Manager"],
  "Permissions": "12345",
  "jti": "unique-token-id",
  "exp": 1705608000,
  "iss": "MarketingPlatform.API",
  "aud": "MarketingPlatform.Web"
}
```

### Token Validation

All API requests must include the JWT token:
```http
GET /api/campaigns
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Token Rotation

- Refresh tokens are single-use
- When a refresh token is used, it is revoked and a new pair is issued
- Old JWT tokens become invalid after expiry
- External provider tokens are also rotated and stored securely

---

## 4. Security Features

### Password Requirements
- Minimum 8 characters
- At least one uppercase letter
- At least one lowercase letter
- At least one digit
- At least one special character

### Token Security
- JWT tokens signed with HMAC-SHA256
- Refresh tokens are cryptographically random (64 bytes)
- External provider tokens encrypted at rest
- Tokens transmitted only over HTTPS

### CSRF Protection
- OAuth2 flows use state parameter for CSRF protection
- State tokens are validated on callback

### Account Security
- Accounts can be deactivated (IsActive flag)
- Failed login attempts logged
- Last login timestamp tracked
- Tokens revoked on password change

---

## 5. Configuration

### JWT Settings (appsettings.json)

```json
{
  "JwtSettings": {
    "Secret": "your-super-secret-key-min-32-chars",
    "Issuer": "MarketingPlatform.API",
    "Audience": "MarketingPlatform.Web",
    "ExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7
  }
}
```

### OAuth2 Settings (Optional - All Disabled by Default)

```json
{
  "OAuth2": {
    "Enabled": true,
    "Providers": {
      "AzureAD": {
        "Enabled": false,
        "ClientId": "",
        "ClientSecret": "",
        "Authority": "https://login.microsoftonline.com",
        "TenantId": "common",
        "Scopes": "openid profile email"
      },
      "Google": {
        "Enabled": false,
        "ClientId": "",
        "ClientSecret": "",
        "Scopes": "openid profile email"
      },
      "Okta": {
        "Enabled": false,
        "ClientId": "",
        "ClientSecret": "",
        "Domain": "your-domain.okta.com",
        "Scopes": "openid profile email"
      },
      "Cognito": {
        "Enabled": false,
        "ClientId": "",
        "ClientSecret": "",
        "Domain": "your-domain.auth.region.amazoncognito.com",
        "Region": "us-east-1",
        "UserPoolId": "",
        "Scopes": "openid profile email"
      }
    }
  }
}
```

---

## 6. Provider-Specific Setup

### Azure AD Setup

1. Register application in Azure Portal
2. Configure redirect URI: `https://yourapi.com/signin-oauth`
3. Add API permissions: `User.Read`
4. Create client secret
5. Update appsettings.json with ClientId, ClientSecret, and TenantId
6. Set `Enabled: true` for AzureAD provider

### Google OAuth Setup

1. Create project in Google Cloud Console
2. Enable Google+ API
3. Configure OAuth consent screen
4. Create OAuth 2.0 credentials
5. Add authorized redirect URI
6. Update appsettings.json with ClientId and ClientSecret
7. Set `Enabled: true` for Google provider

### Okta Setup

1. Create Okta developer account
2. Create new application (Web)
3. Configure sign-in redirect URI
4. Note ClientId and ClientSecret
5. Update appsettings.json with credentials and domain
6. Set `Enabled: true` for Okta provider

### AWS Cognito Setup

1. Create Cognito User Pool
2. Configure app client
3. Enable OAuth 2.0 flows
4. Configure callback URLs
5. Note UserPoolId, ClientId, ClientSecret, and domain
6. Update appsettings.json with credentials
7. Set `Enabled: true` for Cognito provider

---

## 7. Admin Management

### Configure Providers (Admin Only)

```http
POST /api/oauth2/admin/providers
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "name": "AzureAD",
  "displayName": "Microsoft Azure AD",
  "providerType": "OAuth2",
  "clientId": "your-client-id",
  "clientSecret": "your-client-secret",
  "authority": "https://login.microsoftonline.com",
  "tenantId": "common",
  "scopes": "openid profile email",
  "isEnabled": true
}
```

### Update Provider
```http
PUT /api/oauth2/admin/providers/{id}
Authorization: Bearer {admin-token}
```

### Delete Provider
```http
DELETE /api/oauth2/admin/providers/{id}
Authorization: Bearer {admin-token}
```

---

## 8. Best Practices

### For Developers

1. **Always use HTTPS** in production
2. **Store secrets securely** - use Azure Key Vault, AWS Secrets Manager, or environment variables
3. **Validate tokens** on every API request
4. **Implement token refresh** before expiry
5. **Handle token expiration** gracefully in UI
6. **Log authentication events** for security auditing
7. **Use JWT as default** - OAuth2 is optional

### For IT Teams

1. **Configure OAuth2 providers** only if needed
2. **Test each provider** in development before production
3. **Monitor authentication logs** for suspicious activity
4. **Rotate client secrets** regularly
5. **Review permissions** granted to OAuth2 apps
6. **Document provider configurations** for team
7. **Keep JWT as fallback** authentication method

### For Users

1. **Use strong passwords** for JWT authentication
2. **Enable 2FA** on external providers (Google, Azure AD, etc.)
3. **Review linked accounts** periodically
4. **Revoke access** from unused external accounts
5. **Report suspicious activity** immediately

---

## 9. Troubleshooting

### JWT Authentication Issues

**Problem**: "Invalid token"
- **Solution**: Check token expiry, ensure proper Bearer format, validate Secret/Issuer/Audience match

**Problem**: "User not found or inactive"
- **Solution**: Verify user exists in database, check IsActive flag

### OAuth2 Issues

**Problem**: "Provider not configured"
- **Solution**: Ensure provider exists in database and is enabled

**Problem**: "Invalid authorization code"
- **Solution**: Check redirect URI matches exactly, ensure code is not expired

**Problem**: "Token exchange failed"
- **Solution**: Verify ClientId and ClientSecret are correct, check provider endpoints

---

## 10. Migration Guide

### Existing Users to OAuth2

Users with existing JWT accounts can link external providers:

1. User logs in with JWT
2. User navigates to account settings
3. User clicks "Link [Provider] Account"
4. OAuth2 flow initiated
5. External account linked to existing user

Both authentication methods work simultaneously.

---

## Summary

- **JWT Authentication**: Default, always available, username/password based
- **OAuth2/SSO**: Optional, requires admin configuration, enterprise identity providers
- **Security**: Industry-standard protocols, secure token handling, comprehensive auditing
- **Flexibility**: Support for multiple providers, easy to extend
- **Integration**: Works with existing user management and RBAC systems

For additional support, refer to API documentation at `/swagger` or contact your system administrator.
