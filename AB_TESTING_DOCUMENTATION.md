# A/B Testing for Campaigns - Feature Documentation

## Overview
The A/B Testing feature allows users to create, run, and analyze multiple message/content variations for SMS, MMS, and Email campaigns. This enables data-driven decision making by comparing the performance of different campaign variants.

## Key Features

### 1. Variant Management
- **Create Multiple Variants**: Create up to unlimited variants for a single campaign
- **Control Variant**: Designate one variant as the control group for comparison
- **Traffic Allocation**: Distribute recipients across variants based on percentage allocation
- **Variant Configuration**: Each variant can have unique:
  - Subject line (for Email)
  - Message body
  - HTML content (for Email)
  - Media URLs (for MMS)
  - Personalization tokens

### 2. Analytics & Reporting
Each variant tracks the following metrics:
- **Delivery Metrics**:
  - Total Sent
  - Total Delivered
  - Total Failed
  - Delivery Rate

- **Engagement Metrics**:
  - Total Clicks
  - Click Rate (CTR)
  - Total Opens (Email)
  - Open Rate (Email)
  - Total Replies
  - Total Conversions
  - Conversion Rate

- **Quality Metrics**:
  - Total Opt-outs
  - Opt-out Rate
  - Total Bounces
  - Bounce Rate

- **Statistical Analysis**:
  - Confidence Level
  - Statistical Significance Indicator

### 3. Winner Selection
- **Automatic Recommendations**: System suggests the best-performing variant based on:
  - Click-through rate (weighted 60%)
  - Delivery rate (weighted 40%)
  - Conversion rate
- **Manual Selection**: Users can manually select a winning variant
- **Comparison Dashboard**: Side-by-side comparison of all variants

## Database Schema

### CampaignVariant Table
Stores individual variant configurations:
- **Id**: Primary key
- **CampaignId**: Foreign key to Campaign
- **Name**: Variant name
- **Description**: Optional description
- **TrafficPercentage**: Percentage of recipients to receive this variant (0-100)
- **IsControl**: Indicates if this is the control variant
- **IsActive**: Whether the variant is active
- **Channel**: Communication channel (SMS/MMS/Email)
- **Subject**: Subject line (for Email)
- **MessageBody**: Message content
- **HTMLContent**: HTML content (for Email)
- **MediaUrls**: JSON array of media URLs
- **MessageTemplateId**: Optional template reference
- **PersonalizationTokens**: JSON dictionary of personalization tokens
- **RecipientCount**: Number of recipients assigned to this variant
- **SentCount**: Number of messages sent
- **DeliveredCount**: Number of messages delivered
- **FailedCount**: Number of failed messages

### CampaignVariantAnalytics Table
Stores analytics data for each variant:
- **Id**: Primary key
- **CampaignVariantId**: Foreign key to CampaignVariant
- All metrics listed in Analytics section above

### Campaign Table Updates
New fields added:
- **IsABTest**: Boolean flag indicating if campaign uses A/B testing
- **WinningVariantId**: ID of the selected winning variant (nullable)
- **ABTestEndDate**: Optional end date for the A/B test

### CampaignMessage Table Updates
New field added:
- **VariantId**: Foreign key to CampaignVariant (nullable)

## API Endpoints

### Variant CRUD Operations

#### Get All Variants for a Campaign
```
GET /api/campaigns/{campaignId}/variants
```
Returns list of all variants for the specified campaign.

#### Get Specific Variant
```
GET /api/campaigns/{campaignId}/variants/{variantId}
```
Returns details of a specific variant including analytics.

#### Create Variant
```
POST /api/campaigns/{campaignId}/variants
Content-Type: application/json

{
  "name": "Variant A",
  "description": "Test variant with emphasis on urgency",
  "trafficPercentage": 50,
  "isControl": false,
  "channel": 0,
  "subject": "Limited Time Offer!",
  "messageBody": "Don't miss out! Sale ends today!",
  "htmlContent": "<html>...</html>",
  "mediaUrls": "[\"https://example.com/image.jpg\"]",
  "personalizationTokens": "{\"firstName\": \"John\"}"
}
```

#### Update Variant
```
PUT /api/campaigns/{campaignId}/variants/{variantId}
Content-Type: application/json

{
  "name": "Updated Variant A",
  "description": "Updated description",
  "trafficPercentage": 45,
  "isActive": true,
  "subject": "Updated Subject",
  "messageBody": "Updated message body",
  "htmlContent": "<html>...</html>",
  "mediaUrls": "[\"https://example.com/image2.jpg\"]",
  "personalizationTokens": "{\"firstName\": \"Jane\"}"
}
```

#### Delete Variant
```
DELETE /api/campaigns/{campaignId}/variants/{variantId}
```
Soft deletes the variant (only in Draft status).

#### Activate Variant
```
POST /api/campaigns/{campaignId}/variants/{variantId}/activate
```

