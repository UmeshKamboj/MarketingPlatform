/**
 * landing-page.js - Landing page functionality
 * Handles pricing plans loading and display
 */

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    loadPricingPlans();
});

/**
 * Load pricing plans from API
 */
async function loadPricingPlans() {
    const container = document.getElementById('pricing-plans');
    if (!container) return;
    
    try {
        // Build full API URL using AppUrls helper - NO STATIC URLS
        const apiUrl = window.AppUrls ? 
            window.AppUrls.buildApiUrl(window.AppUrls.api.subscriptionPlans.landing) : 
            '/api/subscriptionplans/landing';
        
        // Fetch plans from API that are marked to show on landing page
        const response = await fetch(apiUrl);
        
        if (!response.ok) {
            throw new Error('Failed to load pricing plans');
        }

        const result = await response.json();
        
        if (result.success && result.data && result.data.length > 0) {
            renderPricingPlans(result.data);
        } else {
            container.innerHTML = `
                <div class="col-12">
                    <div class="alert alert-info text-center">
                        <i class="bi bi-info-circle"></i> No pricing plans available at this time.
                    </div>
                </div>
            `;
        }
    } catch (error) {
        console.error('Error loading pricing plans:', error);
        container.innerHTML = `
            <div class="col-12">
                <div class="alert alert-warning text-center">
                    <i class="bi bi-exclamation-triangle"></i> Unable to load pricing plans at this time.
                </div>
            </div>
        `;
    }
}

/**
 * Render pricing plans in the container
 * @param {Array} plans - Array of pricing plan objects
 */
function renderPricingPlans(plans) {
    const container = document.getElementById('pricing-plans');
    
    let html = '';
    plans.forEach((plan, index) => {
        // Parse features from JSON string if needed
        let features = [];
        try {
            features = plan.features ? (typeof plan.features === 'string' ? JSON.parse(plan.features) : plan.features) : [];
        } catch (e) {
            console.error('Error parsing features for plan:', plan.name, e);
            features = [];
        }

        // Determine if plan is popular (middle plan typically)
        const isPopular = index === Math.floor(plans.length / 2);
        const popularBadge = isPopular ? '<div class="position-absolute top-0 start-50 translate-middle"><span class="badge bg-warning text-dark">Most Popular</span></div>' : '';
        const cardClass = isPopular ? 'border-primary shadow-lg' : 'border-0 shadow-sm';
        
        html += `
            <div class="col-lg-4 col-md-6">
                <div class="card h-100 ${cardClass} position-relative">
                    ${popularBadge}
                    <div class="card-body p-4">
                        <h3 class="card-title text-center mb-3">${escapeHtml(plan.name)}</h3>
                        <p class="text-muted text-center mb-4">${escapeHtml(plan.description || '')}</p>
                        <div class="text-center mb-4">
                            <span class="display-4 fw-bold">$${plan.priceMonthly ? plan.priceMonthly.toFixed(2) : '0.00'}</span>
                            <span class="text-muted">/month</span>
                        </div>
                        <ul class="list-unstyled mb-4">
                            ${features.map(feature => `
                                <li class="mb-2">
                                    <i class="bi bi-check-circle-fill text-success me-2"></i>
                                    ${escapeHtml(feature)}
                                </li>
                            `).join('')}
                            ${features.length === 0 ? '<li class="text-muted">No features listed</li>' : ''}
                        </ul>
                        <div class="text-center">
                            <a href="/Auth/Register?plan=${plan.id}" class="btn ${isPopular ? 'btn-primary' : 'btn-outline-primary'} w-100">
                                Choose ${escapeHtml(plan.name)}
                            </a>
                        </div>
                    </div>
                </div>
            </div>
        `;
    });

    container.innerHTML = html;
}

/**
 * Escape HTML to prevent XSS
 * @param {string} text - Text to escape
 * @returns {string} Escaped text
 */
function escapeHtml(text) {
    if (!text) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}
