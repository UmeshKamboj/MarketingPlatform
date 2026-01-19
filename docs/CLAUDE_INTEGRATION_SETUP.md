# Claude Integration Setup Guide

## Overview
Claude AI has been integrated into your MarketingPlatform project. This guide explains how to set up and use the Claude service.

## Prerequisites

1. **Anthropic API Key**: Get your API key from [https://console.anthropic.com](https://console.anthropic.com)

## Installation & Setup

### Step 1: Set Environment Variable
Add your Anthropic API key as an environment variable:

**Windows (PowerShell):**
```powershell
$env:ANTHROPIC_API_KEY = "your-api-key-here"
```

**Windows (Command Prompt):**
```cmd
set ANTHROPIC_API_KEY=your-api-key-here
```

**For Development (appsettings.Development.json):**
You can also add it to your appsettings file, but **DO NOT commit to source control**:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "ANTHROPIC_API_KEY": "your-api-key-here"
}
```

### Step 2: Build the Project
```bash
dotnet build
```

### Step 3: Restore NuGet Packages
The Anthropic.SDK NuGet package will be automatically restored when building.

## Usage

### Available Endpoints

#### 1. Generate Text
**POST** `/api/claude/generate-text`

Request:
```json
{
  "prompt": "Write a professional email subject line for a marketing campaign",
  "maxTokens": 1024
}
```

#### 2. Analyze Content
**POST** `/api/claude/analyze-content`

Request:
```json
{
  "content": "Your SMS message here",
  "analysisType": "SMS engagement potential"
}
```

#### 3. Generate Ideas
**POST** `/api/claude/generate-ideas`

Request:
```json
{
  "topic": "summer product launch",
  "count": 5
}
```

#### 4. Proofread Content
**POST** `/api/claude/proofread`

Request:
```json
{
  "content": "Your content to proofread here"
}
```

#### 5. Translate Content
**POST** `/api/claude/translate`

Request:
```json
{
  "content": "English content to translate",
  "targetLanguage": "Spanish"
}
```

#### 6. Generate Campaign Copy
**POST** `/api/claude/generate-campaign-copy`

Request:
```json
{
  "campaignTopic": "Black Friday Sale",
  "targetAudience": "Tech-savvy millennials aged 25-35"
}
```

### Using in Services

Inject the `IClaudeService` into your services:

```csharp
public class MyService
{
    private readonly IClaudeService _claudeService;

    public MyService(IClaudeService claudeService)
    {
        _claudeService = claudeService;
    }

    public async Task<string> CreateCampaignContent()
    {
        return await _claudeService.GenerateCampaignCopyAsync(
            "Product Launch", 
            "Enterprise customers"
        );
    }
}
```

## Project Structure

- **IClaudeService**: Interface definition
- **ClaudeService**: Implementation in `MarketingPlatform.Application/Services/ClaudeService.cs`
- **ClaudeController**: API endpoints in `MarketingPlatform.API/Controllers/ClaudeController.cs`
- **Program.cs**: Dependency injection configuration

## Models

The integration uses `claude-3-5-sonnet-20241022` by default, which is:
- Fast and cost-effective
- Good for most tasks
- Suitable for production workloads

You can change the model constant in `ClaudeService.cs`:
```csharp
private const string Model = "claude-3-5-sonnet-20241022";
```

## Error Handling

All methods include error logging. If an error occurs:

1. Check that `ANTHROPIC_API_KEY` is properly set
2. Verify your API key has available credits
3. Check application logs in the `Logs/` folder
4. Ensure the model name is valid

## Best Practices

1. **API Key Security**: Never commit API keys to source control
2. **Rate Limiting**: The Claude API has rate limits; implement caching for repeated requests
3. **Token Usage**: Monitor token usage in your Anthropic dashboard
4. **Error Handling**: Always wrap Claude calls in try-catch blocks
5. **Logging**: Use the provided logging to track usage and errors

## Cost Estimation

Token pricing (as of your integration date):
- Input: ~$0.003 per 1K tokens
- Output: ~$0.015 per 1K tokens

Monitor your usage in the [Anthropic Console](https://console.anthropic.com)

## Troubleshooting

| Issue | Solution |
|-------|----------|
| "ANTHROPIC_API_KEY environment variable not set" | Set the environment variable as shown in Step 1 |
| HTTP 401 Unauthorized | Verify your API key is valid and has credits |
| Slow responses | Check network connection and Anthropic service status |
| Token limit exceeded | Reduce `maxTokens` parameter or break request into smaller parts |

## Next Steps

1. Test the endpoints using Swagger UI
2. Integrate Claude into your campaign creation workflow
3. Use it for content analysis and optimization
4. Monitor usage and costs in the Anthropic dashboard

## Documentation

- [Anthropic API Docs](https://docs.anthropic.com)
- [Claude Models](https://docs.anthropic.com/en/docs/models/overview)
- [Your Integration Code](./ClaudeService.cs)
