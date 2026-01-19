# Email & SMS Preview & Testing API Documentation

This document describes the message preview and test send functionality for all message types (SMS, MMS, Email).

## Overview

The preview and testing functionality allows users to:
- Preview message content with variable substitution
- See device-specific rendering (Desktop, Mobile, Tablet for emails; Mobile for SMS/MMS)
- Validate message content (character counts, SMS segments, etc.)
- Send test messages to specific recipients before launching campaigns
- Identify missing variables and potential issues

## API Endpoints

### 1. Preview Message

**Endpoint:** `POST /api/messages/preview`

**Description:** Preview message content with variable substitution and device-specific rendering. This endpoint does not send any actual messages.

**Authorization:** Required (Bearer token)

**Request Body:**
```json
{
  "channel": "Email",
  "campaignId": 123,
  "templateId": 456,
  "subject": "Hello {{FirstName}}, special offer inside!",
  "messageBody": "Dear {{FirstName}} {{LastName}},\n\nWe have a special offer for you...",
  "htmlContent": "<html><body><h1>Hello {{FirstName}}</h1><p>Special offer...</p></body></html>",
  "mediaUrls": ["https://example.com/image.jpg"],
  "variableValues": {
    "FirstName": "John",
    "LastName": "Doe"
  },
  "contactId": 789
}
```

**Request Parameters:**
- `channel` (required): One of "SMS", "MMS", or "Email"
- `campaignId` (optional): Campaign ID for context
- `templateId` (optional): Template ID to use for content
- `subject` (optional): Message subject (for Email only)
- `messageBody` (required): Plain text message body
- `htmlContent` (optional): HTML content (for Email)
- `mediaUrls` (optional): Array of media URLs (for MMS)
- `variableValues` (required): Dictionary of variable name-value pairs for substitution
- `contactId` (optional): Contact ID to use for loading additional variables

**Response:**
```json
{
  "success": true,
  "data": {
    "channel": "Email",
    "subject": "Hello John, special offer inside!",
    "messageBody": "Dear John Doe,\n\nWe have a special offer for you...",
    "htmlContent": "<html><body><h1>Hello John</h1><p>Special offer...</p></body></html>",
    "mediaUrls": ["https://example.com/image.jpg"],
    "devicePreviews": [
      {
        "deviceType": "Desktop",
        "subject": "Hello John, special offer inside!",
        "messageBody": "Dear John Doe,\n\nWe have a special offer for you...",
        "htmlContent": "<html><body><h1>Hello John</h1><p>Special offer...</p></body></html>",
        "characterCount": 55,
        "isTruncated": false,
        "warnings": []
      },
      {
        "deviceType": "Mobile",
        "subject": "Hello John, special offer inside!",
        "messageBody": "Dear John Doe,\n\nWe have a special offer for you...",
        "htmlContent": "<html><body><h1>Hello John</h1><p>Special offer...</p></body></html>",
        "characterCount": 55,
        "isTruncated": true,
        "warnings": ["Subject may be truncated on mobile devices (typically shows ~40 characters)"]
      },
      {
        "deviceType": "Tablet",
        "subject": "Hello John, special offer inside!",
        "messageBody": "Dear John Doe,\n\nWe have a special offer for you...",
        "htmlContent": "<html><body><h1>Hello John</h1><p>Special offer...</p></body></html>",
        "characterCount": 55,
        "isTruncated": false,
        "warnings": []
      }
    ],
    "missingVariables": [],
    "characterCount": 55,
    "smsSegments": null,
    "validationWarnings": [],
    "validationErrors": []
  },
  "message": null,
  "errors": null
}
```

**Response Fields:**
- `channel`: Channel type
- `subject`: Rendered subject line (Email only)
- `messageBody`: Rendered plain text body
- `htmlContent`: Rendered HTML content (Email only)
- `mediaUrls`: Array of media URLs (MMS only)
- `devicePreviews`: Array of device-specific previews
  - `deviceType`: Device type (Desktop, Mobile, Tablet for Email; Mobile for SMS/MMS)
  - `subject`: Rendered subject for this device
  - `messageBody`: Rendered body for this device
  - `htmlContent`: Rendered HTML for this device
  - `characterCount`: Character count
  - `isTruncated`: Whether content is truncated on this device
  - `warnings`: Device-specific warnings
