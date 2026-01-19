/**
 * auth-login.js - Login page functionality
 * Handles login form submission, password toggle, and reCAPTCHA validation
 */

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    initializePasswordToggle();
    initializeLoginForm();
    loadRecaptchaConfig();
});

/**
 * Load reCAPTCHA configuration dynamically from API
 */
async function loadRecaptchaConfig() {
    try {
        // Check if authConfig is available (set from server)
        if (typeof window.authConfig !== 'undefined' && window.authConfig.recaptchaSiteKey) {
            // Use the siteKey from server-side config
            renderRecaptcha(window.authConfig.recaptchaSiteKey);
        } else {
            // Fallback: try to fetch from API endpoint
            const response = await fetch('/api/auth/recaptcha-config');
            if (response.ok) {
                const config = await response.json();
                renderRecaptcha(config.siteKey);
            } else {
                console.warn('Could not load reCAPTCHA configuration');
            }
        }
    } catch (error) {
        console.error('Error loading reCAPTCHA config:', error);
    }
}

/**
 * Render reCAPTCHA widget
 */
function renderRecaptcha(siteKey) {
    const recaptchaContainer = document.querySelector('.g-recaptcha');
    if (recaptchaContainer && siteKey && siteKey !== 'YOUR_RECAPTCHA_SITE_KEY') {
        recaptchaContainer.setAttribute('data-sitekey', siteKey);
    }
}

/**
 * Initialize password visibility toggle
 */
function initializePasswordToggle() {
    const toggleButton = document.getElementById('togglePassword');
    if (!toggleButton) return;

    toggleButton.addEventListener('click', function() {
        const passwordInput = document.getElementById('password');
        const icon = this.querySelector('i');
        
        if (passwordInput.type === 'password') {
            passwordInput.type = 'text';
            icon.classList.replace('bi-eye', 'bi-eye-slash');
        } else {
            passwordInput.type = 'password';
            icon.classList.replace('bi-eye-slash', 'bi-eye');
        }
    });
}

/**
 * Initialize login form submission
 */
function initializeLoginForm() {
    const loginForm = document.getElementById('loginForm');
    if (!loginForm) return;

    // Check if server-side authentication is enabled
    // If so, don't intercept - let the form submit naturally
    if (window.authConfig && window.authConfig.useServerSideAuth === true) {
        console.log('Server-side authentication enabled - form will submit to server');
        return; // Don't add event listener, allow normal form submission
    }

    loginForm.addEventListener('submit', async function(e) {
        e.preventDefault();
        
        const email = document.getElementById('email').value;
        const password = document.getElementById('password').value;
        const rememberMe = document.getElementById('rememberMe').checked;
        const errorDiv = document.getElementById('errorMessage');
        const successDiv = document.getElementById('successMessage');
        const loginBtn = document.getElementById('loginBtn');
        
        // Get reCAPTCHA response
        let recaptchaResponse = '';
        try {
            recaptchaResponse = grecaptcha.getResponse();
        } catch (error) {
            console.warn('reCAPTCHA not available:', error);
        }
        
        if (recaptchaResponse === '') {
            errorDiv.textContent = 'Please complete the reCAPTCHA verification';
            errorDiv.classList.remove('d-none');
            successDiv.classList.add('d-none');
            return;
        }
        
        // Clear previous messages
        errorDiv.classList.add('d-none');
        successDiv.classList.add('d-none');
        
        // Disable button and show loading state
        loginBtn.disabled = true;
        loginBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Signing in...';
        
        try {
            // Get API base URL from config or fallback
            const apiBaseUrl = (window.authConfig && window.authConfig.apiBaseUrl) || '/api';
            
            // Call login API using AppUrls if available
            const loginUrl = (window.AppUrls && window.AppUrls.api && window.AppUrls.api.auth && window.AppUrls.api.auth.login) 
                || `${apiBaseUrl}/auth/login`;
            
            const response = await fetch(loginUrl, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ 
                    email, 
                    password, 
                    rememberMe,
                    recaptchaToken: recaptchaResponse
                })
            });
            
            const data = await response.json();
            
            if (response.ok) {
                // Store authentication token
                localStorage.setItem('authToken', data.data.token);
                localStorage.setItem('userEmail', email);
                
                // Show success notification
                if (typeof showNotification === 'function') {
                    showNotification('Login successful! Redirecting...', 'success');
                } else {
                    successDiv.textContent = 'Login successful! Redirecting...';
                    successDiv.classList.remove('d-none');
                }
                
                // Redirect to dashboard using AppUrls if available
                const dashboardUrl = (window.AppUrls && window.AppUrls.users && window.AppUrls.users.dashboard) 
                    || '/users/dashboard';
                
                setTimeout(() => {
                    window.location.href = dashboardUrl;
                }, 1000);
            } else {
                const errorMessage = data.message || 'Invalid email or password';
                
                if (typeof showNotification === 'function') {
                    showNotification(errorMessage, 'error');
                } else {
                    errorDiv.textContent = errorMessage;
                    errorDiv.classList.remove('d-none');
                }
                
                // Reset reCAPTCHA
                try {
                    grecaptcha.reset();
                } catch (error) {
                    console.warn('Could not reset reCAPTCHA:', error);
                }
            }
        } catch (error) {
            console.error('Login error:', error);
            const errorMessage = 'An error occurred. Please try again.';
            
            if (typeof showNotification === 'function') {
                showNotification(errorMessage, 'error');
            } else {
                errorDiv.textContent = errorMessage;
                errorDiv.classList.remove('d-none');
            }
            
            // Reset reCAPTCHA
            try {
                grecaptcha.reset();
            } catch (error) {
                console.warn('Could not reset reCAPTCHA:', error);
            }
        } finally {
            // Re-enable button
            loginBtn.disabled = false;
            loginBtn.innerHTML = '<i class="bi bi-box-arrow-in-right"></i> Sign In';
        }
    });
}
