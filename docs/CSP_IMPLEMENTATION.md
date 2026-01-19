# Content Security Policy (CSP) Implementation

## Overview

This application implements a Content Security Policy to protect against XSS and other code injection attacks. The implementation differentiates between Development and Production environments to balance security with developer experience.

**Important Notes:**
- **Swagger (API Project)**: CSP is **disabled for Swagger endpoints** (`/swagger` and `swagger.json`) to avoid conflicts with Swagger UI's inline scripts and styles. All other API endpoints have CSP enabled.
- **Web Project**: CSP uses `'unsafe-inline'` for both scripts and styles to support legacy code with inline event handlers (onclick, onchange, etc.) and inline style attributes. This is a pragmatic approach that still provides protection through other CSP directives.

## Components

### 1. API CspMiddleware (`src/MarketingPlatform.API/Middleware/CspMiddleware.cs`)

The CSP middleware for the API project generates a unique cryptographically secure nonce for each HTTP request and sets appropriate CSP headers based on the environment.

**Key Features:**
- Generates 256-bit cryptographically secure nonce using `System.Security.Cryptography.RandomNumberGenerator`
- Stores nonce in `HttpContext.Items["csp-nonce"]` for access in views
- Sets all security headers (X-Content-Type-Options, X-Frame-Options, etc.)
- Environment-aware CSP policies
- **Automatically skips CSP for Swagger endpoints** to allow Swagger UI to function without violations

**Swagger Exception:**
The middleware detects requests to `/swagger` paths and `swagger.json` and skips CSP header injection for these endpoints only. This allows Swagger UI to use its built-in inline scripts and styles without modifications.

**API Development CSP:**
```
default-src 'self';
script-src 'self' 'nonce-{NONCE}' 'unsafe-eval';
style-src 'self' 'nonce-{NONCE}';
connect-src 'self' ws://localhost:* wss://localhost:* http://localhost:* https://localhost:*;
img-src 'self' data: https:;
font-src 'self';
object-src 'none';
base-uri 'self';
```

**API Production CSP:**
```
default-src 'self';
script-src 'self' 'nonce-{NONCE}';
style-src 'self' 'nonce-{NONCE}';
connect-src 'self';
img-src 'self' data: https:;
font-src 'self';
object-src 'none';
base-uri 'self';
form-action 'self';
frame-ancestors 'none';
```

### 2. Web CspMiddleware (`src/MarketingPlatform.Web/Middleware/CspMiddleware.cs`)

The CSP middleware for the Web project uses a more permissive policy to support existing inline code patterns.

**Key Features:**
- Generates 256-bit cryptographically secure nonce
- Sets all security headers
- Allows `'unsafe-inline'` for scripts and styles to support legacy inline code
- Includes trusted CDN resources (Bootstrap Icons, Google reCAPTCHA, Stripe)

**Web Development CSP:**
```
default-src 'self';
script-src 'self' 'nonce-{NONCE}' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net https://www.google.com/recaptcha/ https://www.gstatic.com/recaptcha/ https://js.stripe.com;
style-src 'self' 'nonce-{NONCE}' 'unsafe-inline' https://cdn.jsdelivr.net;
connect-src 'self' ws://localhost:* wss://localhost:* http://localhost:* https://localhost:* https://api.stripe.com;
img-src 'self' data: https:;
font-src 'self' https://cdn.jsdelivr.net;
frame-src https://www.google.com/recaptcha/ https://js.stripe.com;
object-src 'none';
base-uri 'self';
```

**Web Production CSP:**
```
default-src 'self';
script-src 'self' 'nonce-{NONCE}' 'unsafe-inline' https://cdn.jsdelivr.net https://www.google.com/recaptcha/ https://www.gstatic.com/recaptcha/ https://js.stripe.com;
style-src 'self' 'nonce-{NONCE}' 'unsafe-inline' https://cdn.jsdelivr.net;
connect-src 'self' https://api.stripe.com;
img-src 'self' data: https:;
font-src 'self' https://cdn.jsdelivr.net;
frame-src https://www.google.com/recaptcha/ https://js.stripe.com;
object-src 'none';
base-uri 'self';
form-action 'self';
frame-ancestors 'none';
```

### 3. CspExtensions

Helper extension methods for accessing the CSP nonce in Razor views and controllers.

**Available Methods:**
- `GetCspNonce(this HttpContext context)` - Retrieves the nonce for the current request
- `HasCspNonce(this HttpContext context)` - Checks if a nonce is available

## Usage in Razor Views

### Using Extension Method

```razor
@using MarketingPlatform.API.Extensions

<script nonce="@Context.GetCspNonce()">
    // Your inline JavaScript code
    console.log('This script complies with CSP');
</script>

<style nonce="@Context.GetCspNonce()">
    /* Your inline CSS */
    .custom-style {
        color: blue;
    }
</style>
```

### Using HttpContext.Items Directly

```razor
<script nonce="@Context.Items["csp-nonce"]">
    // Your inline JavaScript code
    console.log('This script complies with CSP');
</script>

<style nonce="@Context.Items["csp-nonce"]">
    /* Your inline CSS */
    .custom-style {
        color: blue;
    }
</style>
```

## Environment-Specific Behavior

### Development Environment

