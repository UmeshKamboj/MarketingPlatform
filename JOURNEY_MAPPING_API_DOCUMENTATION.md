# Contact Journey Mapping (Workflow Designer) API Documentation

## Overview
The Contact Journey Mapping feature enables marketers to visually build, edit, and monitor customer journeys using a workflow designer. This system supports branch logic, triggers, delays, and integrates with campaigns, messages, and the automation engine.

## Architecture
- **Pattern**: Repository and Services Pattern
- **Entities**: `Workflow`, `WorkflowStep`, `WorkflowExecution`
- **Service**: `IWorkflowService` / `WorkflowService`
- **Controller**: `JourneysController`
- **DTOs**: Journey namespace with 6 DTOs

## Key Features
- ✅ Visual workflow/journey designer support
- ✅ Branch logic and conditional workflows
- ✅ Node positioning for visual canvas
- ✅ Multiple trigger types (Event, Schedule, Keyword, Manual)
- ✅ Action types (SMS, MMS, Email, Wait, Add/Remove Tags, Group Management)
- ✅ Journey execution monitoring
- ✅ Statistics and analytics
- ✅ Pause/Resume/Cancel execution control

## API Endpoints

### Base URL
```
/api/journeys
```

All endpoints require authentication with Bearer token.

---

## Journey Management

### 1. Get All Journeys (Paginated)
Get all journeys/workflows for the current user with pagination and search.

**Endpoint:** `GET /api/journeys`

**Query Parameters:**
- `pageNumber` (int, optional): Page number (default: 1)
- `pageSize` (int, optional): Items per page (default: 10)
- `searchTerm` (string, optional): Search in journey name or description

**Response:**
```json
{
  "success": true,
  "message": "Success",
  "data": {
    "items": [
      {
        "id": 1,
        "name": "Welcome Series",
        "description": "Onboarding journey for new customers",
        "triggerType": 0,
        "triggerCriteria": "{\"event\":\"contact_created\"}",
        "isActive": true,
        "createdAt": "2026-01-15T10:00:00Z",
        "updatedAt": "2026-01-15T10:00:00Z",
        "nodes": [
          {
            "id": 1,
            "stepOrder": 1,
            "actionType": 0,
            "actionConfiguration": "{\"messageBody\":\"Welcome!\"}",
            "delayMinutes": null,
            "positionX": 100,
            "positionY": 100,
            "nodeLabel": "Welcome SMS",
            "branchCondition": null,
            "nextNodeOnTrue": null,
            "nextNodeOnFalse": null
          }
        ],
        "totalExecutions": 150,
        "activeExecutions": 12,
        "completedExecutions": 135,
        "failedExecutions": 3
      }
    ],
    "totalCount": 25,
    "pageNumber": 1,
    "pageSize": 10
  }
}
```

**Example Request:**
```bash
curl -X GET "https://localhost:7001/api/journeys?pageNumber=1&pageSize=10&searchTerm=welcome" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

### 2. Get Journey by ID
Retrieve a specific journey with all nodes and statistics.

**Endpoint:** `GET /api/journeys/{id}`

**Path Parameters:**
- `id` (int): Journey ID

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "Welcome Series",
    "description": "Onboarding journey for new customers",
    "triggerType": 0,
    "triggerCriteria": "{\"event\":\"contact_created\"}",
    "isActive": true,
    "createdAt": "2026-01-15T10:00:00Z",
    "updatedAt": "2026-01-15T10:00:00Z",
    "nodes": [...],
    "totalExecutions": 150,
    "activeExecutions": 12,
    "completedExecutions": 135,
    "failedExecutions": 3
  }
}
```