- `missingVariables`: Array of variable names that were not provided
- `characterCount`: Total character count
- `smsSegments`: Number of SMS segments (SMS/MMS only)
- `validationWarnings`: Array of validation warnings
- `validationErrors`: Array of validation errors

**Example for SMS:**
```json
{
  "channel": "SMS",
  "messageBody": "Hi {{FirstName}}, your appointment is at {{Time}}. Reply YES to confirm.",
  "variableValues": {
    "FirstName": "Alice",
    "Time": "3:00 PM"
  }
}
```

Response:
```json
{
  "success": true,
  "data": {
    "channel": "SMS",
    "subject": null,
    "messageBody": "Hi Alice, your appointment is at 3:00 PM. Reply YES to confirm.",
    "htmlContent": null,
    "mediaUrls": null,
    "devicePreviews": [
      {
        "deviceType": "Mobile",
        "subject": null,
        "messageBody": "Hi Alice, your appointment is at 3:00 PM. Reply YES to confirm.",
        "htmlContent": null,
        "characterCount": 62,
        "isTruncated": false,
        "warnings": []
      }
    ],
    "missingVariables": [],
    "characterCount": 62,
    "smsSegments": 1,
    "validationWarnings": [],
    "validationErrors": []
  }
}
```

### 2. Send Test Message

**Endpoint:** `POST /api/messages/test-send`

**Description:** Send test messages to specific recipients. Messages are prefixed with "[TEST]" to indicate they are test messages.

**Authorization:** Required (Bearer token)

**Request Body:**
```json
{
  "channel": "Email",
  "campaignId": 123,
  "templateId": 456,
  "recipients": ["test@example.com", "user@example.com"],
  "subject": "Hello {{FirstName}}, special offer inside!",
  "messageBody": "Dear {{FirstName}} {{LastName}},\n\nWe have a special offer for you...",
  "htmlContent": "<html><body><h1>Hello {{FirstName}}</h1><p>Special offer...</p></body></html>",
  "mediaUrls": ["https://example.com/image.jpg"],
  "variableValues": {
    "FirstName": "Test",
    "LastName": "User"
  },
  "contactId": 789
}
```

**Request Parameters:**
- `channel` (required): One of "SMS", "MMS", or "Email"
- `campaignId` (optional): Campaign ID for context
- `templateId` (optional): Template ID to use for content
- `recipients` (required): Array of recipient addresses (email addresses for Email, phone numbers for SMS/MMS)
- `subject` (optional): Message subject (for Email only)
- `messageBody` (required): Plain text message body
- `htmlContent` (optional): HTML content (for Email)
- `mediaUrls` (optional): Array of media URLs (for MMS)
- `variableValues` (required): Dictionary of variable name-value pairs for substitution
- `contactId` (optional): Contact ID to use for loading additional variables

**Response:**
```json
{
  "success": true,
  "data": {
    "successCount": 2,
    "failureCount": 0,
    "recipients": [
      {
        "recipient": "test@example.com",
        "success": true,
        "errorMessage": null,
        "externalMessageId": "msg_abc123"
      },
      {
        "recipient": "user@example.com",
        "success": true,
        "errorMessage": null,
        "externalMessageId": "msg_def456"
      }
    ],
    "isSuccess": true
  },
  "message": "Test message sent successfully to 2 recipient(s)",
  "errors": null
}
```

**Response Fields:**
- `successCount`: Number of successfully sent test messages
- `failureCount`: Number of failed test messages
- `recipients`: Array of recipient results
  - `recipient`: Recipient address
  - `success`: Whether the send was successful
  - `errorMessage`: Error message if send failed
  - `externalMessageId`: External message ID from provider
- `isSuccess`: Overall success status (true if all sends succeeded)

**Example for SMS Test:**
```json
{
  "channel": "SMS",
  "recipients": ["+1234567890", "+0987654321"],
  "messageBody": "Hi {{FirstName}}, testing SMS delivery.",
  "variableValues": {
    "FirstName": "Test"
  }
}
```

