using MarketingPlatform.Core.Entities;
using MarketingPlatform.Core.Enums;

namespace MarketingPlatform.Application.Interfaces
{
    public interface IMessageRoutingService
    {
        /// <summary>
        /// Routes a message through the appropriate channel and provider
        /// </summary>
        Task<(bool Success, string? ExternalId, string? Error, decimal? Cost, int AttemptNumber)> RouteMessageAsync(
            CampaignMessage message);

        /// <summary>
        /// Determines if a message should be retried based on retry strategy
        /// </summary>
        Task<(bool ShouldRetry, int DelaySeconds)> ShouldRetryMessageAsync(CampaignMessage message);

        /// <summary>
        /// Gets the next retry delay based on retry strategy
        /// </summary>
        Task<int> CalculateRetryDelayAsync(int attemptNumber, ChannelType channel);

        /// <summary>
        /// Attempts to send through fallback channel if primary fails
        /// </summary>
        Task<(bool Success, string? ExternalId, string? Error, decimal? Cost, FallbackReason? Reason)> TryFallbackChannelAsync(
            CampaignMessage message, 
            string primaryError);

        /// <summary>
        /// Logs delivery attempt with detailed information
        /// </summary>
        Task LogDeliveryAttemptAsync(
            int campaignMessageId,
            int attemptNumber,
            ChannelType channel,
            string? providerName,
            bool success,
            string? externalId,
            string? error,
            string? errorCode,
            decimal? cost,
            int responseTimeMs,
            FallbackReason? fallbackReason = null);
    }
}
