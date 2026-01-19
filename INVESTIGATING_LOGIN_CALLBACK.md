# Investigating the `/auth/login-callback` Endpoint

## The Mystery

Based on your previous network trace, you observed:
1. POST to `/auth/login` succeeds (200 OK)
2. Then a call to `/auth/login-callback` appears
3. Multiple cookies are set: `jwt_token`, `refresh_token`, `.AspNetCore.Cookies`, `.AspNetCore.Session`

**This endpoint doesn't exist in the Web application code we implemented.**

---

## Where It Could Be Coming From

### 1. External Authentication Provider (Most Likely)

OAuth providers (Google, Microsoft, Facebook, etc.) create callback endpoints automatically.

**Check API `Program.cs` for:**

```csharp
builder.Services.AddAuthentication()
    .AddGoogle(options => { ... })
    .AddMicrosoft(options => { ... })
    .AddOpenIdConnect(options => { ... })
    .AddFacebook(options => { ... });
```

These automatically create callback routes like:
- `/signin-google`
- `/signin-microsoft`
- `/auth/login-callback` (if configured)

**How to check:**
```bash
cd src/MarketingPlatform.API
grep -r "AddGoogle\|AddMicrosoft\|AddOpenIdConnect\|AddFacebook\|AddOAuth" .
```

Or search in Visual Studio/VS Code for these terms in the API project.

### 2. Identity Server or OAuth Middleware

If the API has Identity Server configured, it creates various auth endpoints.

**Check API for:**
- `AddIdentityServer()`
- `AddOpenIddict()`
- Custom OAuth middleware

### 3. Custom Middleware

Someone may have created custom authentication middleware.

**Check API for:**
```bash
grep -r "login-callback\|LoginCallback" src/MarketingPlatform.API
```

### 4. Third-Party Auth Library

Libraries like:
- `AspNet.Security.OAuth.Providers`
- `IdentityModel`
- Custom auth packages

**Check API `.csproj` for auth packages:**
```bash
cat src/MarketingPlatform.API/MarketingPlatform.API.csproj | grep -i "auth\|oauth\|identity"
```

---

## Why This Matters

If external authentication is configured in the API:

1. **It might be interfering with cookie auth**
   - Multiple authentication schemes compete
   - Different cookies are set by different handlers
   - The `[Authorize]` attribute might check the wrong scheme

2. **Cookies might be created by API, not Web**
   - If API is setting cookies, Web's cookie auth won't work
   - API should only return JWT tokens
   - Web should manage its own session cookies

3. **OAuth callback might redirect differently**
   - OAuth flows use redirects for callbacks
   - This could bypass our server-side login flow

---

## Solution Approaches

### Approach 1: Disable External Auth Temporarily

In **API Program.cs**, comment out external auth:

```csharp
// builder.Services.AddAuthentication()
//     .AddGoogle(...)
//     .AddMicrosoft(...);
```

This isolates the cookie auth to test if it works without OAuth interference.

### Approach 2: Separate Authentication Schemes

Keep both but clarify which is which:

**API** should use JWT Bearer tokens ONLY:
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { ... });

// External OAuth for users who want social login:
// .AddGoogle(...)
// .AddMicrosoft(...);
```

**Web** should use Cookie auth ONLY:
```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => { ... });
```

### Approach 3: Use External Auth Properly

If you WANT to use Google/Microsoft login:

1. User clicks "Sign in with Google" on Web
2. Web redirects to API's Google OAuth endpoint
3. API handles OAuth callback
4. API returns JWT to Web
5. Web creates cookie with the JWT

This requires implementing OAuth flow in Web controllers.

---

## Diagnosis Script

Create a file `check-api-auth.ps1`:

```powershell
# Check what authentication is configured in the API

Write-Host "Checking API authentication configuration..." -ForegroundColor Green

# Check Program.cs
Write-Host "`n1. Checking Program.cs for authentication setup:" -ForegroundColor Yellow
Get-Content src/MarketingPlatform.API/Program.cs | Select-String -Pattern "AddAuthentication|AddGoogle|AddMicrosoft|AddOpenIdConnect|AddFacebook|AddJwtBearer|AddIdentityServer"

# Check .csproj for auth packages
Write-Host "`n2. Checking for authentication packages:" -ForegroundColor Yellow
Get-Content src/MarketingPlatform.API/MarketingPlatform.API.csproj | Select-String -Pattern "Authentication|OAuth|Identity|OpenIdConnect"

# Check for login-callback in code
Write-Host "`n3. Searching for login-callback references:" -ForegroundColor Yellow
Get-ChildItem -Path src/MarketingPlatform.API -Recurse -Include "*.cs" | Select-String -Pattern "login-callback|LoginCallback" | Select-Object Path, LineNumber, Line

# Check appsettings for OAuth config
Write-Host "`n4. Checking appsettings for OAuth configuration:" -ForegroundColor Yellow
Get-Content src/MarketingPlatform.API/appsettings.json | Select-String -Pattern "Google|Microsoft|Facebook|OAuth|OpenId" -Context 2
Get-Content src/MarketingPlatform.API/appsettings.Development.json | Select-String -Pattern "Google|Microsoft|Facebook|OAuth|OpenId" -Context 2

Write-Host "`nDiagnosis complete!" -ForegroundColor Green
```

**Run it:**
```powershell
pwsh check-api-auth.ps1
```

Or on Linux/Mac:
```bash
chmod +x check-api-auth.sh
./check-api-auth.sh
```

---

## What to Look For

### Good (No OAuth Interference):
```
# API should have ONLY JWT Bearer authentication:
.AddJwtBearer(options => { ... })

# No external OAuth providers
# No Identity Server
# No OpenID Connect
```

### Problematic (OAuth Might Interfere):
```
.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddGoogle(options => { ... })
.AddMicrosoft(options => { ... })
```

This means API is configured for browser-based OAuth login, which conflicts with our JWT+Cookie approach.

---

## Recommended Architecture

**For this application, you should have:**

### API (MarketingPlatform.API):
- **JWT Bearer authentication ONLY**
- Returns JWT tokens in API responses
- No cookies, no sessions, stateless
- External OAuth can be here for "Sign in with Google" functionality

### Web (MarketingPlatform.Web):
- **Cookie authentication ONLY**
- Calls API to get JWT
- Stores JWT in encrypted cookie
- Manages user session with cookies
- Extracts JWT from cookie for API calls

### Flow:
```
1. User submits login form (Web)
   ↓
2. Web calls API /auth/login
   ↓
3. API validates credentials, returns JWT
   ↓
4. Web receives JWT, creates cookie with SignInAsync
   ↓
5. User is authenticated via cookie
   ↓
6. Protected pages extract JWT from cookie to call API
   ↓
7. API validates JWT on each request
```

This is the architecture we implemented. If OAuth is interfering, it needs to be separated or disabled.

---

## Next Steps

1. **Run the diagnosis script** to see what's configured in API
2. **Check if OAuth is enabled** in API Program.cs
3. **Temporarily disable OAuth** to test if cookie auth works
4. **Report findings** so we can adjust the implementation

The `/auth/login-callback` endpoint is the key clue. Finding where it's defined will solve the mystery!
