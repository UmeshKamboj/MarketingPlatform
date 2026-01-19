# Compliance & Consent Management API Documentation

## Overview
The Compliance & Consent Management system provides comprehensive tools for managing user consent, tracking compliance, enforcing quiet hours, and maintaining detailed audit logs. The system supports channel-specific consent (SMS, MMS, Email) with full automation capabilities.

## Features

### 1. Channel-Specific Consent Management
- Track opt-in/opt-out separately for SMS, MMS, and Email channels
- Record consent dates for each channel
- Support multiple consent sources (WebForm, API, Import, Keyword, Manual, Campaign)
- Historical tracking of all consent changes

### 2. Compliance Audit Logging
- Comprehensive logging of all compliance actions
- Track IP addresses and user agents for consent changes
- Link audit logs to contacts and campaigns
- Filter logs by action type, channel, or contact

### 3. Quiet Hours Enforcement
- Configurable quiet hours with start/end times
- Timezone-aware enforcement (IANA timezone support)
- Calculate next allowed send time
- Handles quiet hours spanning midnight

### 4. Suppression List Integration
- Automatic enforcement at compliance check level
- Check both phone numbers and email addresses
- Auto-add to suppression on opt-out keywords
- Auto-remove from suppression on opt-in keywords

### 5. Keyword-Based Automation
- Configurable opt-out keywords (default: STOP, UNSUBSCRIBE, CANCEL, END, QUIT)
- Configurable opt-in keywords (default: START, SUBSCRIBE, YES, JOIN)
- Custom confirmation messages
- Automatic suppression list management

## API Endpoints

### Consent Management

#### Get Contact Consent Status
```http
GET /api/compliance/contacts/{contactId}/consent-status
Authorization: Bearer {token}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "contactId": 1,
    "contactName": "John Doe",
    "phoneNumber": "+1234567890",
    "email": "john@example.com",
    "smsOptIn": true,
    "mmsOptIn": false,
    "emailOptIn": true,
    "smsOptInDate": "2026-01-15T10:00:00Z",
    "mmsOptInDate": null,
    "emailOptInDate": "2026-01-10T14:30:00Z"
  }
}
```

#### Record Consent (Opt-In or Opt-Out)
```http
POST /api/compliance/consent
Authorization: Bearer {token}
Content-Type: application/json

{
  "contactId": 1,
  "channel": 0,
  "source": 0,
  "consentGiven": true,
  "ipAddress": "192.168.1.1",
  "userAgent": "Mozilla/5.0...",
  "notes": "User opted in via web form"
}
```

**Channel Values:**
- 0 = SMS
- 1 = MMS
- 2 = Email
- 3 = All

**Source Values:**
- 0 = WebForm
- 1 = API
- 2 = Import
- 3 = Keyword
- 4 = Manual
- 5 = Campaign

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "contactId": 1,
    "channel": 0,
    "status": 0,
    "source": 0,
    "consentDate": "2026-01-18T17:30:00Z",
    "revokedDate": null,
    "ipAddress": "192.168.1.1",
    "notes": "User opted in via web form"
  },
  "message": "Consent recorded successfully"
}
```

#### Bulk Record Consent
```http
POST /api/compliance/consent/bulk
Authorization: Bearer {token}
Content-Type: application/json

