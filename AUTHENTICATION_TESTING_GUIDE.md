# Authentication Testing Guide

## Quick Fix Applied ✅

### Changes Made:
1. **Removed Identity conflict** - Removed ASP.NET Core Identity which was conflicting with cookie auth
2. **Fixed cookie authentication** - Configured proper cookie-based authentication
3. **Disabled JavaScript interception** - Login form now submits to server instead of JavaScript API call
4. **Made reCAPTCHA optional** - Won't block testing if not configured

---

## Testing Steps

### 1. Start Both Applications

**Terminal 1 - Start API:**
```bash
cd src/MarketingPlatform.API
dotnet run
```
Wait for: `Now listening on: https://localhost:7011`

**Terminal 2 - Start Web:**
```bash
cd src/MarketingPlatform.Web
dotnet run
```
Wait for: `Now listening on: https://localhost:7061`

### 2. Create Test User (Using API)

**Option A: Using Postman/cURL:**
```bash
curl -X POST https://localhost:7011/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test@123456",
    "firstName": "Test",
    "lastName": "User",
    "phoneNumber": "+1234567890"
  }'
```

**Option B: Using Browser Console** (visit `https://localhost:7061` first):
```javascript
fetch('https://localhost:7011/api/auth/register', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
        email: 'test@example.com',
        password: 'Test@123456',
        firstName: 'Test',
        lastName: 'User',
        phoneNumber: '+1234567890'
    })
})
.then(r => r.json())
.then(console.log);
```

### 3. Test Login

1. Navigate to: `https://localhost:7061/auth/login`
2. Enter credentials:
   - **Email:** `test@example.com`
   - **Password:** `Test@123456`
3. Click "Sign In"
4. **Expected Result:** Redirected to `/users/dashboard`

### 4. Verify Cookie

1. Open DevTools (F12)
2. Go to Application → Cookies
3. Look for `MarketingPlatform.Auth` cookie
4. Should see encrypted session data

### 5. Test Protected Pages

1. Navigate to: `https://localhost:7061/campaigns`
2. **Expected:** Page loads (no redirect to login)
3. Navigate to: `https://localhost:7061/users/dashboard`
4. **Expected:** Dashboard loads

### 6. Test Logout

1. Navigate to: `https://localhost:7061/auth/logout`
2. **Expected:** Redirected to login page
3. Check cookies - `MarketingPlatform.Auth` should be gone
4. Try accessing `/campaigns` - should redirect to login

---

## Troubleshooting

### Still Getting Redirect Loop?

#### Check 1: Clear Browser Data
```
1. Open DevTools (F12)
2. Right-click refresh button → "Empty Cache and Hard Reload"
3. Or: Settings → Privacy → Clear browsing data
4. Close and reopen browser
```

#### Check 2: Verify API is Running
```bash
curl https://localhost:7011/api/health
```
Should return health status.

#### Check 3: Check Server Logs

**Web Application Logs:**
Look for:
- `User {Email} logged in successfully via server-side`
- Any errors in authentication

**API Application Logs:**
Look for:
- Registration/login requests
- JWT token generation
- Any authentication errors

#### Check 4: Test API Directly

```bash
# Test Registration
curl -k -X POST https://localhost:7011/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"Test@123456","firstName":"Test","lastName":"User"}'

# Test Login
curl -k -X POST https://localhost:7011/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"Test@123456"}'
```

Should return JWT token.

### Login Form Not Submitting?

#### Check Browser Console
1. Open DevTools (F12) → Console tab
2. Look for JavaScript errors
3. Verify no errors before form submission

#### Check Network Tab
1. Open DevTools (F12) → Network tab
2. Submit the form
3. Look for POST request to `/auth/login`
4. Check request payload and response

### Cookie Not Being Set?

#### Check Response Headers
1. DevTools → Network → Select login request
2. Check Response Headers for `Set-Cookie`
3. Should see `MarketingPlatform.Auth=...`

#### Check Cookie Security Settings
Make sure:
- Browser allows cookies
- Not in incognito/private mode (unless testing that specifically)
- Third-party cookie settings don't block first-party cookies

### API Connection Issues?