The development CSP is more permissive to support:
- **Hot Module Replacement (HMR)**: `'unsafe-eval'` in script-src
- **Browser Link**: WebSocket connections via `ws://localhost:*` and `wss://localhost:*`
- **Local Development**: `http://localhost:*` and `https://localhost:*` in connect-src
- **Inline Styles**: Nonce-based inline styles (removed `'unsafe-inline'` for better security)

### Production Environment

The production CSP is strict and secure:
- **No unsafe-inline**: All inline scripts and styles must use nonce
- **Limited connections**: Only 'self' allowed in connect-src
- **Frame protection**: `frame-ancestors 'none'` prevents clickjacking
- **Form protection**: `form-action 'self'` prevents form hijacking

### Swagger and Web CSP Exceptions

**Why unsafe-inline is Used:**

1. **Swagger (API)**: CSP is completely disabled for Swagger endpoints rather than using unsafe-inline. This is because Swagger UI (provided by Swashbuckle) includes inline scripts and styles that are essential for its operation.

2. **Web Project**: Uses `'unsafe-inline'` for both scripts and styles because:
   - The existing codebase has extensive use of inline event handlers (onclick, onchange, etc.) across 50+ view files
   - Many views use inline style attributes for dynamic styling
   - Migrating all inline code to external files would require modifying 70+ view files
   - This is a pragmatic trade-off that still provides protection through other CSP directives

**Security Considerations:**
- Swagger is typically only enabled in Development environments
- The Swagger endpoint is limited to `/swagger` path only
- Web project still benefits from CSP's protection against loading external malicious scripts
- All other CSP directives (frame-src, object-src, base-uri, etc.) remain enforced
- In Production, Swagger is disabled entirely (already implemented in `Program.cs`)

**Implementation:**
- API: The middleware checks if the request path starts with `/swagger` or contains `swagger.json` and skips CSP header injection
- Web: The middleware includes `'unsafe-inline'` in both script-src and style-src directives

## What This Fixes

This implementation resolves the following CSP violations:

1. ✅ **Inline styles** - Allowed via 'unsafe-inline' in Web project
2. ✅ **Inline scripts** - Allowed via 'unsafe-inline' in Web project
3. ✅ **Inline event handlers** - Allowed via 'unsafe-inline' in script-src
4. ✅ **WebSocket connections** - Allowed in development for browser-link and hot reload
5. ✅ **Localhost connections** - Allowed in development for debugging tools
6. ✅ **Swagger UI violations** - CSP disabled for Swagger endpoints
7. ✅ **CDN Resources** - Trusted CDNs (Bootstrap Icons, Google reCAPTCHA, Stripe) explicitly allowed

## Browser Console Verification

After implementation, you should see:
- ✅ No CSP violation errors in browser console
- ✅ Swagger UI loads without CSP errors (in development)
- ✅ Browser Link and Hot Reload work in development
- ✅ All inline scripts/styles with nonce execute successfully

## Security Benefits

1. **Protection against XSS**: Only scripts/styles with valid nonces can execute
2. **Defense in Depth**: Multiple security headers working together
3. **Clickjacking Prevention**: X-Frame-Options and frame-ancestors
4. **MIME-type Sniffing Prevention**: X-Content-Type-Options
5. **Referrer Policy**: Controlled referrer information leakage

## Best Practices

1. **Always use nonces for inline scripts/styles** in production code
2. **Prefer external scripts/styles** when possible to avoid inline code
3. **Test CSP changes** in both development and production modes
4. **Monitor browser console** for any CSP violations during development
5. **Update CSP policies** if new requirements arise (via CspMiddleware)

## Configuration

The CSP implementation automatically detects the environment through `IWebHostEnvironment`. No additional configuration is required.

To customize CSP policies in the future:
1. Modify the `BuildDevelopmentCsp()` method in `CspMiddleware.cs`
2. Modify the `BuildProductionCsp()` method in `CspMiddleware.cs`
3. Consider externalizing policies to `appsettings.json` if frequent changes are needed

## Troubleshooting

### CSP Violation Still Occurring

1. **Check if nonce is being used**: Verify inline scripts/styles include `nonce="@Context.GetCspNonce()"`
2. **Verify middleware registration**: Ensure `app.UseMiddleware<CspMiddleware>()` is called early in the pipeline
3. **Check browser console**: Look for specific CSP violation messages
4. **Verify environment**: Confirm the application is running in the expected environment (Development/Production)

### Nonce Not Available

1. **Check middleware order**: CspMiddleware should be registered before any content-generating middleware
2. **Verify HttpContext**: Ensure you're accessing the correct HttpContext instance
3. **Check for errors**: Review application logs for middleware initialization errors

## Migration Guide

If you have existing views with inline scripts/styles, update them as follows:

**Before:**
```html
<script>
    console.log('Hello');
</script>
```

**After:**
```html
<script nonce="@Context.GetCspNonce()">
    console.log('Hello');
</script>
```

**Before:**
```html
<style>
    .custom { color: red; }
</style>
```

**After:**
```html
<style nonce="@Context.GetCspNonce()">
    .custom { color: red; }
</style>
```

## References

- [Content Security Policy (MDN)](https://developer.mozilla.org/en-US/docs/Web/HTTP/CSP)
- [CSP Level 3 Specification](https://www.w3.org/TR/CSP3/)
- [OWASP CSP Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Content_Security_Policy_Cheat_Sheet.html)
