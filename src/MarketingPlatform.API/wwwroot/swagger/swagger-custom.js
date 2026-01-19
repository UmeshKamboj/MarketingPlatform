/* Custom Swagger UI JavaScript - Externalized for CSP Compliance */

// Initialize Swagger UI when DOM is ready
function initializeSwaggerUI() {
    // Get the base URL from the page
    const baseUrl = window.location.origin;
    
    // Initialize Swagger UI with custom configuration
    window.ui = SwaggerUIBundle({
        url: baseUrl + "/swagger/v1/swagger.json",
        dom_id: '#swagger-ui',
        deepLinking: true,
        presets: [
            SwaggerUIBundle.presets.apis,
            SwaggerUIStandalonePreset
        ],
        plugins: [
            SwaggerUIBundle.plugins.DownloadUrl
        ],
        layout: "StandaloneLayout",
        defaultModelsExpandDepth: 1,
        defaultModelExpandDepth: 1,
        docExpansion: "list",
        filter: true,
        showExtensions: true,
        showCommonExtensions: true,
        displayRequestDuration: true,
        tryItOutEnabled: true,
        persistAuthorization: true,
        
        // Custom request interceptor for adding authorization header
        requestInterceptor: function(request) {
            // Authorization is handled by Swagger UI's built-in auth system
            return request;
        },
        
        // Custom response interceptor for logging
        responseInterceptor: function(response) {
            // Log responses in development mode
            if (window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1') {
                console.log('API Response:', response);
            }
            return response;
        },
        
        // Error handling
        onComplete: function() {
            console.log('Swagger UI loaded successfully');
        }
    });
}

// Wait for DOM to be fully loaded
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeSwaggerUI);
} else {
    // DOM is already loaded
    initializeSwaggerUI();
}

// Helper function to handle authorization persistence
function setupAuthPersistence() {
    // Swagger UI already handles auth persistence via persistAuthorization config
    // This function is here for any additional custom auth handling if needed
}

// Add custom event listeners for Swagger UI interactions
function addCustomEventListeners() {
    // Add any custom event listeners here if needed
    // For example, tracking API calls, custom analytics, etc.
}

// Initialize custom features after Swagger UI loads
window.addEventListener('load', function() {
    setupAuthPersistence();
    addCustomEventListeners();
});