{
  "contactIds": [1, 2, 3, 4, 5],
  "channel": 0,
  "source": 4,
  "consentGiven": true,
  "ipAddress": "192.168.1.1",
  "notes": "Bulk opt-in for promotional campaign"
}
```

**Response:**
```json
{
  "success": true,
  "data": 5,
  "message": "Consent recorded for 5 contacts"
}
```

#### Revoke Consent (Opt-Out)
```http
POST /api/compliance/contacts/{contactId}/revoke-consent?channel=0&reason=User%20request
Authorization: Bearer {token}
```

**Response:**
```json
{
  "success": true,
  "data": true,
  "message": "Consent revoked successfully"
}
```

#### Get Contact Consents (Paginated)
```http
GET /api/compliance/contacts/{contactId}/consents?pageNumber=1&pageSize=20
Authorization: Bearer {token}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": 5,
        "contactId": 1,
        "channel": 0,
        "status": 1,
        "source": 3,
        "consentDate": "2026-01-18T16:00:00Z",
        "revokedDate": "2026-01-18T17:00:00Z",
        "ipAddress": null,
        "notes": "Opted out via STOP keyword"
      }
    ],
    "pageNumber": 1,
    "pageSize": 20,
    "totalCount": 1,
    "totalPages": 1
  }
}
```

#### Get Consent History (Paginated)
```http
GET /api/compliance/contacts/{contactId}/consent-history?pageNumber=1&pageSize=20
Authorization: Bearer {token}
```

### Compliance Settings

#### Get Compliance Settings
```http
GET /api/compliance/settings
Authorization: Bearer {token}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "userId": "user-guid",
    "requireDoubleOptIn": false,
    "requireDoubleOptInSms": false,
    "requireDoubleOptInEmail": true,
    "enableQuietHours": true,
    "quietHoursStart": "22:00:00",
    "quietHoursEnd": "08:00:00",
    "quietHoursTimeZone": "America/New_York",
    "companyName": "Acme Corp",
    "companyAddress": "123 Main St, New York, NY",
    "privacyPolicyUrl": "https://example.com/privacy",
    "termsOfServiceUrl": "https://example.com/terms",
    "optOutKeywords": "STOP,UNSUBSCRIBE,CANCEL,END,QUIT",
    "optInKeywords": "START,SUBSCRIBE,YES,JOIN",
    "optOutConfirmationMessage": "You have been unsubscribed. Reply START to opt back in.",
    "optInConfirmationMessage": "You have been subscribed. Reply STOP to opt out.",
    "enforceSuppressionList": true,
    "enableConsentTracking": true,
    "enableAuditLogging": true,
    "consentRetentionDays": 2555
  }
}
```

#### Update Compliance Settings
```http
PUT /api/compliance/settings
Authorization: Bearer {token}
Content-Type: application/json

{
  "requireDoubleOptIn": false,
  "requireDoubleOptInSms": true,
  "requireDoubleOptInEmail": true,
  "enableQuietHours": true,
  "quietHoursStart": "21:00:00",
  "quietHoursEnd": "09:00:00",
  "quietHoursTimeZone": "America/Los_Angeles",
  "companyName": "My Company",
  "companyAddress": "456 Business Ave",
  "privacyPolicyUrl": "https://mycompany.com/privacy",
  "termsOfServiceUrl": "https://mycompany.com/terms",
  "optOutKeywords": "STOP,UNSUBSCRIBE,CANCEL",
  "optInKeywords": "START,SUBSCRIBE",
  "optOutConfirmationMessage": "Unsubscribed. Text START to rejoin.",
  "optInConfirmationMessage": "Subscribed! Text STOP to opt out.",
  "enforceSuppressionList": true,
  "enableConsentTracking": true,
  "enableAuditLogging": true,
  "consentRetentionDays": 2555
}
```

### Compliance Checks

#### Check Compliance for Contact
```http
GET /api/compliance/contacts/{contactId}/check?channel=0&campaignId=5
Authorization: Bearer {token}
```

**Response (Compliant):**
```json
{
  "success": true,
  "data": {
    "isCompliant": true,
    "violations": [],
    "isSuppressed": false,
    "hasConsent": true,
    "isQuietHoursViolation": false,
    "message": "Contact is compliant for messaging"
  }
}
```

**Response (Non-Compliant):**
```json
{
  "success": true,
  "data": {
    "isCompliant": false,
    "violations": [
      "Contact has not opted in for SMS",
      "Contact is on the suppression list",
      "Current time is within quiet hours (22:00:00 - 08:00:00)"
    ],
    "isSuppressed": true,
    "hasConsent": false,
    "isQuietHoursViolation": true,
    "message": "Compliance violations: Contact has not opted in for SMS; Contact is on the suppression list; Current time is within quiet hours (22:00:00 - 08:00:00)"
  }
}
```

#### Check Quiet Hours
```http
GET /api/compliance/quiet-hours/check
Authorization: Bearer {token}
```

Or specify a future time:
```http
GET /api/compliance/quiet-hours/check?sendTime=2026-01-19T03:00:00Z
Authorization: Bearer {token}
```

**Response (In Quiet Hours):**
```json
{
  "success": true,
  "data": {
    "isQuietHours": true,
    "message": "Current time 23:30 is within quiet hours (22:00 - 08:00)",
    "quietHoursStart": "22:00:00",
    "quietHoursEnd": "08:00:00",
    "nextAllowedTime": "2026-01-19T08:00:00Z"
  }
}
```

**Response (Not in Quiet Hours):**
```json
{
  "success": true,
  "data": {
    "isQuietHours": false,
    "message": "Not in quiet hours",
    "quietHoursStart": "22:00:00",
    "quietHoursEnd": "08:00:00",
    "nextAllowedTime": null
  }
}
```

#### Check If Contact Is Suppressed
```http
GET /api/compliance/check-suppression?phoneOrEmail=%2B1234567890
Authorization: Bearer {token}
```

**Response:**
```json
{
  "success": true,
  "data": false
}
```

#### Filter Compliant Contacts
```http
POST /api/compliance/filter-compliant?channel=0&campaignId=5
Authorization: Bearer {token}
Content-Type: application/json

