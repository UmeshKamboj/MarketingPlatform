using MarketingPlatform.Application.Interfaces;
using MarketingPlatform.Core.Entities;
using MarketingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace MarketingPlatform.Application.Services
{
    public class RateLimitService : IRateLimitService
    {
        private readonly IRepository<FrequencyControl> _frequencyControlRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RateLimitService> _logger;

        public RateLimitService(
            IRepository<FrequencyControl> frequencyControlRepository,
            IUnitOfWork unitOfWork,
            ILogger<RateLimitService> logger)
        {
            _frequencyControlRepository = frequencyControlRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> CanSendMessageAsync(int contactId, string userId)
        {
            var control = await GetOrCreateFrequencyControlAsync(contactId, userId);

            // Reset counters if needed
            await ResetCountersIfNeededAsync(control);

            // Check limits
            if (control.MessagesSentToday >= control.MaxMessagesPerDay)
            {
                _logger.LogWarning("Daily limit reached for contact {ContactId}", contactId);
                return false;
            }

            if (control.MessagesSentThisWeek >= control.MaxMessagesPerWeek)
            {
                _logger.LogWarning("Weekly limit reached for contact {ContactId}", contactId);
                return false;
            }

            if (control.MessagesSentThisMonth >= control.MaxMessagesPerMonth)
            {
                _logger.LogWarning("Monthly limit reached for contact {ContactId}", contactId);
                return false;
            }

            return true;
        }

        public async Task RecordMessageSentAsync(int contactId, string userId)
        {
            var control = await GetOrCreateFrequencyControlAsync(contactId, userId);

            // Reset counters if needed
            await ResetCountersIfNeededAsync(control);

            // Increment counters
            control.MessagesSentToday++;
            control.MessagesSentThisWeek++;
            control.MessagesSentThisMonth++;
            control.LastMessageSentAt = DateTime.UtcNow;
            control.UpdatedAt = DateTime.UtcNow;

            _frequencyControlRepository.Update(control);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Recorded message sent for contact {ContactId}. Daily: {Daily}, Weekly: {Weekly}, Monthly: {Monthly}",
                contactId, control.MessagesSentToday, control.MessagesSentThisWeek, control.MessagesSentThisMonth);
        }

        public async Task<FrequencyControl?> GetFrequencyControlAsync(int contactId, string userId)
        {
            return await _frequencyControlRepository.FirstOrDefaultAsync(fc =>
                fc.ContactId == contactId && fc.UserId == userId && !fc.IsDeleted);
        }

        public async Task UpdateFrequencyControlAsync(int contactId, string userId, int maxPerDay, int maxPerWeek, int maxPerMonth)
        {
            var control = await GetOrCreateFrequencyControlAsync(contactId, userId);

            control.MaxMessagesPerDay = maxPerDay;
            control.MaxMessagesPerWeek = maxPerWeek;
            control.MaxMessagesPerMonth = maxPerMonth;
            control.UpdatedAt = DateTime.UtcNow;

            _frequencyControlRepository.Update(control);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Updated frequency control for contact {ContactId}. Daily: {Daily}, Weekly: {Weekly}, Monthly: {Monthly}",
                contactId, maxPerDay, maxPerWeek, maxPerMonth);
        }

        public async Task ResetDailyCountersAsync()
        {
            var controls = await _frequencyControlRepository.FindAsync(fc => !fc.IsDeleted);

            foreach (var control in controls)
            {
                control.MessagesSentToday = 0;
                control.UpdatedAt = DateTime.UtcNow;
                _frequencyControlRepository.Update(control);
            }

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Reset daily counters for {Count} frequency controls", controls.Count());
        }

        public async Task ResetWeeklyCountersAsync()
        {
            var controls = await _frequencyControlRepository.FindAsync(fc => !fc.IsDeleted);

            foreach (var control in controls)
            {
                control.MessagesSentThisWeek = 0;
                control.UpdatedAt = DateTime.UtcNow;
                _frequencyControlRepository.Update(control);
            }

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Reset weekly counters for {Count} frequency controls", controls.Count());
        }

        public async Task ResetMonthlyCountersAsync()
        {
            var controls = await _frequencyControlRepository.FindAsync(fc => !fc.IsDeleted);

            foreach (var control in controls)
            {
                control.MessagesSentThisMonth = 0;
                control.UpdatedAt = DateTime.UtcNow;
                _frequencyControlRepository.Update(control);
            }

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Reset monthly counters for {Count} frequency controls", controls.Count());
        }

        private async Task<FrequencyControl> GetOrCreateFrequencyControlAsync(int contactId, string userId)
        {
            var control = await _frequencyControlRepository.FirstOrDefaultAsync(fc =>
                fc.ContactId == contactId && fc.UserId == userId && !fc.IsDeleted);

            if (control == null)
            {
                control = new FrequencyControl
                {
                    ContactId = contactId,
                    UserId = userId,
                    MaxMessagesPerDay = 5,
                    MaxMessagesPerWeek = 20,
                    MaxMessagesPerMonth = 50,
                    MessagesSentToday = 0,
                    MessagesSentThisWeek = 0,
                    MessagesSentThisMonth = 0,
                    LastMessageSentAt = DateTime.UtcNow
                };

                await _frequencyControlRepository.AddAsync(control);
                await _unitOfWork.SaveChangesAsync();
            }

            return control;
        }

        private async Task ResetCountersIfNeededAsync(FrequencyControl control)
        {
            var now = DateTime.UtcNow;
            var lastSent = control.LastMessageSentAt;
            bool needsUpdate = false;

            // Reset daily counter if it's a new day
            if (lastSent.Date < now.Date)
            {
                control.MessagesSentToday = 0;
                needsUpdate = true;
            }

            // Reset weekly counter if it's a new week (week starts on Monday)
            var lastSentWeekStart = lastSent.AddDays(-(int)lastSent.DayOfWeek + (int)DayOfWeek.Monday);
            var nowWeekStart = now.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday);
            if (lastSentWeekStart.Date < nowWeekStart.Date)
            {
                control.MessagesSentThisWeek = 0;
                needsUpdate = true;
            }

            // Reset monthly counter if it's a new month
            if (lastSent.Year < now.Year || (lastSent.Year == now.Year && lastSent.Month < now.Month))
            {
                control.MessagesSentThisMonth = 0;
                needsUpdate = true;
            }

            if (needsUpdate)
            {
                control.UpdatedAt = DateTime.UtcNow;
                _frequencyControlRepository.Update(control);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