**Example Request:**
```bash
curl -X GET "https://localhost:7001/api/journeys/1" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

### 3. Create Journey
Create a new journey/workflow with nodes.

**Endpoint:** `POST /api/journeys`

**Request Body:**
```json
{
  "name": "Re-engagement Campaign",
  "description": "Win back inactive customers",
  "triggerType": 1,
  "triggerCriteria": "{\"schedule\":\"daily\",\"time\":\"10:00\"}",
  "isActive": false,
  "nodes": [
    {
      "stepOrder": 1,
      "actionType": 0,
      "actionConfiguration": "{\"messageBody\":\"We miss you! Come back for 20% off.\"}",
      "delayMinutes": null,
      "positionX": 100,
      "positionY": 100,
      "nodeLabel": "Initial SMS",
      "branchCondition": null,
      "nextNodeOnTrue": null,
      "nextNodeOnFalse": null
    },
    {
      "stepOrder": 2,
      "actionType": 3,
      "actionConfiguration": null,
      "delayMinutes": 1440,
      "positionX": 100,
      "positionY": 200,
      "nodeLabel": "Wait 1 Day",
      "branchCondition": null,
      "nextNodeOnTrue": null,
      "nextNodeOnFalse": null
    },
    {
      "stepOrder": 3,
      "actionType": 2,
      "actionConfiguration": "{\"subject\":\"Special Offer\",\"messageBody\":\"...\"}",
      "delayMinutes": null,
      "positionX": 100,
      "positionY": 300,
      "nodeLabel": "Follow-up Email",
      "branchCondition": null,
      "nextNodeOnTrue": null,
      "nextNodeOnFalse": null
    }
  ]
}
```

**Response:**
```json
{
  "success": true,
  "message": "Journey created successfully",
  "data": {
    "id": 5,
    "name": "Re-engagement Campaign",
    ...
  }
}
```

**Example Request:**
```bash
curl -X POST "https://localhost:7001/api/journeys" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d @create-journey.json
```

---

### 4. Update Journey
Update an existing journey/workflow.

**Endpoint:** `PUT /api/journeys/{id}`

**Path Parameters:**
- `id` (int): Journey ID

**Request Body:** Same structure as Create Journey

**Response:**
```json
{
  "success": true,
  "message": "Journey updated successfully",
  "data": true
}
```

**Example Request:**
```bash
curl -X PUT "https://localhost:7001/api/journeys/5" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d @update-journey.json
```

---

### 5. Delete Journey
Delete a journey/workflow. Cannot delete journeys with active executions.

**Endpoint:** `DELETE /api/journeys/{id}`

**Path Parameters:**
- `id` (int): Journey ID

**Response:**
```json
{
  "success": true,
  "message": "Journey deleted successfully",
  "data": true
}
```

**Error Response (Active Executions):**
```json
{
  "success": false,
  "message": "Cannot delete journey with active executions. Please stop all running executions first.",
  "errors": []
}
```

**Example Request:**
```bash
curl -X DELETE "https://localhost:7001/api/journeys/5" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

### 6. Duplicate Journey
Create a copy of an existing journey. The copy is created as inactive.

**Endpoint:** `POST /api/journeys/{id}/duplicate`

**Path Parameters:**
- `id` (int): Journey ID to duplicate

**Response:**
```json
{
  "success": true,
  "message": "Journey duplicated successfully",
  "data": {
    "id": 6,
    "name": "Welcome Series (Copy)",
    "isActive": false,
    ...
  }
}
```

**Example Request:**
```bash
curl -X POST "https://localhost:7001/api/journeys/1/duplicate" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

## Journey Analytics

### 7. Get Journey Statistics
Get comprehensive statistics for a journey.

**Endpoint:** `GET /api/journeys/{id}/stats`

**Path Parameters:**
- `id` (int): Journey ID

**Response:**
```json
{
  "success": true,
  "data": {
    "journeyId": 1,
    "journeyName": "Welcome Series",
    "totalExecutions": 150,
    "activeExecutions": 12,
    "completedExecutions": 135,
    "failedExecutions": 3,
    "pausedExecutions": 0,
    "completionRate": 90.0,
    "failureRate": 2.0,
    "averageExecutionTimeMinutes": 125.5
  }
}
```

**Example Request:**
```bash
curl -X GET "https://localhost:7001/api/journeys/1/stats" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