[1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
```

**Response:**
```json
{
  "success": true,
  "data": [1, 3, 5, 7, 9],
  "message": "5 compliant contacts"
}
```

### Audit Logs

#### Get Audit Logs
```http
GET /api/compliance/audit-logs?pageNumber=1&pageSize=20
Authorization: Bearer {token}
```

With filters:
```http
GET /api/compliance/audit-logs?pageNumber=1&pageSize=20&actionType=0&channel=0&contactId=1
Authorization: Bearer {token}
```

**Action Type Values:**
- 0 = OptIn
- 1 = OptOut
- 2 = ConsentGiven
- 3 = ConsentRevoked
- 4 = SuppressionAdded
- 5 = SuppressionRemoved
- 6 = QuietHoursViolation
- 7 = ComplianceCheck

**Response:**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": 42,
        "userId": "user-guid",
        "contactId": 1,
        "campaignId": 5,
        "actionType": 0,
        "channel": 0,
        "description": "Contact opted in for SMS",
        "ipAddress": "192.168.1.1",
        "actionDate": "2026-01-18T17:30:00Z",
        "contactName": "John Doe",
        "campaignName": "Summer Sale"
      }
    ],
    "pageNumber": 1,
    "pageSize": 20,
    "totalCount": 1,
    "totalPages": 1
  }
}
```

### Keyword Processing

#### Process Opt-Out Keyword
```http
POST /api/compliance/contacts/{contactId}/process-optout?keyword=STOP&channel=0
Authorization: Bearer {token}
```

**Response (Keyword Recognized):**
```json
{
  "success": true,
  "data": true,
  "message": "You have been unsubscribed. Reply START to opt back in."
}
```

**Response (Keyword Not Recognized):**
```json
{
  "success": false,
  "message": "Keyword not recognized as opt-out"
}
```

#### Process Opt-In Keyword
```http
POST /api/compliance/contacts/{contactId}/process-optin?keyword=START&channel=0
Authorization: Bearer {token}
```

**Response (Keyword Recognized):**
```json
{
  "success": true,
  "data": true,
  "message": "You have been subscribed. Reply STOP to opt out."
}
```

## Integration Examples

### Example 1: Pre-Campaign Compliance Check
Before sending a campaign, filter contacts for compliance:

```javascript
// Get all contacts for campaign
const contactIds = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

// Filter for compliant contacts
const response = await fetch('/api/compliance/filter-compliant?channel=0&campaignId=5', {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify(contactIds)
});

const result = await response.json();
const compliantContactIds = result.data; // [1, 3, 5, 7, 9]

// Send campaign only to compliant contacts
```

### Example 2: Processing Inbound SMS
When receiving an inbound SMS, check for opt-out keywords:

```javascript
const inboundMessage = "STOP";
const fromPhone = "+1234567890";

// Find contact by phone
const contact = await findContactByPhone(fromPhone);

// Process opt-out keyword
const response = await fetch(`/api/compliance/contacts/${contact.id}/process-optout?keyword=${inboundMessage}&channel=0`, {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`
  }
});