#### Deactivate Variant
```
POST /api/campaigns/{campaignId}/variants/{variantId}/deactivate
```

### Analytics & Comparison

#### Compare Variants
```
GET /api/campaigns/{campaignId}/variants/comparison
```
Returns comparison data and recommended winner.

Response:
```json
{
  "success": true,
  "data": {
    "variants": [...],
    "winningVariant": {...},
    "recommendedAction": "Variant 'B' shows 5.25% improvement in click rate over control. Consider selecting as winner."
  }
}
```

#### Select Winning Variant
```
POST /api/campaigns/{campaignId}/variants/{variantId}/select-winner
```
Marks the specified variant as the winner.

## Web UI Components

### Variant Management Page
URL: `/Campaigns/Variants/{campaignId}`

Features:
- Traffic allocation summary with visual indicator
- List of all variants with key metrics
- Create new variant modal
- Edit/Delete variant actions
- Performance comparison table
- Winner selection interface

### Campaign Creation Enhancement
- Checkbox to enable A/B testing during campaign creation
- After creating an A/B test campaign, users are directed to variant management

### Campaign List Enhancement
- Displays A/B test indicator badge for campaigns with variants
- Shows winning variant (if selected) on campaign card

## Usage Workflow

### Creating an A/B Test Campaign

1. **Create Campaign**:
   - Go to Campaigns > Create New Campaign
   - Fill in basic information
   - Check "Enable A/B Testing for this campaign"
   - Complete content, audience, and schedule configuration
   - Save campaign (status: Draft)

2. **Create Variants**:
   - Navigate to campaign detail or variants page
   - Click "Add Variant"
   - Configure variant with:
     - Name and description
     - Traffic percentage
     - Control designation
     - Message content
   - Repeat for additional variants
   - Ensure total traffic allocation = 100%

3. **Launch Campaign**:
   - Review all variants
   - Verify traffic allocation
   - Start campaign
   - System automatically distributes recipients to variants based on traffic percentages

4. **Monitor Performance**:
   - View real-time analytics for each variant
   - Compare performance metrics
   - Review system recommendations

5. **Select Winner**:
   - Review comparison dashboard
   - Select winning variant
   - Optionally use winning variant for future campaigns

## Service Layer

### CampaignABTestingService
Key methods:
- `CreateVariantAsync`: Creates a new variant
- `GetVariantByIdAsync`: Retrieves variant details
- `GetCampaignVariantsAsync`: Lists all variants for a campaign
- `UpdateVariantAsync`: Updates variant configuration
- `DeleteVariantAsync`: Soft deletes a variant
- `SelectVariantForRecipientAsync`: Randomly selects variant based on traffic allocation
- `UpdateVariantAnalyticsAsync`: Recalculates variant analytics
- `CompareVariantsAsync`: Compares all variants and suggests winner
- `SelectWinningVariantAsync`: Marks variant as winner
- `ValidateVariantTrafficAllocationAsync`: Ensures traffic adds up to 100%

## Integration Points

### Message Sending Integration
When sending messages for A/B test campaigns:
1. For each recipient, call `SelectVariantForRecipientAsync()`
2. Use returned variant's content for message
3. Associate message with variant by setting `CampaignMessage.VariantId`

### Analytics Integration
- Variant analytics are updated when messages are sent/delivered
- Call `UpdateVariantAnalyticsAsync()` periodically or after status changes
- Analytics dashboard shows variant-specific metrics

## Best Practices

1. **Traffic Allocation**:
   - Ensure total traffic allocation equals 100%
   - Allocate at least 10-20% to each variant for statistical significance
   - Control variant should typically receive 30-50% of traffic

2. **Variant Design**:
   - Test one variable at a time for clearer results
   - Use descriptive names for variants
   - Document what each variant tests

3. **Sample Size**:
   - Ensure sufficient sample size (minimum 100+ recipients per variant)
   - Longer test duration increases confidence

4. **Winner Selection**:
   - Wait for statistical significance before selecting winner
   - Consider multiple metrics, not just one
   - Review confidence level before making decision

## Future Enhancements

Potential improvements for future versions:
1. Multi-variate testing (testing multiple variables simultaneously)
2. Automatic winner selection based on configurable criteria
3. Scheduled A/B test duration with automatic winner selection
4. Advanced statistical analysis (p-values, confidence intervals)
5. A/B test templates for common scenarios
6. Integration with external analytics platforms
7. A/B test history and learnings repository

## Security Considerations

- All API endpoints require authentication
- Users can only access variants for their own campaigns
- Variants can only be modified when campaign is in Draft status
- Soft delete prevents accidental data loss
- Input validation on all traffic percentages and content fields

## Performance Considerations

- Variant selection uses efficient random allocation algorithm
- Analytics are calculated asynchronously
- Indexes on CampaignId for fast variant lookup
- Soft delete with query filters for optimal performance