### 8. Get Journey Executions
Get paginated list of journey executions with contact details.

**Endpoint:** `GET /api/journeys/{id}/executions`

**Path Parameters:**
- `id` (int): Journey ID

**Query Parameters:**
- `pageNumber` (int, optional): Page number (default: 1)
- `pageSize` (int, optional): Items per page (default: 10)

**Response:**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": 123,
        "journeyId": 1,
        "journeyName": "Welcome Series",
        "contactId": 456,
        "contactName": "John Doe",
        "contactEmail": "john@example.com",
        "status": 0,
        "currentStepOrder": 2,
        "currentStepName": "Wait 1 Day",
        "startedAt": "2026-01-18T08:00:00Z",
        "completedAt": null,
        "errorMessage": null
      }
    ],
    "totalCount": 150,
    "pageNumber": 1,
    "pageSize": 10
  }
}
```

**Example Request:**
```bash
curl -X GET "https://localhost:7001/api/journeys/1/executions?pageNumber=1&pageSize=20" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

## Journey Execution

### 9. Execute Journey
Start journey execution for a specific contact.

**Endpoint:** `POST /api/journeys/{id}/execute`

**Path Parameters:**
- `id` (int): Journey ID

**Request Body:**
```json
456
```
(Just the contact ID as an integer)

**Response:**
```json
{
  "success": true,
  "message": "Journey execution started",
  "data": true
}
```

**Example Request:**
```bash
curl -X POST "https://localhost:7001/api/journeys/1/execute" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d "456"
```

---

### 10. Pause Execution
Pause a running journey execution.

**Endpoint:** `POST /api/journeys/executions/{executionId}/pause`

**Path Parameters:**
- `executionId` (int): Journey execution ID

**Response:**
```json
{
  "success": true,
  "message": "Journey execution paused",
  "data": true
}
```

**Example Request:**
```bash
curl -X POST "https://localhost:7001/api/journeys/executions/123/pause" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

### 11. Resume Execution
Resume a paused journey execution.

**Endpoint:** `POST /api/journeys/executions/{executionId}/resume`

**Path Parameters:**
- `executionId` (int): Journey execution ID

**Response:**
```json
{
  "success": true,
  "message": "Journey execution resumed",
  "data": true
}
```

**Example Request:**
```bash
curl -X POST "https://localhost:7001/api/journeys/executions/123/resume" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

### 12. Cancel Execution
Cancel a journey execution.

**Endpoint:** `POST /api/journeys/executions/{executionId}/cancel`

**Path Parameters:**
- `executionId` (int): Journey execution ID

**Response:**
```json
{
  "success": true,
  "message": "Journey execution cancelled",
  "data": true
}
```