const result = await response.json();
if (result.success && result.data) {
  // Send confirmation message
  await sendSMS(fromPhone, result.message);
}
```

### Example 3: Web Form Opt-In
Record consent when user submits web form:

```javascript
const formData = {
  contactId: 123,
  channel: 2, // Email
  source: 0, // WebForm
  consentGiven: true,
  ipAddress: request.ip,
  userAgent: request.headers['user-agent'],
  notes: "Newsletter signup form"
};

const response = await fetch('/api/compliance/consent', {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify(formData)
});
```

### Example 4: Quiet Hours Check Before Sending
Before scheduling a message, check quiet hours:

```javascript
const sendTime = new Date('2026-01-19T03:00:00Z');

const response = await fetch(`/api/compliance/quiet-hours/check?sendTime=${sendTime.toISOString()}`, {
  headers: {
    'Authorization': `Bearer ${token}`
  }
});

const result = await response.json();
if (result.data.isQuietHours) {
  console.log(`Cannot send now. Next allowed time: ${result.data.nextAllowedTime}`);
  // Schedule for next allowed time
  scheduleSend(result.data.nextAllowedTime);
} else {
  // Send immediately
  sendNow();
}
```

## Best Practices

### 1. Always Check Compliance
Before sending any message, always perform a compliance check:
```javascript
const check = await checkCompliance(contactId, channel, campaignId);
if (!check.isCompliant) {
  console.log('Compliance violations:', check.violations);
  return; // Don't send
}
```

### 2. Record All Consent Changes
Ensure all consent changes are recorded with proper source tracking:
```javascript
await recordConsent({
  contactId,
  channel,
  source: 'WebForm', // or API, Import, etc.
  consentGiven: true,
  ipAddress: userIp,
  notes: 'Context about this consent change'
});
```

### 3. Honor Opt-Out Immediately
Process opt-out keywords in real-time:
```javascript
if (await processOptOutKeyword(contactId, keyword, channel)) {
  // Send confirmation
  // Stop all pending messages to this contact
}
```

### 4. Implement Double Opt-In When Required
For channels requiring double opt-in:
```javascript
// Step 1: Send confirmation message
await sendConfirmationMessage(contact);

// Step 2: Wait for confirmation response
// Step 3: Only then record final consent
await recordConsent({ ...consentData, consentGiven: true });
```

### 5. Regular Audit Log Reviews
Periodically review audit logs for compliance issues:
```javascript
const logs = await getAuditLogs({
  pageNumber: 1,
  pageSize: 100,
  actionType: 'ComplianceCheck',
  // Review all compliance checks
});
```

### 6. Keep Suppression List Updated
Ensure suppression list is integrated with consent management:
- Auto-add on opt-out
- Auto-remove on opt-in
- Regular reconciliation

### 7. Timezone Awareness
Always use timezone-aware quiet hours:
```javascript
await updateSettings({
  enableQuietHours: true,
  quietHoursStart: '22:00:00',
  quietHoursEnd: '08:00:00',
  quietHoursTimeZone: 'America/New_York' // IANA timezone
});
```

## Compliance Requirements by Region

### United States - TCPA Compliance
- Require explicit opt-in for SMS/MMS
- Honor opt-out keywords (STOP, UNSUBSCRIBE, etc.)
- Respect quiet hours (9 PM - 8 AM local time)
- Maintain consent records for 4+ years

### European Union - GDPR Compliance
- Obtain explicit consent with clear purpose
- Allow easy withdrawal of consent
- Maintain detailed audit trail
- Honor data retention limits
- Provide consent history to users

### Canada - CASL Compliance
- Require explicit opt-in for commercial messages
- Include unsubscribe mechanism in every message
- Maintain consent records indefinitely
- Track all consent sources

## Error Handling

All API endpoints return standardized error responses:

```json
{
  "success": false,
  "message": "Error description",
  "errors": ["Detailed error 1", "Detailed error 2"]
}
```

Common HTTP status codes:
- 200: Success
- 400: Bad Request (validation error)
- 401: Unauthorized (missing or invalid token)
- 404: Not Found (contact or resource not found)
- 500: Internal Server Error

## Support

For questions or issues with the Compliance & Consent Management system, please contact:
- Email: compliance@marketingplatform.com
- Documentation: https://docs.marketingplatform.com/compliance
