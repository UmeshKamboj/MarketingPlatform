# Server-Side Authentication & HttpClient Service Layer Implementation

## Overview

This document describes the implementation of server-side authentication with cookie-based sessions and a comprehensive HttpClient service layer for API communication in the Marketing Platform Web application.

## Problem Fixed

### Original Issue
**Problem:** After login, users were redirected back to the login page (`/auth/login?ReturnUrl=%2Fusers%2Fdashboard`) instead of accessing protected pages.

**Root Cause:**
1. JavaScript stored JWT tokens in `localStorage` after successful API authentication
2. ASP.NET Core's `[Authorize]` attribute checked for authentication cookies, not localStorage
3. No authentication cookies were being set, causing authorization to fail
4. The web application relied entirely on client-side JavaScript for authentication

## Solution Implemented

### Architecture Changes

```
OLD FLOW (Client-Side Only):
Browser → JavaScript → API (JWT Auth) → localStorage
           ↓
        Web Controller [Authorize] → ❌ No Cookie → Redirect to Login

NEW FLOW (Server-Side with Cookies):
Browser → Web Controller → AuthenticationService → API (JWT Auth)
           ↓                      ↓
        Cookie Created      Token Stored in Cookie
           ↓
        Web Controller [Authorize] → ✅ Cookie Exists → Access Granted
```

## Components Added

### 1. API Client Infrastructure (`src/MarketingPlatform.Web/Services/`)

#### **IApiClient.cs**
Generic HTTP client interface for API communication.

```csharp
public interface IApiClient
{
    Task<TResponse?> GetAsync<TResponse>(string endpoint, ...);
    Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, ...);
    Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data, ...);
    Task<TResponse?> DeleteAsync<TResponse>(string endpoint, ...);
    void SetAuthorizationToken(string token);
    void ClearAuthorizationToken();
}
```

**Features:**
- Generic methods for all HTTP verbs
- Automatic JSON serialization/deserialization
- Error handling and logging
- Bearer token management
- Configurable base URL from appsettings.json

#### **ApiClient.cs**
Implementation of the HTTP client with:
- Automatic retry logic consideration
- Structured error responses
- JSON options configuration
- Request/response logging

### 2. Authentication Services

#### **IAuthenticationService.cs & AuthenticationService.cs**

Handles all authentication operations:

**Key Methods:**
- `LoginAsync(LoginRequestDto)` - Authenticate user and create cookie session
- `RegisterAsync(RegisterRequestDto)` - Register new user and create session
- `RefreshTokenAsync(RefreshTokenRequestDto)` - Refresh authentication token
- `LogoutAsync(userId)` - Clear session and revoke tokens
- `ChangePasswordAsync(userId, ChangePasswordDto)` - Change user password

**Cookie Creation:**
- Stores JWT token in authentication cookie claims
- Includes user ID, email, name, roles
- Configurable expiration (default: 1 hour)
- Sliding expiration support
- Secure and HttpOnly flags

### 3. Campaign API Service

#### **ICampaignApiService.cs & CampaignApiService.cs**

Server-side service for campaign operations:

**Key Methods:**
- `GetCampaignsAsync(PagedRequest)` - Get paginated campaigns
- `GetCampaignByIdAsync(id)` - Get specific campaign
- `GetCampaignsByStatusAsync(status)` - Filter by status
- `CreateCampaignAsync(CreateCampaignDto)` - Create new campaign
- `UpdateCampaignAsync(id, UpdateCampaignDto)` - Update campaign
- `DeleteCampaignAsync(id)` - Delete campaign
- `StartCampaignAsync(id)` - Start campaign
- `PauseCampaignAsync(id)` - Pause campaign
- `ResumeCampaignAsync(id)` - Resume campaign
- `ArchiveCampaignAsync(id)` - Archive campaign

**Authorization:**
- Automatically retrieves JWT token from user's cookie
- Sets Authorization header for API requests
- Handles 401 Unauthorized responses

## Configuration Changes

### Program.cs Updates

#### 1. Cookie Authentication Setup

```csharp
using Microsoft.AspNetCore.Authentication.Cookies;
using MarketingPlatform.Web.Services;

// Configure Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "MarketingPlatform.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
        options.LoginPath = "/auth/login";
        options.LogoutPath = "/auth/logout";
        options.AccessDeniedPath = "/auth/access-denied";
    });
```

#### 2. Service Registration

```csharp
// HTTP Client Services (Server-side API communication)
builder.Services.AddHttpClient<IApiClient, ApiClient>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ICampaignApiService, CampaignApiService>();
```