**Example Request:**
```bash
curl -X POST "https://localhost:7001/api/journeys/executions/123/cancel" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

## Enums

### TriggerType
Defines how a journey is triggered:
```
0 = Event      // Triggered by an event (e.g., contact created)
1 = Schedule   // Triggered on a schedule (e.g., daily, weekly)
2 = Keyword    // Triggered by keyword response
3 = Manual     // Manually triggered
```

### WorkflowActionType
Defines the type of action in a workflow step:
```
0 = SendSMS          // Send SMS message
1 = SendMMS          // Send MMS message
2 = SendEmail        // Send email message
3 = Wait             // Wait/delay for specified minutes
4 = AddToGroup       // Add contact to a group
5 = RemoveFromGroup  // Remove contact from a group
6 = AddTag           // Add tag to contact
```

### WorkflowExecutionStatus
Status of a workflow execution:
```
0 = Running    // Currently executing
1 = Completed  // Successfully completed
2 = Failed     // Failed with error
3 = Paused     // Paused by user
```

---

## Data Models

### Journey Node Properties

**Visual Designer Properties:**
- `positionX` (double?): X coordinate on visual canvas
- `positionY` (double?): Y coordinate on visual canvas
- `nodeLabel` (string?): Human-readable label for the node

**Branch Logic Properties:**
- `branchCondition` (string?): JSON condition for branching logic
- `nextNodeOnTrue` (int?): Step ID to execute if condition is true
- `nextNodeOnFalse` (int?): Step ID to execute if condition is false

**Example Branch Condition:**
```json
{
  "field": "Tag",
  "operator": "In",
  "value": "1,2,3"
}
```

---

## Integration Points

### Campaigns
Journey nodes can trigger campaigns:
```json
{
  "actionType": 0,
  "actionConfiguration": {
    "campaignId": 123,
    "messageBody": "..."
  }
}
```

### Messages
Direct message sending through action configuration:
```json
{
  "actionType": 0,
  "actionConfiguration": {
    "messageBody": "Hello {{FirstName}}!",
    "personalizationTokens": ["FirstName", "LastName"]
  }
}
```

### Analytics
Journey statistics are automatically tracked:
- Total executions
- Active/Completed/Failed counts
- Completion rate
- Average execution time
- Per-step analytics

### Automation Engine
Journeys integrate with Hangfire for:
- Background execution
- Delayed step processing
- Retry logic
- Scheduled triggers

---

## Best Practices

### 1. Journey Design
- Keep journeys focused on a single objective
- Use descriptive node labels
- Test with small groups before full activation
- Include wait steps between message actions

### 2. Node Configuration
- Always set `nodeLabel` for better visualization
- Store positions for visual designer (`positionX`, `positionY`)
- Use appropriate `delayMinutes` to avoid overwhelming contacts

### 3. Branch Logic
- Keep branch conditions simple and testable
- Always define both `nextNodeOnTrue` and `nextNodeOnFalse`
- Use tags or groups for segmentation logic

### 4. Monitoring
- Regularly check journey statistics
- Monitor failure rates
- Review execution logs
- Pause problematic journeys for investigation

### 5. Testing
- Start journeys as inactive (`isActive: false`)
- Test with test contacts first
- Duplicate production journeys for testing
- Use pause/resume for debugging

---

## Error Handling

### Common Errors

**401 Unauthorized**
```json
{
  "success": false,
  "message": "Unauthorized",
  "errors": []
}
```

**404 Not Found**
```json
{
  "success": false,
  "message": "Journey not found",
  "errors": []
}
```

**400 Bad Request - Active Executions**
```json
{
  "success": false,
  "message": "Cannot delete journey with active executions. Please stop all running executions first.",
  "errors": []
}
```

**400 Bad Request - Validation Error**
```json
{
  "success": false,
  "message": "Failed to create journey",
  "errors": ["Journey name is required", "At least one node is required"]
}
```

---

## Security

All journey endpoints require:
1. Valid JWT Bearer token
2. User ownership verification (journeys belong to user)
3. Cannot access other users' journeys
4. Cannot execute journeys for other users' contacts

---

## Database Schema

### WorkflowSteps Table (Enhanced)
```sql
CREATE TABLE WorkflowSteps (
    Id INT PRIMARY KEY IDENTITY,
    WorkflowId INT NOT NULL,
    StepOrder INT NOT NULL,
    ActionType INT NOT NULL,
    ActionConfiguration NVARCHAR(MAX),
    DelayMinutes INT,
    -- Visual Designer Fields
    PositionX FLOAT,
    PositionY FLOAT,
    NodeLabel NVARCHAR(200),
    -- Branch Logic Fields
    BranchCondition NVARCHAR(MAX),
    NextNodeOnTrue INT,
    NextNodeOnFalse INT,
    -- Base Entity Fields
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2,
    IsDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (WorkflowId) REFERENCES Workflows(Id)
);
```

---

## Change Log

### Version 1.0 (January 2026)
- Initial release
- Visual workflow designer support
- Branch logic capabilities
- Journey execution monitoring
- CRUD operations for journeys
- Integration with campaigns and analytics

---

## Support

For questions or issues with the Journey Mapping API:
- Check existing documentation
- Review example requests
- Test with Swagger/OpenAPI interface
- Contact: support@marketingplatform.com
