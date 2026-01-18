using System.Security.Cryptography;

namespace MarketingPlatform.API.Middleware
{
    /// <summary>
    /// Middleware for implementing Content Security Policy (CSP) with nonce-based inline script/style execution.
    /// Generates a unique cryptographically secure nonce per request and sets appropriate CSP headers
    /// based on the environment (Development vs Production).
    /// </summary>
    public class CspMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<CspMiddleware> _logger;

        public CspMiddleware(
            RequestDelegate next,
            IWebHostEnvironment environment,
            ILogger<CspMiddleware> logger)
        {
            _next = next;
            _environment = environment;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Generate a cryptographically secure nonce for this request
            var nonce = GenerateNonce();
            
            // Store nonce in HttpContext.Items so it can be accessed by views
            context.Items["csp-nonce"] = nonce;

            // Remove server header for security
            context.Response.Headers.Remove("Server");
            
            // Add security headers
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
            
            // Set CSP header based on environment
            var cspHeader = _environment.IsDevelopment()
                ? BuildDevelopmentCsp(nonce)
                : BuildProductionCsp(nonce);
            
            context.Response.Headers.Append("Content-Security-Policy", cspHeader);
            
            _logger.LogDebug("CSP header set with nonce: {Nonce} for environment: {Environment}", 
                nonce, _environment.EnvironmentName);

            await _next(context);
        }

        /// <summary>
        /// Generates a cryptographically secure random nonce.
        /// </summary>
        /// <returns>Base64-encoded nonce string</returns>
        private static string GenerateNonce()
        {
            var nonceBytes = new byte[32]; // 256 bits
            RandomNumberGenerator.Fill(nonceBytes);
            return Convert.ToBase64String(nonceBytes);
        }

        /// <summary>
        /// Builds a CSP header for development environment.
        /// More permissive to allow hot reload, browser-link, and WebSocket connections.
        /// </summary>
        /// <param name="nonce">The nonce to include in the CSP</param>
        /// <returns>CSP header value</returns>
        private static string BuildDevelopmentCsp(string nonce)
        {
            return $"default-src 'self'; " +
                   $"script-src 'self' 'nonce-{nonce}' 'unsafe-eval'; " +
                   $"style-src 'self' 'nonce-{nonce}' 'unsafe-inline'; " +
                   $"connect-src 'self' ws: wss: http://localhost:* https://localhost:*; " +
                   $"img-src 'self' data: https:; " +
                   $"font-src 'self'; " +
                   $"object-src 'none'; " +
                   $"base-uri 'self';";
        }

        /// <summary>
        /// Builds a strict CSP header for production environment.
        /// Uses nonces for inline scripts/styles without allowing unsafe-inline or unsafe-eval.
        /// </summary>
        /// <param name="nonce">The nonce to include in the CSP</param>
        /// <returns>CSP header value</returns>
        private static string BuildProductionCsp(string nonce)
        {
            return $"default-src 'self'; " +
                   $"script-src 'self' 'nonce-{nonce}'; " +
                   $"style-src 'self' 'nonce-{nonce}'; " +
                   $"connect-src 'self'; " +
                   $"img-src 'self' data: https:; " +
                   $"font-src 'self'; " +
                   $"object-src 'none'; " +
                   $"base-uri 'self'; " +
                   $"form-action 'self'; " +
                   $"frame-ancestors 'none';";
        }
    }
}