## Variable Substitution

Messages can contain variables in the format `{{VariableName}}`. These will be replaced with values from:
1. `variableValues` dictionary (provided in request)
2. Contact data (if `contactId` is provided)

**Standard Contact Variables:**
- `{{FirstName}}`: Contact's first name
- `{{LastName}}`: Contact's last name
- `{{Email}}`: Contact's email address
- `{{Phone}}` or `{{PhoneNumber}}`: Contact's phone number

## Validation Rules

### SMS/MMS
- **Character Limits:**
  - GSM-7 encoding: 160 chars per segment, 153 for concatenated
  - Unicode encoding: 70 chars per segment, 67 for concatenated
- **Warnings:**
  - Message exceeds 10 SMS segments (1600 characters)
  - Unicode characters detected (reduces segment size)
  - MMS message has no media attachments

### Email
- **Subject Line:** Warning if exceeds 100 characters
- **Subject Truncation:** Mobile devices typically show ~40 characters
- **Warnings:**
  - No subject line
  - Very large email body (>100KB)

## Device-Specific Previews

### Email
1. **Desktop:** Full content rendering
2. **Mobile:** Subject line may be truncated (~40 chars visible)
3. **Tablet:** Full content rendering

### SMS/MMS
1. **Mobile:** Only device type, shows segment count

## Error Handling

**Authentication Errors (401):**
```json
{
  "success": false,
  "data": null,
  "message": "Unauthorized",
  "errors": null
}
```

**Validation Errors (400):**
```json
{
  "success": false,
  "data": null,
  "message": "At least one recipient is required for test send",
  "errors": null
}
```

**Authorization Errors (403):**
```json
{
  "success": false,
  "data": null,
  "message": "Campaign not found or access denied",
  "errors": null
}
```

**Server Errors (500):**
```json
{
  "success": false,
  "data": null,
  "message": "Failed to preview message",
  "errors": null
}
```

## Usage Examples

### Example 1: Preview Email Campaign

```bash
curl -X POST https://api.example.com/api/messages/preview \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "channel": "Email",
    "subject": "Welcome {{FirstName}}!",
    "messageBody": "Hi {{FirstName}}, welcome to our platform!",
    "htmlContent": "<h1>Welcome {{FirstName}}!</h1>",
    "variableValues": {
      "FirstName": "John"
    }
  }'
```

### Example 2: Send Test SMS

```bash
curl -X POST https://api.example.com/api/messages/test-send \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "channel": "SMS",
    "recipients": ["+1234567890"],
    "messageBody": "Test message: Your code is {{Code}}",
    "variableValues": {
      "Code": "123456"
    }
  }'
```

### Example 3: Preview MMS with Media

```bash
curl -X POST https://api.example.com/api/messages/preview \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "channel": "MMS",
    "messageBody": "Check out our new product, {{FirstName}}!",
    "mediaUrls": ["https://example.com/product.jpg"],
    "variableValues": {
      "FirstName": "Alice"
    }
  }'
```

### Example 4: Test Email with Contact Data

```bash
curl -X POST https://api.example.com/api/messages/test-send \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "channel": "Email",
    "recipients": ["test@example.com"],
    "subject": "Hi {{FirstName}} {{LastName}}",
    "messageBody": "Your email is {{Email}} and phone is {{Phone}}",
    "contactId": 123
  }'
```
(Variables will be loaded from contact #123)

## Best Practices

1. **Always preview before launching:** Use the preview endpoint to check rendering and validate content
2. **Test on multiple devices:** Review all device previews for emails to ensure proper display
3. **Check character counts:** For SMS/MMS, monitor segment counts to control costs
4. **Validate variables:** Ensure all required variables are provided to avoid rendering issues
5. **Test send to yourself:** Before campaign launch, send test messages to verify delivery
6. **Monitor warnings:** Address validation warnings before sending to large audiences

## Integration Notes

- Both endpoints require authentication via Bearer token
- Test messages are prefixed with "[TEST]" to distinguish from production messages
- Test sends use actual provider APIs and may incur costs
- Preview endpoint does not send messages and has no cost
- All message types (SMS, MMS, Email) use the same endpoints with different `channel` values
