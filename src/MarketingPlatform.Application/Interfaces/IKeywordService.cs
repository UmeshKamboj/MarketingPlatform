using MarketingPlatform.Application.DTOs.Common;
using MarketingPlatform.Application.DTOs.Keyword;

namespace MarketingPlatform.Application.Interfaces
{
    public interface IKeywordService
    {
        // Basic CRUD operations
        Task<KeywordDto?> GetKeywordByIdAsync(string userId, int keywordId);
        Task<PaginatedResult<KeywordDto>> GetKeywordsAsync(string userId, PagedRequest request);
        Task<List<KeywordDto>> GetActiveKeywordsAsync(string userId);
        Task<KeywordDto> CreateKeywordAsync(string userId, CreateKeywordDto dto);
        Task<bool> UpdateKeywordAsync(string userId, int keywordId, UpdateKeywordDto dto);
        Task<bool> DeleteKeywordAsync(string userId, int keywordId);

        // Availability check
        Task<bool> CheckKeywordAvailabilityAsync(string keywordText, string? userId = null);

        // Campaign linking operations
        Task<bool> LinkToCampaignAsync(string userId, int keywordId, int campaignId);
        Task<bool> UnlinkFromCampaignAsync(string userId, int keywordId);

        // Group linking operations
        Task<bool> LinkToGroupAsync(string userId, int keywordId, int groupId);
        Task<bool> UnlinkFromGroupAsync(string userId, int keywordId);

        // Activity tracking
        Task<PaginatedResult<KeywordActivityDto>> GetActivitiesAsync(string userId, int keywordId, PagedRequest request);
        Task<KeywordActivityDto> LogActivityAsync(int keywordId, string phoneNumber, string incomingMessage, string? responseSent);
    }
}
