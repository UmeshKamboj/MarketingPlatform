# ShowOnLanding Feature Documentation

## Overview

The `ShowOnLanding` flag is a boolean property on the `SubscriptionPlan` entity that controls whether a subscription plan is displayed on the public landing page of the Marketing Platform.

## Purpose

This feature allows administrators to:
- Control which subscription plans appear on the public-facing landing page
- Hide internal or special plans from public view while keeping them active
- Dynamically manage landing page content without code changes
- Display only selected plans to potential customers

## Implementation Details

### Database Schema

**Entity:** `SubscriptionPlan`
```csharp
public bool ShowOnLanding { get; set; } = true;
```

**Default Value:** `true` (plans are shown on landing page by default)

### API Endpoints

#### Get Plans for Landing Page
```
GET /api/subscriptionplans/landing
Authorization: Not required (public endpoint)
```

Returns only plans where:
- `IsActive = true`
- `IsVisible = true`
- `ShowOnLanding = true`
- `IsDeleted = false`

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "name": "Free",
      "description": "Perfect for trying out the platform",
      "priceMonthly": 0.00,
      "priceYearly": 0.00,
      "features": { ... },
      "isActive": true,
      "isVisible": true,
      "showOnLanding": true
    }
  ]
}
```

#### Toggle ShowOnLanding Flag
```
PUT /api/subscriptionplans/{id}/show-on-landing
Authorization: Bearer {token} (SuperAdmin role required)
Content-Type: application/json
Body: true or false
```

**Example:**
```bash
curl -X PUT https://localhost:7001/api/subscriptionplans/1/show-on-landing \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d "true"
```

**Response:**
```json
{
  "success": true,
  "message": "Plan will now be displayed on the landing page"
}
```

### Seeded Data

All three default subscription plans are configured to show on the landing page:

| Plan | Price | ShowOnLanding |
|------|-------|---------------|
| Free | $0/month | ✅ true |
| Pro | $49.99/month | ✅ true |
| Enterprise | $199.99/month | ✅ true |

## Usage Scenarios

### Scenario 1: Hiding Plans Temporarily
Hide a plan from the landing page during maintenance or updates:

```bash
PUT /api/subscriptionplans/2/show-on-landing
Body: false
```

The plan remains active and existing subscribers are unaffected.

### Scenario 2: Internal Plans
Create special plans for internal use or partnerships that shouldn't be publicly visible:

```json
{
  "name": "Partner Plan",
  "priceMonthly": 29.99,
  "isActive": true,
  "isVisible": true,
  "showOnLanding": false
}
```

### Scenario 3: Seasonal Promotions
Show special promotional plans only during specific periods:

1. Create promotional plan with `showOnLanding: false`
2. When promotion starts: `PUT /api/subscriptionplans/{id}/show-on-landing` → `true`
3. When promotion ends: `PUT /api/subscriptionplans/{id}/show-on-landing` → `false`

### Scenario 4: A/B Testing
Test different plan configurations:

- Version A: Show Plans 1, 2, 3 on landing
- Version B: Show Plans 1, 4, 5 on landing

Toggle plans without code deployment.

## Frontend Integration

### Landing Page Display

The landing page (`/Home/Index`) automatically fetches and displays plans:

```javascript
async function loadPricingPlans() {
    const response = await fetch(`${apiBaseUrl}/subscriptionplans/landing`);
    const result = await response.json();
    
    if (result.success && result.data.length > 0) {
        renderPricingPlans(result.data);
    }
}
```

### Admin Management UI

SuperAdmins can manage the ShowOnLanding flag through:
- Subscription Plans Management page
- Toggle button for each plan
- Bulk operations for multiple plans

## Permissions

### Who Can Modify
- **SuperAdmin** role only

### Who Can View
- Landing page endpoint is **public** (no authentication required)
- Anyone can see plans where `ShowOnLanding = true`

## Best Practices

### ✅ DO:
- Keep at least one plan visible on landing page
- Use descriptive plan names for better user experience
- Test landing page after toggling plans
- Document reasons for hiding plans

### ❌ DON'T:
- Hide all plans (landing page would show empty)
- Frequently change visibility (confuses potential customers)
- Hide popular plans without alternative
- Use this for access control (use IsActive/IsVisible instead)

## Difference Between Flags

| Flag | Purpose | Affects | Visibility |
|------|---------|---------|------------|
| `IsActive` | Plan is available for subscription | New subscriptions | Hidden if false |
| `IsVisible` | Plan appears in plan listings | Subscription page, APIs | Hidden if false |
| `ShowOnLanding` | Plan appears on public landing page | Landing page only | Still visible in other places |
| `IsDeleted` | Soft delete | All operations | Hidden everywhere |

## Testing

### Manual Testing

1. **View Landing Page**
   ```
   Navigate to: https://localhost:7002/
   Scroll to Pricing Section
   Verify visible plans match ShowOnLanding = true
   ```

2. **Toggle Plan Visibility**
   ```
   Login as SuperAdmin
   Navigate to: /Pricing
   Toggle ShowOnLanding for a plan
   Refresh landing page
   Verify plan appears/disappears
   ```

3. **API Testing**
   ```bash
   # Get landing page plans
   curl https://localhost:7001/api/subscriptionplans/landing
   
   # Toggle plan (requires auth)
   curl -X PUT https://localhost:7001/api/subscriptionplans/1/show-on-landing \
     -H "Authorization: Bearer TOKEN" \
     -H "Content-Type: application/json" \
     -d "false"
   ```

## Troubleshooting

### Problem: No plans showing on landing page

**Possible Causes:**
1. All plans have `ShowOnLanding = false`
2. Plans are not active (`IsActive = false`)
3. Plans are not visible (`IsVisible = false`)
4. API endpoint is not accessible

**Solution:**
```sql
-- Check plan status
SELECT Id, Name, IsActive, IsVisible, ShowOnLanding 
FROM SubscriptionPlans 
WHERE IsDeleted = 0;

-- Enable ShowOnLanding for a plan
UPDATE SubscriptionPlans 
SET ShowOnLanding = 1 
WHERE Id = 1;
```

### Problem: Plan showing on landing but shouldn't

**Possible Causes:**
1. `ShowOnLanding = true` in database
2. Cache not cleared
3. Using old API endpoint

**Solution:**
1. Verify database value
2. Clear browser cache
3. Use correct endpoint: `/api/subscriptionplans/landing`

## Future Enhancements

Potential improvements for this feature:

1. **Display Order:** Add `LandingPageOrder` field to control plan ordering
2. **Scheduling:** Auto-show/hide plans based on date/time
3. **Geolocation:** Show different plans based on user location
4. **Analytics:** Track which plans are viewed/selected most
5. **A/B Testing:** Built-in testing framework for plan displays

## Related Documentation

- [LOGIN_CREDENTIALS.md](LOGIN_CREDENTIALS.md) - Test user accounts
- [README.md](README.md) - Project setup and configuration
- [SUBSCRIPTION_BILLING_IMPLEMENTATION_SUMMARY.md](SUBSCRIPTION_BILLING_IMPLEMENTATION_SUMMARY.md) - Billing implementation details

## Changelog

- **2026-01-18:** Initial implementation of ShowOnLanding feature
- **2026-01-18:** Added API endpoint for toggling flag
- **2026-01-18:** Updated DTOs to include ShowOnLanding property
- **2026-01-18:** Added comprehensive documentation
