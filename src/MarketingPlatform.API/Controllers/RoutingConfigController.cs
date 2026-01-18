using MarketingPlatform.Application.DTOs.Common;
using MarketingPlatform.Core.Entities;
using MarketingPlatform.Core.Enums;
using MarketingPlatform.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketingPlatform.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RoutingConfigController : ControllerBase
    {
        private readonly IRepository<ChannelRoutingConfig> _routingConfigRepository;
        private readonly IRepository<MessageDeliveryAttempt> _deliveryAttemptRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RoutingConfigController> _logger;

        public RoutingConfigController(
            IRepository<ChannelRoutingConfig> routingConfigRepository,
            IRepository<MessageDeliveryAttempt> deliveryAttemptRepository,
            IUnitOfWork unitOfWork,
            ILogger<RoutingConfigController> logger)
        {
            _routingConfigRepository = routingConfigRepository;
            _deliveryAttemptRepository = deliveryAttemptRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Get all routing configurations
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<List<ChannelRoutingConfig>>>> GetAllConfigs()
        {
            var configs = await _routingConfigRepository.GetQueryable()
                .OrderBy(c => c.Channel)
                .ThenByDescending(c => c.Priority)
                .ToListAsync();

            return Ok(ApiResponse<List<ChannelRoutingConfig>>.SuccessResponse(
                configs, 
                "Routing configurations retrieved successfully"));
        }

        /// <summary>
        /// Get routing configuration by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<ChannelRoutingConfig>>> GetConfigById(int id)
        {
            var config = await _routingConfigRepository.GetByIdAsync(id);
            
            if (config == null)
                return NotFound(ApiResponse<ChannelRoutingConfig>.ErrorResponse("Routing configuration not found"));

            return Ok(ApiResponse<ChannelRoutingConfig>.SuccessResponse(
                config, 
                "Routing configuration retrieved successfully"));
        }

        /// <summary>
        /// Get routing configuration by channel
        /// </summary>
        [HttpGet("channel/{channel}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<ChannelRoutingConfig>>> GetConfigByChannel(ChannelType channel)
        {
            var config = await _routingConfigRepository.GetQueryable()
                .Where(c => c.Channel == channel && c.IsActive)
                .OrderByDescending(c => c.Priority)
                .FirstOrDefaultAsync();

            if (config == null)
                return NotFound(ApiResponse<ChannelRoutingConfig>.ErrorResponse(
                    $"No active routing configuration found for channel: {channel}"));

            return Ok(ApiResponse<ChannelRoutingConfig>.SuccessResponse(
                config, 
                "Routing configuration retrieved successfully"));
        }

        /// <summary>
        /// Create a new routing configuration
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<ChannelRoutingConfig>>> CreateConfig(
            [FromBody] ChannelRoutingConfig config)
        {
            try
            {
                config.CreatedAt = DateTime.UtcNow;
                await _routingConfigRepository.AddAsync(config);
                await _unitOfWork.SaveChangesAsync();

                return Ok(ApiResponse<ChannelRoutingConfig>.SuccessResponse(
                    config, 
                    "Routing configuration created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating routing configuration");
                return StatusCode(500, ApiResponse<ChannelRoutingConfig>.ErrorResponse(
                    "An error occurred while creating the routing configuration"));
            }
        }

        /// <summary>
        /// Update an existing routing configuration
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<ChannelRoutingConfig>>> UpdateConfig(
            int id, 
            [FromBody] ChannelRoutingConfig updatedConfig)
        {
            try
            {
                var config = await _routingConfigRepository.GetByIdAsync(id);
                
                if (config == null)
                    return NotFound(ApiResponse<ChannelRoutingConfig>.ErrorResponse(
                        "Routing configuration not found"));

                // Update properties
                config.PrimaryProvider = updatedConfig.PrimaryProvider;
                config.FallbackProvider = updatedConfig.FallbackProvider;
                config.RoutingStrategy = updatedConfig.RoutingStrategy;
                config.EnableFallback = updatedConfig.EnableFallback;
                config.MaxRetries = updatedConfig.MaxRetries;
                config.RetryStrategy = updatedConfig.RetryStrategy;
                config.InitialRetryDelaySeconds = updatedConfig.InitialRetryDelaySeconds;
                config.MaxRetryDelaySeconds = updatedConfig.MaxRetryDelaySeconds;
                config.CostThreshold = updatedConfig.CostThreshold;
                config.IsActive = updatedConfig.IsActive;
                config.Priority = updatedConfig.Priority;
                config.AdditionalSettings = updatedConfig.AdditionalSettings;
                config.UpdatedAt = DateTime.UtcNow;

                _routingConfigRepository.Update(config);
                await _unitOfWork.SaveChangesAsync();

                return Ok(ApiResponse<ChannelRoutingConfig>.SuccessResponse(
                    config, 
                    "Routing configuration updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating routing configuration {ConfigId}", id);
                return StatusCode(500, ApiResponse<ChannelRoutingConfig>.ErrorResponse(
                    "An error occurred while updating the routing configuration"));
            }
        }

        /// <summary>
        /// Delete a routing configuration
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteConfig(int id)
        {
            try
            {
                var config = await _routingConfigRepository.GetByIdAsync(id);
                
                if (config == null)
                    return NotFound(ApiResponse<bool>.ErrorResponse("Routing configuration not found"));

                _routingConfigRepository.Remove(config);
                await _unitOfWork.SaveChangesAsync();

                return Ok(ApiResponse<bool>.SuccessResponse(
                    true, 
                    "Routing configuration deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting routing configuration {ConfigId}", id);
                return StatusCode(500, ApiResponse<bool>.ErrorResponse(
                    "An error occurred while deleting the routing configuration"));
            }
        }

        /// <summary>
        /// Get delivery attempts for a message
        /// </summary>
        [HttpGet("delivery-attempts/{messageId}")]
        public async Task<ActionResult<ApiResponse<List<MessageDeliveryAttempt>>>> GetDeliveryAttempts(int messageId)
        {
            var attempts = await _deliveryAttemptRepository.GetQueryable()
                .Where(a => a.CampaignMessageId == messageId)
                .OrderBy(a => a.AttemptNumber)
                .ToListAsync();

            return Ok(ApiResponse<List<MessageDeliveryAttempt>>.SuccessResponse(
                attempts, 
                "Delivery attempts retrieved successfully"));
        }

        /// <summary>
        /// Get delivery statistics by channel
        /// </summary>
        [HttpGet("stats/channel/{channel}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> GetChannelStats(
            ChannelType channel,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            startDate ??= DateTime.UtcNow.AddDays(-30);
            endDate ??= DateTime.UtcNow;

            var attempts = await _deliveryAttemptRepository.GetQueryable()
                .Where(a => a.Channel == channel && 
                           a.AttemptedAt >= startDate && 
                           a.AttemptedAt <= endDate)
                .ToListAsync();

            var stats = new
            {
                Channel = channel.ToString(),
                TotalAttempts = attempts.Count,
                SuccessfulAttempts = attempts.Count(a => a.Success),
                FailedAttempts = attempts.Count(a => !a.Success),
                SuccessRate = attempts.Any() ? (decimal)attempts.Count(a => a.Success) / attempts.Count * 100 : 0,
                AverageResponseTimeMs = attempts.Any() ? attempts.Average(a => a.ResponseTimeMs) : 0,
                TotalCost = attempts.Where(a => a.CostAmount.HasValue).Sum(a => a.CostAmount.Value),
                FallbackCount = attempts.Count(a => a.FallbackReason.HasValue),
                Period = new { StartDate = startDate, EndDate = endDate }
            };

            return Ok(ApiResponse<object>.SuccessResponse(
                stats, 
                "Channel statistics retrieved successfully"));
        }

        /// <summary>
        /// Get overall delivery statistics
        /// </summary>
        [HttpGet("stats/overall")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> GetOverallStats(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            startDate ??= DateTime.UtcNow.AddDays(-30);
            endDate ??= DateTime.UtcNow;

            var attempts = await _deliveryAttemptRepository.GetQueryable()
                .Where(a => a.AttemptedAt >= startDate && a.AttemptedAt <= endDate)
                .ToListAsync();

            var stats = new
            {
                TotalAttempts = attempts.Count,
                SuccessfulAttempts = attempts.Count(a => a.Success),
                FailedAttempts = attempts.Count(a => !a.Success),
                SuccessRate = attempts.Any() ? (decimal)attempts.Count(a => a.Success) / attempts.Count * 100 : 0,
                AverageResponseTimeMs = attempts.Any() ? attempts.Average(a => a.ResponseTimeMs) : 0,
                TotalCost = attempts.Where(a => a.CostAmount.HasValue).Sum(a => a.CostAmount.Value),
                FallbackCount = attempts.Count(a => a.FallbackReason.HasValue),
                ByChannel = attempts.GroupBy(a => a.Channel).Select(g => new
                {
                    Channel = g.Key.ToString(),
                    TotalAttempts = g.Count(),
                    SuccessfulAttempts = g.Count(a => a.Success),
                    SuccessRate = (decimal)g.Count(a => a.Success) / g.Count() * 100
                }),
                Period = new { StartDate = startDate, EndDate = endDate }
            };

            return Ok(ApiResponse<object>.SuccessResponse(
                stats, 
                "Overall statistics retrieved successfully"));
        }
    }
}
