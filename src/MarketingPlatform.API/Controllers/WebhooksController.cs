using MarketingPlatform.Application.DTOs.Message;
using MarketingPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MarketingPlatform.API.Controllers
{
    [ApiController]
    [Route("api/webhooks")]
    public class WebhooksController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IKeywordService _keywordService;
        private readonly ILogger<WebhooksController> _logger;

        public WebhooksController(
            IMessageService messageService,
            IKeywordService keywordService,
            ILogger<WebhooksController> logger)
        {
            _messageService = messageService;
            _keywordService = keywordService;
            _logger = logger;
        }

        [HttpPost("message-status")]
        public async Task<IActionResult> MessageStatusUpdate([FromBody] MessageStatusUpdateDto dto)
        {
            try
            {
                // TODO: Validate webhook signature from provider
                
                // Find message by ExternalMessageId and update status
                // Note: Using empty userId for webhook updates (system update)
                _logger.LogInformation("Webhook received for message {ExternalId}: {Status}", 
                    dto.ExternalMessageId, dto.Status);

                // For webhook, we need to find the message by ExternalMessageId
                // This requires a service method update or direct repository access
                // For now, log the webhook and return success
                // TODO: Implement UpdateMessageStatusByExternalIdAsync in service

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Webhook processing failed");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("sms-inbound")]
        public async Task<IActionResult> SMSInbound([FromBody] InboundSmsWebhookDto payload)
        {
            try
            {
                // TODO: Validate webhook signature from provider

                _logger.LogInformation("Inbound SMS received from {PhoneNumber}: {Message}", 
                    payload.From, payload.Body);

                // Process keyword if present
                await _keywordService.ProcessInboundKeywordAsync(payload.From, payload.Body);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process inbound SMS");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("sms-delivery")]
        public async Task<IActionResult> SMSDeliveryUpdate([FromBody] dynamic payload)
        {
            // TODO: Parse provider-specific payload format
            // Map to MessageStatusUpdateDto
            // Call UpdateMessageStatusAsync
            
            _logger.LogInformation("SMS delivery webhook received");
            return Ok();
        }

        [HttpPost("email-delivery")]
        public async Task<IActionResult> EmailDeliveryUpdate([FromBody] dynamic payload)
        {
            // TODO: Parse email provider webhook format
            // Handle bounce, delivered, opened, clicked events
            
            _logger.LogInformation("Email delivery webhook received");
            return Ok();
        }
    }

    // DTO for inbound SMS webhook (Twilio format)
    public class InboundSmsWebhookDto
    {
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string MessageSid { get; set; } = string.Empty;
    }
}