#### Check CORS Settings
If API and Web are on different domains, verify CORS in API `Program.cs`:
```csharp
app.UseCors(policy => policy
    .WithOrigins("https://localhost:7061")
    .AllowCredentials()
    .AllowAnyHeader()
    .AllowAnyMethod());
```

#### Check Ports
- API should be on: `https://localhost:7011`
- Web should be on: `https://localhost:7061`

Verify in `appsettings.json`:
```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7011/api"
  }
}
```

---

## Common Issues and Solutions

### Issue: "The antiforgery token could not be decrypted"

**Solution:**
1. Clear browser cookies
2. Restart Web application
3. Try again

### Issue: "Cannot connect to API"

**Solution:**
1. Verify API is running: `curl https://localhost:7011/api/health`
2. Check firewall isn't blocking port 7011
3. Verify `ApiSettings:BaseUrl` in `appsettings.json`

### Issue: "Invalid email or password"

**Solution:**
1. Verify user exists in database
2. Check password meets requirements:
   - At least 8 characters
   - Contains uppercase letter
   - Contains lowercase letter
   - Contains digit
   - Contains special character
3. Try registering a new user

### Issue: "Access Denied" page

**Solution:**
1. This means authentication worked, but authorization failed
2. Check user roles in database
3. Verify controller/action authorization requirements

---

## Database Inspection

### Check if User Exists

**SQL Server:**
```sql
USE MarketingPlatformDb;
SELECT * FROM AspNetUsers WHERE Email = 'test@example.com';
```

### Check User Roles

```sql
SELECT u.Email, r.Name as Role
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.Email = 'test@example.com';
```

### Create User Manually (If Needed)

Use the API registration endpoint - it handles password hashing correctly.

---

## Debug Mode

### Enable Detailed Logging

**appsettings.Development.json:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore.Authentication": "Debug",
      "Microsoft.AspNetCore.Authorization": "Debug",
      "MarketingPlatform.Web.Services": "Debug"
    }
  }
}
```

Restart web application to see detailed auth logs.

---

## Success Indicators

✅ **Login Successful:**
- No redirect loop
- Redirected to `/users/dashboard`
- Cookie `MarketingPlatform.Auth` present
- Can access protected pages

✅ **Authentication Working:**
- Server logs show: "User {email} logged in successfully"
- Browser DevTools shows cookie
- Network tab shows successful POST to `/auth/login`

✅ **Authorization Working:**
- Protected pages load without redirect
- Appropriate error pages for insufficient permissions
- Logout clears session properly

---

## Test Checklist

- [ ] API is running on port 7011
- [ ] Web is running on port 7061
- [ ] Test user created via API
- [ ] Can access login page
- [ ] Can submit login form
- [ ] Redirected to dashboard (not login)
- [ ] Cookie is set in browser
- [ ] Can access protected pages (/campaigns, /users/dashboard)
- [ ] Logout clears cookie
- [ ] After logout, redirected to login when accessing protected pages

---

## Quick Test Script

**PowerShell:**
```powershell
# Register user
$registerData = @{
    email = "test@example.com"
    password = "Test@123456"
    firstName = "Test"
    lastName = "User"
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:7011/api/auth/register" `
    -Method POST `
    -Body $registerData `
    -ContentType "application/json" `
    -SkipCertificateCheck

# Login
$loginData = @{
    email = "test@example.com"
    password = "Test@123456"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "https://localhost:7011/api/auth/login" `
    -Method POST `
    -Body $loginData `
    -ContentType "application/json" `
    -SkipCertificateCheck

Write-Host "JWT Token: $($response.data.token)"
```

---

## Next Steps After Success

1. ✅ Verify authentication works
2. ✅ Test all protected pages
3. ✅ Configure reCAPTCHA (optional)
4. ✅ Add more test users
5. ✅ Test role-based authorization
6. ✅ Configure production settings

---

## Support

If you're still experiencing issues:

1. Check the server logs (Web and API)
2. Check browser console for JavaScript errors
3. Verify database connection
4. Verify API is accessible
5. Try with a fresh browser session
6. Restart both applications

The authentication system is designed to work - if it's not, there's likely a configuration issue or the API isn't running properly.
