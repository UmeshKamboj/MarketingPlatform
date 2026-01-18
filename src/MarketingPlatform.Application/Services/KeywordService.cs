using AutoMapper;
using MarketingPlatform.Application.DTOs.Common;
using MarketingPlatform.Application.DTOs.Keyword;
using MarketingPlatform.Application.Interfaces;
using MarketingPlatform.Core.Entities;
using MarketingPlatform.Core.Enums;
using MarketingPlatform.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MarketingPlatform.Application.Services
{
    public class KeywordService : IKeywordService
    {
        private readonly IRepository<Keyword> _keywordRepository;
        private readonly IRepository<KeywordActivity> _activityRepository;
        private readonly IRepository<Campaign> _campaignRepository;
        private readonly IRepository<ContactGroup> _groupRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<KeywordService> _logger;

        public KeywordService(
            IRepository<Keyword> keywordRepository,
            IRepository<KeywordActivity> activityRepository,
            IRepository<Campaign> campaignRepository,
            IRepository<ContactGroup> groupRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<KeywordService> logger)
        {
            _keywordRepository = keywordRepository;
            _activityRepository = activityRepository;
            _campaignRepository = campaignRepository;
            _groupRepository = groupRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<KeywordDto?> GetKeywordByIdAsync(string userId, int keywordId)
        {
            var keyword = await _keywordRepository.GetQueryable()
                .Include(k => k.LinkedCampaign)
                .Include(k => k.OptInGroup)
                .Where(k => k.Id == keywordId && k.UserId == userId && !k.IsDeleted)
                .FirstOrDefaultAsync();

            if (keyword == null)
                return null;

            return _mapper.Map<KeywordDto>(keyword);
        }

        public async Task<PaginatedResult<KeywordDto>> GetKeywordsAsync(string userId, PagedRequest request)
        {
            var query = _keywordRepository.GetQueryable()
                .Include(k => k.LinkedCampaign)
                .Include(k => k.OptInGroup)
                .Where(k => k.UserId == userId && !k.IsDeleted);

            // Apply search
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var searchLower = request.SearchTerm.ToLower();
                query = query.Where(k =>
                    k.KeywordText.ToLower().Contains(searchLower) ||
                    (k.Description != null && k.Description.ToLower().Contains(searchLower)));
            }

            var totalCount = await query.CountAsync();

            // Apply sorting
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                query = request.SortBy.ToLower() switch
                {
                    "keywordtext" => request.SortDescending ? query.OrderByDescending(k => k.KeywordText) : query.OrderBy(k => k.KeywordText),
                    "status" => request.SortDescending ? query.OrderByDescending(k => k.Status) : query.OrderBy(k => k.Status),
                    "createdat" => request.SortDescending ? query.OrderByDescending(k => k.CreatedAt) : query.OrderBy(k => k.CreatedAt),
                    _ => query.OrderByDescending(k => k.CreatedAt)
                };
            }
            else
            {
                query = query.OrderByDescending(k => k.CreatedAt);
            }

            // Apply pagination
            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var dtos = _mapper.Map<List<KeywordDto>>(items);

            return new PaginatedResult<KeywordDto>
            {
                Items = dtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }

        public async Task<List<KeywordDto>> GetActiveKeywordsAsync(string userId)
        {
            var keywords = await _keywordRepository.GetQueryable()
                .Include(k => k.LinkedCampaign)
                .Include(k => k.OptInGroup)
                .Where(k => k.UserId == userId && k.Status == KeywordStatus.Active && !k.IsDeleted)
                .ToListAsync();

            return _mapper.Map<List<KeywordDto>>(keywords);
        }

        public async Task<KeywordDto> CreateKeywordAsync(string userId, CreateKeywordDto dto)
        {
            // Check if keyword already exists
            var existingKeyword = await _keywordRepository.FirstOrDefaultAsync(
                k => k.KeywordText.ToLower() == dto.KeywordText.ToLower() && !k.IsDeleted);

            if (existingKeyword != null)
            {
                throw new InvalidOperationException($"Keyword '{dto.KeywordText}' already exists.");
            }

            // Validate campaign if provided
            if (dto.LinkedCampaignId.HasValue)
            {
                var campaign = await _campaignRepository.FirstOrDefaultAsync(
                    c => c.Id == dto.LinkedCampaignId.Value && c.UserId == userId && !c.IsDeleted);
                
                if (campaign == null)
                {
                    throw new InvalidOperationException("Campaign not found or you don't have permission to link it.");
                }
            }

            // Validate group if provided
            if (dto.OptInGroupId.HasValue)
            {
                var group = await _groupRepository.FirstOrDefaultAsync(
                    g => g.Id == dto.OptInGroupId.Value && g.UserId == userId && !g.IsDeleted);
                
                if (group == null)
                {
                    throw new InvalidOperationException("Group not found or you don't have permission to link it.");
                }
            }

            var keyword = _mapper.Map<Keyword>(dto);
            keyword.UserId = userId;

            await _keywordRepository.AddAsync(keyword);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Keyword '{KeywordText}' created by user {UserId}", dto.KeywordText, userId);

            // Reload with navigation properties
            var createdKeyword = await _keywordRepository.GetQueryable()
                .Include(k => k.LinkedCampaign)
                .Include(k => k.OptInGroup)
                .Where(k => k.Id == keyword.Id)
                .FirstOrDefaultAsync();

            return _mapper.Map<KeywordDto>(createdKeyword);
        }

        public async Task<bool> UpdateKeywordAsync(string userId, int keywordId, UpdateKeywordDto dto)
        {
            var keyword = await _keywordRepository.FirstOrDefaultAsync(
                k => k.Id == keywordId && k.UserId == userId && !k.IsDeleted);

            if (keyword == null)
                return false;

            // Prevent updating globally reserved keywords
            if (keyword.IsGloballyReserved)
            {
                _logger.LogWarning("Attempted to update globally reserved keyword {KeywordId}", keywordId);
                throw new InvalidOperationException("Cannot update a globally reserved keyword.");
            }

            keyword.Description = dto.Description;
            keyword.Status = dto.Status;
            keyword.ResponseMessage = dto.ResponseMessage;
            keyword.UpdatedAt = DateTime.UtcNow;

            _keywordRepository.Update(keyword);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Keyword {KeywordId} updated by user {UserId}", keywordId, userId);

            return true;
        }

        public async Task<bool> DeleteKeywordAsync(string userId, int keywordId)
        {
            var keyword = await _keywordRepository.FirstOrDefaultAsync(
                k => k.Id == keywordId && k.UserId == userId && !k.IsDeleted);

            if (keyword == null)
                return false;

            // Prevent deleting globally reserved keywords
            if (keyword.IsGloballyReserved)
            {
                _logger.LogWarning("Attempted to delete globally reserved keyword {KeywordId}", keywordId);
                throw new InvalidOperationException("Cannot delete a globally reserved keyword.");
            }

            keyword.IsDeleted = true;
            keyword.UpdatedAt = DateTime.UtcNow;

            _keywordRepository.Update(keyword);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Keyword {KeywordId} deleted by user {UserId}", keywordId, userId);

            return true;
        }

        public async Task<bool> CheckKeywordAvailabilityAsync(string keywordText, string? userId = null)
        {
            var existingKeyword = await _keywordRepository.FirstOrDefaultAsync(
                k => k.KeywordText.ToLower() == keywordText.ToLower() && !k.IsDeleted);

            // If keyword exists and is globally reserved, it's not available
            if (existingKeyword != null && existingKeyword.IsGloballyReserved)
                return false;

            // If keyword exists and belongs to another user, it's not available
            if (existingKeyword != null && userId != null && existingKeyword.UserId != userId)
                return false;

            // If keyword doesn't exist, it's available
            if (existingKeyword == null)
                return true;

            // If keyword exists and belongs to the same user, it's not available (duplicate)
            return false;
        }

        public async Task<bool> LinkToCampaignAsync(string userId, int keywordId, int campaignId)
        {
            var keyword = await _keywordRepository.FirstOrDefaultAsync(
                k => k.Id == keywordId && k.UserId == userId && !k.IsDeleted);

            if (keyword == null)
                return false;

            var campaign = await _campaignRepository.FirstOrDefaultAsync(
                c => c.Id == campaignId && c.UserId == userId && !c.IsDeleted);

            if (campaign == null)
            {
                throw new InvalidOperationException("Campaign not found or you don't have permission to link it.");
            }

            keyword.LinkedCampaignId = campaignId;
            keyword.UpdatedAt = DateTime.UtcNow;

            _keywordRepository.Update(keyword);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Keyword {KeywordId} linked to campaign {CampaignId}", keywordId, campaignId);

            return true;
        }

        public async Task<bool> UnlinkFromCampaignAsync(string userId, int keywordId)
        {
            var keyword = await _keywordRepository.FirstOrDefaultAsync(
                k => k.Id == keywordId && k.UserId == userId && !k.IsDeleted);

            if (keyword == null)
                return false;

            keyword.LinkedCampaignId = null;
            keyword.UpdatedAt = DateTime.UtcNow;

            _keywordRepository.Update(keyword);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Keyword {KeywordId} unlinked from campaign", keywordId);

            return true;
        }

        public async Task<bool> LinkToGroupAsync(string userId, int keywordId, int groupId)
        {
            var keyword = await _keywordRepository.FirstOrDefaultAsync(
                k => k.Id == keywordId && k.UserId == userId && !k.IsDeleted);

            if (keyword == null)
                return false;

            var group = await _groupRepository.FirstOrDefaultAsync(
                g => g.Id == groupId && g.UserId == userId && !g.IsDeleted);

            if (group == null)
            {
                throw new InvalidOperationException("Group not found or you don't have permission to link it.");
            }

            keyword.OptInGroupId = groupId;
            keyword.UpdatedAt = DateTime.UtcNow;

            _keywordRepository.Update(keyword);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Keyword {KeywordId} linked to group {GroupId}", keywordId, groupId);

            return true;
        }

        public async Task<bool> UnlinkFromGroupAsync(string userId, int keywordId)
        {
            var keyword = await _keywordRepository.FirstOrDefaultAsync(
                k => k.Id == keywordId && k.UserId == userId && !k.IsDeleted);

            if (keyword == null)
                return false;

            keyword.OptInGroupId = null;
            keyword.UpdatedAt = DateTime.UtcNow;

            _keywordRepository.Update(keyword);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Keyword {KeywordId} unlinked from group", keywordId);

            return true;
        }

        public async Task<PaginatedResult<KeywordActivityDto>> GetActivitiesAsync(string userId, int keywordId, PagedRequest request)
        {
            // Verify keyword ownership
            var keyword = await _keywordRepository.FirstOrDefaultAsync(
                k => k.Id == keywordId && k.UserId == userId && !k.IsDeleted);

            if (keyword == null)
            {
                return new PaginatedResult<KeywordActivityDto>
                {
                    Items = new List<KeywordActivityDto>(),
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = 0,
                    TotalPages = 0
                };
            }

            var query = _activityRepository.GetQueryable()
                .Include(a => a.Keyword)
                .Where(a => a.KeywordId == keywordId && !a.IsDeleted);

            // Apply search
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var searchLower = request.SearchTerm.ToLower();
                query = query.Where(a =>
                    a.PhoneNumber.ToLower().Contains(searchLower) ||
                    a.IncomingMessage.ToLower().Contains(searchLower));
            }

            var totalCount = await query.CountAsync();

            // Apply sorting
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                query = request.SortBy.ToLower() switch
                {
                    "receivedat" => request.SortDescending ? query.OrderByDescending(a => a.ReceivedAt) : query.OrderBy(a => a.ReceivedAt),
                    "phonenumber" => request.SortDescending ? query.OrderByDescending(a => a.PhoneNumber) : query.OrderBy(a => a.PhoneNumber),
                    _ => query.OrderByDescending(a => a.ReceivedAt)
                };
            }
            else
            {
                query = query.OrderByDescending(a => a.ReceivedAt);
            }

            // Apply pagination
            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var dtos = _mapper.Map<List<KeywordActivityDto>>(items);

            return new PaginatedResult<KeywordActivityDto>
            {
                Items = dtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }

        public async Task<KeywordActivityDto> LogActivityAsync(int keywordId, string phoneNumber, string incomingMessage, string? responseSent)
        {
            var keyword = await _keywordRepository.FirstOrDefaultAsync(
                k => k.Id == keywordId && !k.IsDeleted);

            if (keyword == null)
            {
                throw new InvalidOperationException("Keyword not found.");
            }

            var activity = new KeywordActivity
            {
                KeywordId = keywordId,
                PhoneNumber = phoneNumber,
                IncomingMessage = incomingMessage,
                ResponseSent = responseSent,
                ReceivedAt = DateTime.UtcNow
            };

            await _activityRepository.AddAsync(activity);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Activity logged for keyword {KeywordId} from {PhoneNumber}", keywordId, phoneNumber);

            // Reload with navigation properties
            var createdActivity = await _activityRepository.GetQueryable()
                .Include(a => a.Keyword)
                .Where(a => a.Id == activity.Id)
                .FirstOrDefaultAsync();

            return _mapper.Map<KeywordActivityDto>(createdActivity);
        }
    }
}
