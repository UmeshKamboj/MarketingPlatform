# JavaScript Externalization Guidelines

## Overview
This document describes the conventions and best practices established during the JavaScript externalization refactoring of the MarketingPlatform.

## File Organization

### External JavaScript Files
- **Location**: `/wwwroot/js/`
- **Naming Convention**: `[area]-[page].js`
- **Examples**:
  - `messages-details.js` (Messages/Details.cshtml)
  - `campaigns-variants.js` (Campaigns/Variants.cshtml)
  - `pricing-create.js` (Pricing/Create.cshtml)

### View Files
- **Location**: `/Views/[Area]/[Page].cshtml`
- **Script Section Format**:
```html
@section Scripts {
    <!-- External library scripts (if needed) -->
    <script src="https://cdn.example.com/library.js"></script>
    
    <!-- Configuration only -->
    <script nonce="@Context.GetCspNonce()">
        window.[pageNameCamelCase]Config = {
            apiBaseUrl: '@ViewBag.ApiBaseUrl',
            customKey: '@ViewBag.CustomValue'
        };
    </script>
    
    <!-- External page-specific script -->
    <script src="~/js/[area]-[page].js" asp-append-version="true"></script>
}
```

## Event Handling

### Modern Approach (Required)
Use data attributes and event listeners instead of inline onclick handlers:

```html
<!-- ✅ CORRECT -->
<button class="btn btn-primary" data-action="save">Save</button>
<script>
    document.querySelector('[data-action="save"]').addEventListener('click', saveItem);
</script>

<!-- ❌ INCORRECT -->
<button class="btn btn-primary" onclick="saveItem()">Save</button>
```

### Data Attribute Naming
- **Primary action**: `data-action="action-name"` (kebab-case)
- **Related IDs**: `data-item-id="123"`, `data-plan-id="456"` (kebab-case)
- **Other attributes**: `data-[name]="value"`

### Event Delegation
For dynamically generated elements, use event delegation:

```javascript
// ✅ CORRECT - Event delegation
document.addEventListener('click', function(e) {
    if (e.target.closest('[data-action="delete"]')) {
        deleteItem(e.target.closest('[data-action="delete"]').dataset.itemId);
    }
});

// ❌ INCORRECT - Won't work for dynamic elements
document.querySelector('[data-action="delete"]').addEventListener('click', deleteItem);
```

## Notification System

### Using showNotification()
Replace all `alert()` calls with `showNotification()` from `/wwwroot/js/common.js`:

```javascript
// ✅ CORRECT
showNotification('Operation successful!', 'success');
showNotification('Please fill in all fields', 'warning');
showNotification('An error occurred', 'error');
showNotification('Processing...', 'info');

// ❌ INCORRECT
alert('Operation successful!');
```

### Notification Types
- **'success'**: For successful operations (green)
- **'error'**: For errors (red)
- **'warning'**: For warnings (orange)
- **'info'**: For informational messages (blue)

### Default Duration
- Auto-dismisses after 5000ms
- User can manually dismiss with close button

## Configuration Management

### Window Config Objects
All page configuration should be in a window object:

```javascript
// In .cshtml file
window.messagesDetailsConfig = {
    apiBaseUrl: '@ViewBag.ApiBaseUrl',
    messageId: parseInt('@ViewBag.MessageId'),
    canDelete: @ViewBag.CanDelete.ToString().ToLower()
};
```

### Accessing Configuration
In external JavaScript:

```javascript
const apiBaseUrl = window.messagesDetailsConfig?.apiBaseUrl || '';
const messageId = window.messagesDetailsConfig?.messageId || 0;
```

## API Integration

### Modern Fetch API (Preferred)
```javascript
fetch(url, {
    method: 'GET',
    headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
    }
})
.then(response => response.json())
.then(data => {
    if (data.success) {
        showNotification('Success!', 'success');
    }
})
.catch(error => {
    console.error('Error:', error);
    showNotification('An error occurred', 'error');
});
```

### jQuery AJAX (Legacy, Being Phased Out)
```javascript
$.ajax({
    url: url,
    method: 'GET',
    headers: getAjaxHeaders(),
    success: function(response) {
        if (response.success) {
            showNotification('Success!', 'success');
        }
    },
    error: function(xhr) {
        showNotification('Error: ' + xhr.statusText, 'error');
    }
});
```

## DOM Ready Handling

### Recommended Pattern
```javascript
document.addEventListener('DOMContentLoaded', function() {
    initialize();
});

function initialize() {
    // Bind event listeners
    setupEventListeners();
    // Load initial data
    loadData();
}

function setupEventListeners() {
    // Your event binding code
}
```

### Alternative Pattern (IIFE)
```javascript
(function() {
    'use strict';
    
    function initialize() {
        // Your code
    }
    
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initialize);
    } else {
        initialize();
    }
})();
```

## Cache Busting

### Required for All External Scripts
```html
<script src="~/js/page-specific.js" asp-append-version="true"></script>
```

The `asp-append-version="true"` attribute adds a hash query parameter that changes when the file is modified, ensuring browsers fetch the latest version.

## Error Handling

### Proper Try-Catch Pattern
```javascript
async function loadData() {
    try {
        const response = await fetch('/api/data');
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        const data = await response.json();
        displayData(data);
    } catch (error) {
        console.error('Failed to load data:', error);
        showNotification('Failed to load data', 'error');
    }
}
```

## Security Considerations

