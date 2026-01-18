namespace MarketingPlatform.Application.Interfaces
{
    public interface IRateLimitService
    {
        /// <summary>
        /// Check if a message can be sent to a contact based on frequency control
        /// </summary>
        Task<bool> CanSendMessageAsync(int contactId, string userId);

        /// <summary>
        /// Record that a message was sent to a contact
        /// </summary>
        Task RecordMessageSentAsync(int contactId, string userId);

        /// <summary>
        /// Get frequency control settings for a contact
        /// </summary>
        Task<Core.Entities.FrequencyControl?> GetFrequencyControlAsync(int contactId, string userId);

        /// <summary>
        /// Update frequency control settings for a contact
        /// </summary>
        Task UpdateFrequencyControlAsync(int contactId, string userId, int maxPerDay, int maxPerWeek, int maxPerMonth);

        /// <summary>
        /// Reset daily counters for all contacts (scheduled job)
        /// </summary>
        Task ResetDailyCountersAsync();

        /// <summary>
        /// Reset weekly counters for all contacts (scheduled job)
        /// </summary>
        Task ResetWeeklyCountersAsync();

        /// <summary>
        /// Reset monthly counters for all contacts (scheduled job)
        /// </summary>
        Task ResetMonthlyCountersAsync();
    }
}
