/**
 * URL Configuration - Centralized URL Management
 * All application URLs in one place for easy maintenance
 */

const AppUrls = {
    // Home & Public Pages
    home: {
        index: '/home/index',
        privacy: '/home/privacy',
        terms: '/home/terms',
        error: '/home/error'
    },

    // Authentication
    auth: {
        login: '/auth/login',
        register: '/auth/register',
        logout: '/auth/logout',
        forgotPassword: '/auth/forgot-password',
        resetPassword: '/auth/reset-password'
    },

    // Keywords Management
    keywords: {
        index: '/keywords/index',
        create: '/keywords/create',
        edit: (id) => `/keywords/edit/${id}`,
        analytics: (id) => `/keywords/analytics/${id}`,
        details: (id) => `/keywords/details/${id}`,
        delete: (id) => `/keywords/delete/${id}`
    },

    // Campaigns Management
    campaigns: {
        index: '/campaigns/index',
        create: '/campaigns/create',
        edit: (id) => `/campaigns/edit/${id}`,
        variants: (id) => `/campaigns/variants/${id}`,
        details: (id) => `/campaigns/details/${id}`,
        delete: (id) => `/campaigns/delete/${id}`,
        start: (id) => `/campaigns/start/${id}`,
        pause: (id) => `/campaigns/pause/${id}`
    },

    // Contacts Management
    contacts: {
        index: '/contacts/index',
        create: '/contacts/create',
        details: (id) => `/contacts/details/${id}`,
        groups: '/contacts/groups',
        delete: (id) => `/contacts/delete/${id}`,
        addToGroup: '/contacts/addtogroup',
        removeFromGroup: '/contacts/removefromgroup'
    },

    // Messages Management
    messages: {
        index: '/messages/index',
        compose: '/messages/compose',
        details: (id) => `/messages/details/${id}`,
        preview: '/messages/preview',
        send: (id) => `/messages/send/${id}`
    },

    // Templates Management
    templates: {
        index: '/templates/index',
        create: '/templates/create',
        edit: (id) => `/templates/edit/${id}`,
        preview: (id) => `/templates/preview/${id}`,
        delete: (id) => `/templates/delete/${id}`,
        get: (id) => `/templates/get/${id}`
    },

    // Analytics & Reporting
    analytics: {
        index: '/analytics/index',
        campaigns: '/analytics/campaigns',
        reports: '/analytics/reports',
        export: '/analytics/export'
    },

    // Workflows Management
    workflows: {
        index: '/workflows/index',
        create: '/workflows/create',
        edit: (id) => `/workflows/edit/${id}`,
        details: (id) => `/workflows/details/${id}`,
        delete: (id) => `/workflows/delete/${id}`,
        execute: (id) => `/workflows/execute/${id}`
    },

    // Users Management
    users: {
        index: '/users/index',
        dashboard: '/users/dashboard',
        profile: '/users/profile',
        settings: '/users/settings',
        details: (id) => `/users/details/${id}`,
        edit: (id) => `/users/edit/${id}`,
        delete: (id) => `/users/delete/${id}`
    },

    // Roles Management
    roles: {
        index: '/roles/index',
        create: '/roles/create',
        edit: (id) => `/roles/edit/${id}`,
        details: (id) => `/roles/details/${id}`,
        delete: (id) => `/roles/delete/${id}`,
        togglePermission: '/roles/togglepermission'
    },

    // Settings
    settings: {
        index: '/settings/index',
        integrations: '/settings/integrations',
        compliance: '/settings/compliance'
    },

    // Billing
    billing: {
        index: '/billing/index',
        plans: '/billing/plans',
        history: '/billing/history',
        subscription: '/billing/subscription',
        subscribe: (planId) => `/billing/subscribe/${planId || ''}`,
        cancel: '/billing/cancel',
        invoice: (id) => `/billing/invoice/${id}`,
        pay: (id) => `/billing/pay/${id}`
    },

    // Providers
    providers: {
        index: '/providers/index',
        create: '/providers/create',
        edit: (id) => `/providers/edit/${id}`,
        health: '/providers/health',
        routing: '/providers/routing',
        delete: (id) => `/providers/delete/${id}`,
        test: (id) => `/providers/test/${id}`,
        toggle: (id) => `/providers/toggle/${id}`
    },

    // Suppression Lists
    suppression: {
        index: '/suppression/index',
        create: '/suppression/create',
        entries: (id) => `/suppression/entries/${id}`,
        import: '/suppression/import',
        edit: (id) => `/suppression/edit/${id}`,
        delete: (id) => `/suppression/delete/${id}`
    },

    // Pricing Management
    pricing: {
        index: '/pricing/index',
        create: '/pricing/create',
        edit: (id) => `/pricing/edit/${id}`,
        channels: (modelId) => `/pricing/channels/${modelId}`,
        delete: (id) => `/pricing/delete/${id}`
    },

    // Webhooks Management
    webhooks: {
        index: '/webhooks/index',
        create: '/webhooks/create',
        edit: (id) => `/webhooks/edit/${id}`,
        delete: (id) => `/webhooks/delete/${id}`,
        test: (id) => `/webhooks/test/${id}`,
        logs: (id) => `/webhooks/logs/${id}`
    },

    // URL Management
    urls: {
        index: '/urls/index',
        create: '/urls/create',
        edit: (id) => `/urls/edit/${id}`,
        delete: (id) => `/urls/delete/${id}`,
        analytics: (id) => `/urls/analytics/${id}`
    },

    // Super Admin
    superAdmin: {
        auditLog: (id) => `/superadmin/auditlog/${id}`
    },

    // API Endpoints
    api: {
        base: '/api',
        
        // Keywords API
        keywords: {
            list: '/api/keywords',
            get: (id) => `/api/keywords/${id}`,
            create: '/api/keywords',
            update: (id) => `/api/keywords/${id}`,
            delete: (id) => `/api/keywords/${id}`,
            checkAvailability: (keyword) => `/api/keywords/check-availability?keywordText=${encodeURIComponent(keyword)}`,
            activities: (id) => `/api/keywords/${id}/activities`,
            analytics: (id) => `/api/keywords/${id}/analytics`
        },

        // Campaigns API
        campaigns: {
            list: '/api/campaigns',
            get: (id) => `/api/campaigns/${id}`,
            create: '/api/campaigns',
            update: (id) => `/api/campaigns/${id}`,
            delete: (id) => `/api/campaigns/${id}`
        },

        // Contacts API
        contacts: {
            list: '/api/contacts',
            get: (id) => `/api/contacts/${id}`,
            create: '/api/contacts',
            update: (id) => `/api/contacts/${id}`,
            delete: (id) => `/api/contacts/${id}`
        },

        // Messages API
        messages: {
            list: '/api/messages',
            get: (id) => `/api/messages/${id}`,
            send: '/api/messages/send',
            preview: '/api/messages/preview'
        },

        // Templates API
        templates: {
            list: '/api/templates',
            get: (id) => `/api/templates/${id}`,
            create: '/api/templates',
            update: (id) => `/api/templates/${id}`,
            delete: (id) => `/api/templates/${id}`
        },

        // Analytics API
        analytics: {
            dashboard: '/api/analytics/dashboard',
            campaigns: '/api/analytics/campaigns',
            keywords: '/api/analytics/keywords'
        },

        // Auth API
        auth: {
            login: '/api/auth/login',
            register: '/api/auth/register',
            logout: '/api/auth/logout',
            refreshToken: '/api/auth/refresh-token',
            forgotPassword: '/api/auth/forgot-password',
            resetPassword: '/api/auth/reset-password'
        }
    }
};

// Helper function to build URL with query parameters
AppUrls.buildUrl = function(baseUrl, params = {}) {
    const url = new URL(baseUrl, window.location.origin);
    Object.keys(params).forEach(key => {
        if (params[key] !== null && params[key] !== undefined) {
            url.searchParams.append(key, params[key]);
        }
    });
    return url.pathname + url.search;
};

// Helper function to get API base URL from configuration
AppUrls.getApiBaseUrl = function() {
    return window.keywordsConfig?.apiBaseUrl || 
           window.appConfig?.apiBaseUrl || 
           AppUrls.api.base;
};

// Make it available globally
if (typeof window !== 'undefined') {
    window.AppUrls = AppUrls;
}

// Export for module systems
if (typeof module !== 'undefined' && module.exports) {
    module.exports = AppUrls;
}