### AuthController Changes

#### Updated Actions

**GET `/auth/login`** - Display login page
```csharp
[Route("login")]
public IActionResult Login()
{
    ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
    ViewBag.RecaptchaSiteKey = _configuration["ReCaptcha:SiteKey"];
    return View();
}
```

**POST `/auth/login`** - Server-side login processing
```csharp
[HttpPost("login")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> LoginPost([FromForm] LoginRequestDto model, string? returnUrl = null)
{
    // Validates credentials via API
    // Creates authentication cookie
    // Redirects to returnUrl or dashboard
}
```

**POST `/auth/register`** - Server-side registration
```csharp
[HttpPost("register")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> RegisterPost([FromForm] RegisterRequestDto model)
{
    // Registers user via API
    // Creates authentication cookie
    // Redirects to dashboard
}
```

**GET `/auth/logout`** - Logout action
```csharp
[Authorize]
[Route("logout")]
public async Task<IActionResult> Logout()
{
    // Calls API logout
    // Clears authentication cookie
    // Redirects to login page
}
```

**GET `/auth/access-denied`** - Access denied page
```csharp
[Route("access-denied")]
public IActionResult AccessDenied()
{
    return View();
}
```

### View Updates

#### Login.cshtml Changes

1. **Added Model Binding:**
```cshtml
@model MarketingPlatform.Application.DTOs.Auth.LoginRequestDto
```

2. **Server-Side Form:**
```cshtml
<form asp-action="LoginPost" asp-controller="Auth" method="post">
    @Html.AntiForgeryToken()
    <!-- Form fields with asp-for bindings -->
</form>
```

3. **Error Display:**
```cshtml
@if (ViewBag.ErrorMessage != null)
{
    <div class="alert alert-danger alert-dismissible fade show">
        @ViewBag.ErrorMessage
    </div>
}
```

4. **Field Validation:**
```cshtml
<input asp-for="Email" ... />
<span asp-validation-for="Email" class="text-danger small"></span>
```

## Authentication Flow

### Login Flow (Server-Side)

```
1. User submits login form
   ↓
2. POST to /auth/login (server-side)
   ↓
3. AuthController.LoginPost()
   ↓
4. AuthenticationService.LoginAsync()
   ↓
5. ApiClient.PostAsync() → API /api/auth/login
   ↓
6. API validates credentials, returns JWT token
   ↓
7. AuthenticationService creates authentication cookie with:
   - User ID (ClaimTypes.NameIdentifier)
   - Email (ClaimTypes.Email)
   - Name (ClaimTypes.Name)
   - Roles (ClaimTypes.Role)
   - JWT token (access_token)
   - Refresh token (refresh_token)
   - Expiration (token_expiration)
   ↓
8. HttpContext.SignInAsync() sets cookie
   ↓
9. Redirect to /users/dashboard or returnUrl
   ↓
10. [Authorize] attribute checks cookie → ✅ Access granted
```

### Authorization Flow

```
1. User requests protected page (e.g., /campaigns)
   ↓
2. [Authorize] attribute checks authentication
   ↓
3. Reads authentication cookie
   ↓
4. If cookie exists and valid:
   → Extracts claims (user ID, email, roles)
   → Allows access
   ↓
5. If cookie missing or expired:
   → Redirects to /auth/login?ReturnUrl=/campaigns
```

### API Call Flow (Server-Side)

```
1. User requests campaign list
   ↓
2. CampaignsController.Index()
   ↓
3. CampaignApiService.GetCampaignsAsync()
   ↓
4. EnsureAuthorization() extracts JWT from cookie claims
   ↓
5. ApiClient.SetAuthorizationToken(token)
   ↓
6. ApiClient.GetAsync() → API /api/campaigns
   Headers: Authorization: Bearer <JWT>
   ↓
7. API processes request with JWT authentication
   ↓
8. Returns campaign data
   ↓
9. CampaignApiService returns to controller
   ↓
10. Controller passes data to view
```

## Benefits

### 1. **Proper Authorization**
- `[Authorize]` attribute now works correctly
- Server-side session management
- No redirect loops to login page
- Proper returnUrl handling

### 2. **Security Improvements**
- HttpOnly cookies prevent XSS attacks
- Secure cookies (HTTPS only)
- SameSite protection against CSRF
- Tokens not exposed in JavaScript
- Anti-forgery token validation

### 3. **Maintainability**
- Centralized API communication logic
- Consistent error handling
- Type-safe API calls with generics
- Easy to add new API services