### XSS Prevention
- Use `escapeHtml()` from common.js for user-generated content
- Never use `.innerHTML` with unsanitized user data
- Prefer `.textContent` when possible

```javascript
// ✅ CORRECT
element.textContent = userInput;
element.innerHTML = escapeHtml(userInput);

// ❌ INCORRECT
element.innerHTML = userInput; // XSS vulnerability!
```

### CSP Compliance
- All inline scripts with logic must be in external files
- Only config and nonce'd scripts allowed in `<script>` tags
- External scripts are not subject to CSP restrictions (loaded from same domain)

## Common.js Utilities

### Available Functions
- `showNotification(message, type, duration)` - Display toast notification
- `escapeHtml(text)` - Escape HTML for XSS prevention
- `formatDate(dateStr)` - Format date for display
- `formatNumber(num)` - Format number with thousands separator
- `getAuthToken()` - Get stored auth token
- `getAjaxHeaders()` - Get headers with auth token

See `/wwwroot/js/common.js` for complete list and documentation.

## Bootstrap Integration

### Modals
```javascript
const modal = new bootstrap.Modal(document.getElementById('myModal'));
modal.show();
modal.hide();
```

### Tooltips & Popovers
These are automatically initialized by common.js for elements with:
- `data-bs-toggle="tooltip"`
- `data-bs-toggle="popover"`

## Code Quality Standards

### Required Patterns
- ✅ Use 'use strict' mode in IIFE
- ✅ Use const/let instead of var
- ✅ Use arrow functions for callbacks
- ✅ Use async/await instead of .then() chains
- ✅ Use template literals for string concatenation
- ✅ Use event delegation for dynamic content

### Code Organization
1. Configuration section (imports, constants)
2. Initialization function
3. Event setup functions
4. Business logic functions
5. Helper/utility functions

## Testing Guidelines

### Unit Testing
- External JavaScript files should be testable independently
- Avoid global state where possible
- Use dependency injection for configuration

### Integration Testing
- Ensure event listeners work correctly
- Test with dynamically generated elements
- Verify API integration

## Migration Checklist

When externalization an existing inline script:

- [ ] Create new external JS file with naming convention
- [ ] Extract all functions to external file
- [ ] Replace all onclick attributes with data-action attributes
- [ ] Replace all alert() calls with showNotification()
- [ ] Create/update config object in inline script
- [ ] Setup event listeners for all buttons/links
- [ ] Add script reference with asp-append-version="true"
- [ ] Test all functionality
- [ ] Verify no onclick attributes remain
- [ ] Verify no alert() calls remain
- [ ] Test with CSP enabled

## Examples

### Complete Example: Simple Page with Actions
```html
<!-- View File: Views/Admin/Settings.cshtml -->
<button class="btn btn-primary" data-action="save-settings">Save</button>
<button class="btn btn-secondary" data-action="reset-settings">Reset</button>

@section Scripts {
    <script nonce="@Context.GetCspNonce()">
        window.adminSettingsConfig = {
            apiBaseUrl: '@ViewBag.ApiBaseUrl'
        };
    </script>
    <script src="~/js/admin-settings.js" asp-append-version="true"></script>
}
```

```javascript
// External File: wwwroot/js/admin-settings.js
document.addEventListener('DOMContentLoaded', function() {
    setupEventListeners();
});

function setupEventListeners() {
    const saveBtn = document.querySelector('[data-action="save-settings"]');
    if (saveBtn) {
        saveBtn.addEventListener('click', saveSettings);
    }
    
    const resetBtn = document.querySelector('[data-action="reset-settings"]');
    if (resetBtn) {
        resetBtn.addEventListener('click', resetSettings);
    }
}

async function saveSettings() {
    try {
        const apiBaseUrl = window.adminSettingsConfig?.apiBaseUrl || '';
        const response = await fetch(`${apiBaseUrl}/api/settings`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${getAuthToken()}`
            },
            body: JSON.stringify({ /* settings */ })
        });
        
        if (response.ok) {
            showNotification('Settings saved successfully!', 'success');
        } else {
            throw new Error('Failed to save settings');
        }
    } catch (error) {
        console.error('Error:', error);
        showNotification('Failed to save settings', 'error');
    }
}

function resetSettings() {
    if (confirm('Reset all settings to defaults?')) {
        showNotification('Settings reset!', 'info');
        // Reset logic here
    }
}
```

## References

- **CSP Documentation**: `/docs/CSP_IMPLEMENTATION.md`
- **Common.js**: `/wwwroot/js/common.js`
- **Bootstrap Integration**: https://getbootstrap.com/docs/
- **Fetch API**: https://developer.mozilla.org/en-US/docs/Web/API/Fetch_API

## FAQ

**Q: Should I use jQuery AJAX or Fetch API?**
A: Prefer Fetch API for new code. jQuery AJAX is legacy and being phased out.

**Q: What if I need to access the event object?**
A: Event listeners receive the event as their first parameter:
```javascript
btn.addEventListener('click', function(event) {
    event.preventDefault();
    // Use event object
});
```

**Q: How do I handle dynamic elements?**
A: Use event delegation on the document or a parent container that exists at page load time.

**Q: Can I use inline scripts for complex logic?**
A: No. Inline scripts should contain only configuration. All logic must be in external files for maintainability, testability, and CSP compliance.

## Revision History

- **2024-01**: Initial guidelines established during JavaScript externalization refactoring
  - 9 .cshtml files refactored
  - External JavaScript files created for all pages
  - Consistent patterns established across codebase
