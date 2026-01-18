# Content Security Policy (CSP) Implementation

## Overview

This application implements a nonce-based Content Security Policy to prevent CSP violations while maintaining security. The implementation differentiates between Development and Production environments to balance security with developer experience.

## Components

### 1. CspMiddleware (`src/MarketingPlatform.API/Middleware/CspMiddleware.cs`)

The CSP middleware generates a unique cryptographically secure nonce for each HTTP request and sets appropriate CSP headers based on the environment.

**Key Features:**
- Generates 256-bit cryptographically secure nonce using `System.Security.Cryptography.RandomNumberGenerator`
- Stores nonce in `HttpContext.Items["csp-nonce"]` for access in views
- Sets all security headers (X-Content-Type-Options, X-Frame-Options, etc.)
- Environment-aware CSP policies

**Development CSP:**
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

**Production CSP:**
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

### 2. CspExtensions (`src/MarketingPlatform.API/Extensions/CspExtensions.cs`)

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
- **No unsafe-eval**: Scripts must be external or use nonce
- **No unsafe-inline**: All inline scripts and styles must use nonce
- **Limited connections**: Only 'self' allowed in connect-src
- **Frame protection**: `frame-ancestors 'none'` prevents clickjacking
- **Form protection**: `form-action 'self'` prevents form hijacking

## What This Fixes

This implementation resolves the following CSP violations:

1. ✅ **Inline styles** - Now allowed with nonce
2. ✅ **Inline scripts** - Now allowed with nonce
3. ✅ **WebSocket connections** - Allowed in development for browser-link and hot reload
4. ✅ **Localhost connections** - Allowed in development for debugging tools

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