### 4. **Performance**
- Server-side rendering possible
- Reduced client-side JavaScript complexity
- Automatic token management
- Cookie-based sessions (no localStorage)

### 5. **User Experience**
- Smooth authentication flow
- Proper error messages
- returnUrl support for deep linking
- Remember me functionality
- Access denied page

## Configuration

### appsettings.json

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7011/api"
  },
  "JwtSettings": {
    "Secret": "YourSuperSecretKeyForJWTTokenGeneration12345!",
    "Issuer": "MarketingPlatform.API",
    "Audience": "MarketingPlatform.Web",
    "ExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7
  }
}
```

## Testing the Implementation

### 1. Start Both Projects

**Terminal 1: API**
```bash
cd src/MarketingPlatform.API
dotnet run
```

**Terminal 2: Web**
```bash
cd src/MarketingPlatform.Web
dotnet run
```

### 2. Test Login Flow

1. Navigate to `https://localhost:7061/auth/login`
2. Enter credentials
3. Submit form
4. Should redirect to `/users/dashboard`
5. Cookie should be set in browser

### 3. Test Authorization

1. Navigate to `https://localhost:7061/campaigns`
2. Should see campaigns page (not redirect to login)
3. Check browser DevTools → Application → Cookies
4. Should see `MarketingPlatform.Auth` cookie

### 4. Test Logout

1. Navigate to `/auth/logout`
2. Cookie should be cleared
3. Redirect to login page
4. Accessing `/campaigns` should redirect to login

## Troubleshooting

### Issue: Still redirecting to login after authentication

**Check:**
1. Cookie is being set (DevTools → Application → Cookies)
2. API is returning JWT token
3. Cookie expiration is in the future
4. HTTPS is enabled (cookies require secure context)

**Debug:**
```csharp
// Add to AuthenticationService.LoginAsync after cookie creation
_logger.LogInformation("Cookie created for user {UserId}", authResponse.UserId);
_logger.LogInformation("Cookie expires at {Expiry}", authResponse.TokenExpiration);
```

### Issue: 401 Unauthorized when calling API

**Check:**
1. JWT token is in cookie claims
2. `EnsureAuthorization()` is setting token on ApiClient
3. API is running and accessible
4. Token hasn't expired

**Debug:**
```csharp
// Add to CampaignApiService.EnsureAuthorization()
var token = httpContext.User.FindFirst("access_token")?.Value;
_logger.LogInformation("Using token: {Token}", token?.Substring(0, 20) + "...");
```

### Issue: Cookie not persisting across requests

**Check:**
1. `SameSite` policy matches your setup
2. HTTPS is configured correctly
3. Cookie domain matches your domain
4. Cookie isn't being cleared by browser

## Migration from Client-Side to Server-Side

### Backward Compatibility

The implementation maintains backward compatibility:
- JavaScript authentication still works for API-only clients
- Both POST and JavaScript login methods are supported
- Progressive enhancement: works without JavaScript

### Gradual Migration

You can migrate views gradually:
1. Update one controller at a time
2. Use server-side services in new features
3. Keep JavaScript for AJAX operations
4. Eventually remove redundant client-side auth code

## Future Enhancements

1. **Token Refresh Middleware** - Automatically refresh expired tokens
2. **Remember Me** - Long-lived cookies for persistent login
3. **Two-Factor Authentication** - Add 2FA support
4. **OAuth Integration** - Social login providers
5. **API Response Caching** - Cache frequent API calls
6. **Retry Policies** - Handle transient failures
7. **Circuit Breaker** - Protect against API downtime
8. **Health Checks** - Monitor API availability

## API Services Available

### Current Implementation
- ✅ ApiClient (generic HTTP client)
- ✅ AuthenticationService (login, register, logout)
- ✅ CampaignApiService (full CRUD + lifecycle)

### Easy to Add
Following the same pattern, you can easily add:
- ContactsApiService
- TemplatesApiService
- AnalyticsApiService
- MessagesApiService
- BillingApiService
- etc.

## Summary

This implementation provides a robust, secure, and maintainable authentication system that:
- ✅ Fixes the authorization redirect loop
- ✅ Implements proper cookie-based sessions
- ✅ Provides server-side HttpClient service layer
- ✅ Maintains security best practices
- ✅ Enables proper `[Authorize]` attribute functionality
- ✅ Supports progressive enhancement
- ✅ Follows ASP.NET Core conventions

The web application now properly authenticates users, creates secure sessions, and integrates seamlessly with the API backend.
