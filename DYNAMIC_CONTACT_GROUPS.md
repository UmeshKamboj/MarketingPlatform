# Dynamic Contact Groups Feature

## Overview

Dynamic Contact Groups is a powerful feature that allows automatic management of contact group memberships based on defined rules and criteria. Unlike static groups where contacts are manually added, dynamic groups automatically update their membership based on contact attributes, tags, location, and engagement metrics.

## Features

### Rule-Based Criteria
Dynamic groups support sophisticated filtering rules based on:

1. **Contact Attributes**
   - Email
   - First Name
   - Last Name
   - Phone Number
   - Country
   - City
   - Postal Code
   - Active Status

2. **Custom Attributes**
   - Any custom field stored in contact's CustomAttributes JSON

3. **Tags**
   - Check if contact has specific tags assigned

4. **Engagement Metrics**
   - Total Messages Sent
   - Total Messages Delivered
   - Total Clicks
   - Engagement Score
   - Last Engagement Date

### Supported Operators

- **String Operations**: Equals, NotEquals, Contains, NotContains, StartsWith, EndsWith, In, NotIn, IsNull, IsNotNull
- **Numeric Operations**: Equals, NotEquals, GreaterThan, LessThan, GreaterThanOrEqual, LessThanOrEqual
- **Boolean Operations**: Equals, NotEquals
- **Date Operations**: Equals, NotEquals, GreaterThan, LessThan, GreaterThanOrEqual, LessThanOrEqual, IsNull, IsNotNull

### Rule Logic

Rules can be combined using:
- **AND** - All rules must match
- **OR** - At least one rule must match

## API Endpoints

### 1. Create Dynamic Group

**POST** `/api/contactgroups`

```json
{
  "name": "High Engagement Customers",
  "description": "Contacts with engagement score > 50",
  "isStatic": false,
  "isDynamic": true,
  "ruleCriteria": {
    "logic": "And",
    "rules": [
      {
        "field": "EngagementScore",
        "operator": "GreaterThan",
        "value": "50"
      },
      {
        "field": "IsActive",
        "operator": "Equals",
        "value": "true"
      }
    ]
  }
}
```

### 2. Update Dynamic Group

**PUT** `/api/contactgroups/{id}`

Updates group details and rules. Automatically re-evaluates membership after update.

### 3. Get Group Details

**GET** `/api/contactgroups/{id}`

Returns group details including rule criteria for dynamic groups.

### 4. Refresh Dynamic Group

**POST** `/api/contactgroups/{id}/refresh`

Manually triggers re-evaluation of group rules and updates membership.

### 5. Refresh All Dynamic Groups

**POST** `/api/contactgroups/refresh-all`

Triggers re-evaluation of all dynamic groups for the authenticated user.

## Example Use Cases

### Example 1: Location-Based Group

Create a group for all contacts in California:

```json
{
  "name": "California Contacts",
  "description": "All contacts located in California",
  "isStatic": false,
  "isDynamic": true,
  "ruleCriteria": {
    "logic": "And",
    "rules": [
      {
        "field": "Country",
        "operator": "Equals",
        "value": "USA"
      },
      {
        "field": "City",
        "operator": "In",
        "value": "Los Angeles,San Francisco,San Diego,Sacramento"
      }
    ]
  }
}
```

### Example 2: Engagement-Based Group

Create a group for highly engaged contacts:

```json
{
  "name": "VIP Engaged Customers",
  "description": "Highly engaged customers with recent activity",
  "isStatic": false,
  "isDynamic": true,
  "ruleCriteria": {
    "logic": "And",
    "rules": [
      {
        "field": "EngagementScore",
        "operator": "GreaterThanOrEqual",
        "value": "75"
      },
      {
        "field": "TotalClicks",
        "operator": "GreaterThan",
        "value": "10"
      },
      {
        "field": "LastEngagementDate",
        "operator": "GreaterThan",
        "value": "2026-01-01"
      }
    ]
  }
}
```

### Example 3: Custom Attribute Group

Create a group based on custom attributes:

```json
{
  "name": "Premium Subscribers",
  "description": "Contacts with premium subscription",
  "isStatic": false,
  "isDynamic": true,
  "ruleCriteria": {
    "logic": "Or",
    "rules": [
      {
        "field": "CustomAttribute",
        "attributeName": "subscription_tier",
        "operator": "Equals",
        "value": "premium"
      },
      {
        "field": "CustomAttribute",
        "attributeName": "subscription_tier",
        "operator": "Equals",
        "value": "enterprise"
      }
    ]
  }
}
```

### Example 4: Tag-Based Group

Create a group for contacts with specific tags:

```json
{
  "name": "Newsletter Subscribers",
  "description": "Contacts tagged for newsletter",
  "isStatic": false,
  "isDynamic": true,
  "ruleCriteria": {
    "logic": "And",
    "rules": [
      {
        "field": "HasTag",
        "operator": "Equals",
        "value": "5"  // Tag ID
      },
      {
        "field": "IsActive",
        "operator": "Equals",
        "value": "true"
      }
    ]
  }
}
```

## Automatic Updates

Dynamic groups are automatically updated in two ways:

1. **On-Demand**: When you manually trigger a refresh via API
2. **Background Service**: A background service runs every 15 minutes to update all dynamic groups

The background service ensures groups stay up-to-date as contact data changes.

## Implementation Details

### Services

- **IDynamicGroupEvaluationService**: Evaluates rules and updates group memberships
- **DynamicGroupUpdateProcessor**: Background service for periodic updates

### Models

- **GroupRuleCriteria**: Container for rule logic and rule list
- **GroupRule**: Individual rule with field, operator, and value
- **RuleField**: Enum of supported fields
- **RuleOperator**: Enum of supported operators
- **RuleLogic**: AND/OR logic for combining rules

### Process Flow

1. User creates/updates dynamic group with rule criteria
2. System serializes rules to JSON and stores in ContactGroup.RuleCriteria
3. System immediately evaluates rules against all user's contacts
4. Matching contacts are added to group
5. Background service periodically re-evaluates all dynamic groups
6. Group memberships are automatically updated as contact data changes

## Best Practices

1. **Keep Rules Simple**: Start with simple rules and add complexity as needed
2. **Test Rules**: Use the refresh endpoint to test rule changes before deploying
3. **Monitor Performance**: Complex rules on large contact lists may take time to evaluate
4. **Use Appropriate Operators**: Choose the right operator for your data type
5. **Combine with Tags**: Use tags for quick categorization combined with dynamic rules

## Validation

The system validates:
- A group cannot be both static and dynamic
- Dynamic groups must have at least one rule
- Rule fields and operators must be valid
- Date and numeric values must be parseable

## Notes

- Dynamic groups update automatically; manual contact management is prevented
- Contact counts are updated automatically during evaluation
- Changes to contact data trigger eventual group membership updates (within 15 minutes)
- Rule evaluation is user-scoped for data isolation
