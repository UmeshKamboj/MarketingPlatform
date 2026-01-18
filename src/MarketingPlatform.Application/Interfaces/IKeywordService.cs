using MarketingPlatform.Application.DTOs.Common;
using MarketingPlatform.Application.DTOs.Keyword;
using MarketingPlatform.Core.Enums;

namespace MarketingPlatform.Application.Interfaces
{
    public interface IKeywordService
    {
        Task<KeywordDto?> GetKeywordByIdAsync(string userId, int keywordId);
        Task<PaginatedResult<KeywordDto>> GetKeywordsAsync(string userId, PagedRequest request);
        Task<List<KeywordDto>> GetKeywordsByStatusAsync(string userId, KeywordStatus status);
        Task<KeywordDto> CreateKeywordAsync(string userId, CreateKeywordDto dto);
        Task<bool> UpdateKeywordAsync(string userId, int keywordId, UpdateKeywordDto dto);
        Task<bool> DeleteKeywordAsync(string userId, int keywordId);
        Task<bool> CheckKeywordAvailabilityAsync(string keywordText, string userId);
        Task<PaginatedResult<KeywordActivityDto>> GetKeywordActivitiesAsync(int keywordId, string userId, PagedRequest request);
        Task<KeywordActivityDto> ProcessInboundKeywordAsync(string phoneNumber, string message);
        Task<int> GetKeywordActivityCountAsync(int keywordId);
        Task<KeywordAnalyticsDto?> GetKeywordAnalyticsAsync(int keywordId, string userId);
    }
}
