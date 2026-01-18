using AutoMapper;
using MarketingPlatform.Application.DTOs.Common;
using MarketingPlatform.Application.DTOs.Keyword;
using MarketingPlatform.Application.Interfaces;
using MarketingPlatform.Core.Entities;
using MarketingPlatform.Core.Enums;
using MarketingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace MarketingPlatform.Application.Services
{
    public class KeywordService : IKeywordService
    {
        private readonly IRepository<Keyword> _keywordRepository;
        private readonly IRepository<KeywordActivity> _keywordActivityRepository;
        private readonly IRepository<Campaign> _campaignRepository;
        private readonly IRepository<ContactGroup> _contactGroupRepository;
        private readonly IRepository<ContactGroupMember> _groupMemberRepository;
        private readonly IRepository<Contact> _contactRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<KeywordService> _logger;
        private readonly ISMSProvider _smsProvider;

        public KeywordService(
            IRepository<Keyword> keywordRepository,
            IRepository<KeywordActivity> keywordActivityRepository,
            IRepository<Campaign> campaignRepository,
            IRepository<ContactGroup> contactGroupRepository,
            IRepository<ContactGroupMember> groupMemberRepository,
            IRepository<Contact> contactRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<KeywordService> logger,
            ISMSProvider smsProvider)
        {
            _keywordRepository = keywordRepository;
            _keywordActivityRepository = keywordActivityRepository;
            _campaignRepository = campaignRepository;
            _contactGroupRepository = contactGroupRepository;
            _groupMemberRepository = groupMemberRepository;
            _contactRepository = contactRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _smsProvider = smsProvider;
        }

        public async Task<KeywordDto?> GetKeywordByIdAsync(string userId, int keywordId)
        {
            var keyword = await _keywordRepository.FirstOrDefaultAsync(k =>
                k.Id == keywordId && k.UserId == userId && !k.IsDeleted);

            if (keyword == null)
                return null;

            return await MapToKeywordDtoAsync(keyword);
        }

        public async Task<PaginatedResult<KeywordDto>> GetKeywordsAsync(string userId, PagedRequest request)
        {
            var query = (await _keywordRepository.FindAsync(k =>
                k.UserId == userId && !k.IsDeleted)).AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(k =>
                    k.KeywordText.Contains(request.SearchTerm) ||
                    (k.Description != null && k.Description.Contains(request.SearchTerm)));
            }

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                query = request.SortBy.ToLower() switch
                {
                    "keywordtext" => request.SortDescending ? query.OrderByDescending(k => k.KeywordText) : query.OrderBy(k => k.KeywordText),
                    "status" => request.SortDescending ? query.OrderByDescending(k => k.Status) : query.OrderBy(k => k.Status),
                    "createdat" => request.SortDescending ? query.OrderByDescending(k => k.CreatedAt) : query.OrderBy(k => k.CreatedAt),
                    _ => request.SortDescending ? query.OrderByDescending(k => k.CreatedAt) : query.OrderBy(k => k.CreatedAt)
                };
            }
            else
            {
                query = query.OrderByDescending(k => k.CreatedAt);
            }

            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            var keywords = query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var keywordDtos = new List<KeywordDto>();
            foreach (var keyword in keywords)
            {
                keywordDtos.Add(await MapToKeywordDtoAsync(keyword));
            }

            return new PaginatedResult<KeywordDto>
            {
                Items = keywordDtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }

        public async Task<List<KeywordDto>> GetKeywordsByStatusAsync(string userId, KeywordStatus status)
        {
            var keywords = await _keywordRepository.FindAsync(k =>
                k.UserId == userId && k.Status == status && !k.IsDeleted);

            var keywordDtos = new List<KeywordDto>();
            foreach (var keyword in keywords.OrderByDescending(k => k.CreatedAt))
            {
                keywordDtos.Add(await MapToKeywordDtoAsync(keyword));
            }

            return keywordDtos;
        }

        public async Task<KeywordDto> CreateKeywordAsync(string userId, CreateKeywordDto dto)
        {
            // Normalize keyword text (uppercase, trim)
            var normalizedKeywordText = dto.KeywordText.Trim().ToUpperInvariant();

            // Check if keyword is available
            var isAvailable = await CheckKeywordAvailabilityAsync(normalizedKeywordText, userId);
            if (!isAvailable)
            {
                _logger.LogWarning("Keyword {KeywordText} is already in use", normalizedKeywordText);
                throw new InvalidOperationException($"Keyword '{normalizedKeywordText}' is already in use or reserved");
            }

            // Validate linked campaign if provided
            if (dto.LinkedCampaignId.HasValue)
            {
                var campaign = await _campaignRepository.FirstOrDefaultAsync(c =>
                    c.Id == dto.LinkedCampaignId.Value && c.UserId == userId && !c.IsDeleted);
                if (campaign == null)
                {
                    _logger.LogWarning("Campaign {CampaignId} not found for user {UserId}", dto.LinkedCampaignId.Value, userId);
                    throw new InvalidOperationException($"Campaign with ID {dto.LinkedCampaignId.Value} not found");
                }
            }

            // Validate opt-in group if provided
            if (dto.OptInGroupId.HasValue)
            {
                var group = await _contactGroupRepository.FirstOrDefaultAsync(g =>
                    g.Id == dto.OptInGroupId.Value && g.UserId == userId && !g.IsDeleted);
                if (group == null)
                {
                    _logger.LogWarning("Contact group {GroupId} not found for user {UserId}", dto.OptInGroupId.Value, userId);
                    throw new InvalidOperationException($"Contact group with ID {dto.OptInGroupId.Value} not found");
                }
            }

            var keyword = new Keyword
            {
                UserId = userId,
                KeywordText = normalizedKeywordText,
                Description = dto.Description,
                Status = KeywordStatus.Active,
                ResponseMessage = dto.ResponseMessage,
                LinkedCampaignId = dto.LinkedCampaignId,
                OptInGroupId = dto.OptInGroupId,
                IsGloballyReserved = false
            };

            await _keywordRepository.AddAsync(keyword);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Keyword {KeywordText} created successfully by user {UserId}", normalizedKeywordText, userId);

            var createdKeyword = await GetKeywordByIdAsync(userId, keyword.Id);
            if (createdKeyword == null)
            {
                _logger.LogError("Failed to retrieve created keyword {KeywordId}", keyword.Id);
                throw new InvalidOperationException($"Failed to retrieve created keyword with ID {keyword.Id}");
            }

            return createdKeyword;
        }

        public async Task<bool> UpdateKeywordAsync(string userId, int keywordId, UpdateKeywordDto dto)
        {
            var keyword = await _keywordRepository.FirstOrDefaultAsync(k =>
                k.Id == keywordId && k.UserId == userId && !k.IsDeleted);

            if (keyword == null)
                return false;

            // Check if keyword is globally reserved and prevent modification
            if (keyword.IsGloballyReserved)
            {
                _logger.LogWarning("Attempted to update globally reserved keyword {KeywordId}", keywordId);
                return false;
            }

            // Normalize new keyword text
            var normalizedKeywordText = dto.KeywordText.Trim().ToUpperInvariant();

            // Check if keyword text changed and if new text is available
            if (keyword.KeywordText != normalizedKeywordText)
            {
                var isAvailable = await CheckKeywordAvailabilityAsync(normalizedKeywordText, userId);
                if (!isAvailable)
                {
                    _logger.LogWarning("Keyword {KeywordText} is already in use", normalizedKeywordText);
                    throw new InvalidOperationException($"Keyword '{normalizedKeywordText}' is already in use or reserved");
                }
            }

            // Validate linked campaign if provided
            if (dto.LinkedCampaignId.HasValue)
            {
                var campaign = await _campaignRepository.FirstOrDefaultAsync(c =>
                    c.Id == dto.LinkedCampaignId.Value && c.UserId == userId && !c.IsDeleted);
                if (campaign == null)
                {
                    _logger.LogWarning("Campaign {CampaignId} not found for user {UserId}", dto.LinkedCampaignId.Value, userId);
                    throw new InvalidOperationException($"Campaign with ID {dto.LinkedCampaignId.Value} not found");
                }
            }

            // Validate opt-in group if provided
            if (dto.OptInGroupId.HasValue)
            {
                var group = await _contactGroupRepository.FirstOrDefaultAsync(g =>
                    g.Id == dto.OptInGroupId.Value && g.UserId == userId && !g.IsDeleted);
                if (group == null)
                {
                    _logger.LogWarning("Contact group {GroupId} not found for user {UserId}", dto.OptInGroupId.Value, userId);
                    throw new InvalidOperationException($"Contact group with ID {dto.OptInGroupId.Value} not found");
                }
            }

            // Update keyword properties
            keyword.KeywordText = normalizedKeywordText;
            keyword.Description = dto.Description;
            keyword.Status = dto.Status;
            keyword.ResponseMessage = dto.ResponseMessage;
            keyword.LinkedCampaignId = dto.LinkedCampaignId;
            keyword.OptInGroupId = dto.OptInGroupId;
            keyword.UpdatedAt = DateTime.UtcNow;

            _keywordRepository.Update(keyword);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Keyword {KeywordId} updated successfully", keywordId);

            return true;
        }

        public async Task<bool> DeleteKeywordAsync(string userId, int keywordId)
        {
            var keyword = await _keywordRepository.FirstOrDefaultAsync(k =>
                k.Id == keywordId && k.UserId == userId && !k.IsDeleted);

            if (keyword == null)
                return false;

            // Check if keyword is globally reserved and prevent deletion
            if (keyword.IsGloballyReserved)
            {
                _logger.LogWarning("Attempted to delete globally reserved keyword {KeywordId}", keywordId);
                return false;
            }

            // Soft delete
            keyword.IsDeleted = true;
            keyword.UpdatedAt = DateTime.UtcNow;

            _keywordRepository.Update(keyword);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Keyword {KeywordId} deleted successfully", keywordId);

            return true;
        }

        public async Task<bool> CheckKeywordAvailabilityAsync(string keywordText, string userId)
        {
            var normalizedKeywordText = keywordText.Trim().ToUpperInvariant();

            // Check if keyword is globally reserved
            var globallyReserved = await _keywordRepository.AnyAsync(k =>
                k.KeywordText == normalizedKeywordText && k.IsGloballyReserved && !k.IsDeleted);

            if (globallyReserved)
                return false;

            // Check if keyword is already in use by this user
            var existingKeyword = await _keywordRepository.AnyAsync(k =>
                k.KeywordText == normalizedKeywordText && k.UserId == userId && !k.IsDeleted);

            return !existingKeyword;
        }

        public async Task<PaginatedResult<KeywordActivityDto>> GetKeywordActivitiesAsync(int keywordId, string userId, PagedRequest request)
        {
            // First verify the keyword belongs to the user
            var keyword = await _keywordRepository.FirstOrDefaultAsync(k =>
                k.Id == keywordId && k.UserId == userId && !k.IsDeleted);

            if (keyword == null)
            {
                _logger.LogWarning("Keyword {KeywordId} not found for user {UserId}", keywordId, userId);
                throw new InvalidOperationException($"Keyword with ID {keywordId} not found");
            }

            var query = (await _keywordActivityRepository.FindAsync(ka =>
                ka.KeywordId == keywordId)).AsQueryable();

            // Apply sorting
            query = query.OrderByDescending(ka => ka.ReceivedAt);

            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            var activities = query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var activityDtos = activities.Select(a => new KeywordActivityDto
            {
                Id = a.Id,
                KeywordId = a.KeywordId,
                KeywordText = keyword.KeywordText,
                PhoneNumber = a.PhoneNumber,
                IncomingMessage = a.IncomingMessage,
                ResponseSent = a.ResponseSent,
                ReceivedAt = a.ReceivedAt
            }).ToList();

            return new PaginatedResult<KeywordActivityDto>
            {
                Items = activityDtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }

        public async Task<KeywordActivityDto> ProcessInboundKeywordAsync(string phoneNumber, string message)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                _logger.LogWarning("ProcessInboundKeywordAsync called with null or empty phoneNumber");
                throw new ArgumentException("Phone number is required", nameof(phoneNumber));
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                _logger.LogWarning("ProcessInboundKeywordAsync called with null or empty message");
                throw new ArgumentException("Message is required", nameof(message));
            }

            // Extract the first word as the keyword
            var keywordText = message.Trim().Split(' ')[0].ToUpperInvariant();

            // Find matching keyword
            var keyword = await _keywordRepository.FirstOrDefaultAsync(k =>
                k.KeywordText == keywordText && k.Status == KeywordStatus.Active && !k.IsDeleted);

            if (keyword == null)
            {
                _logger.LogInformation("No active keyword found for text: {KeywordText}", keywordText);
                // Return DTO directly for unknown keyword (not saved to database with KeywordId = 0)
                return new KeywordActivityDto
                {
                    Id = 0,
                    KeywordId = 0,
                    KeywordText = keywordText,
                    PhoneNumber = phoneNumber,
                    IncomingMessage = message,
                    ResponseSent = null,
                    ReceivedAt = DateTime.UtcNow
                };
            }

            // Log keyword activity
            var activity = new KeywordActivity
            {
                KeywordId = keyword.Id,
                PhoneNumber = phoneNumber,
                IncomingMessage = message,
                ReceivedAt = DateTime.UtcNow
            };

            // Send response if configured
            string? responseSent = null;
            if (!string.IsNullOrWhiteSpace(keyword.ResponseMessage))
            {
                try
                {
                    await _smsProvider.SendSMSAsync(phoneNumber, keyword.ResponseMessage);
                    responseSent = keyword.ResponseMessage;
                    _logger.LogInformation("Auto-response sent for keyword {KeywordId} to {PhoneNumber}", keyword.Id, phoneNumber);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send auto-response for keyword {KeywordId}", keyword.Id);
                }
            }

            activity.ResponseSent = responseSent;

            await _keywordActivityRepository.AddAsync(activity);
            await _unitOfWork.SaveChangesAsync();

            // Add contact to opt-in group if configured
            if (keyword.OptInGroupId.HasValue)
            {
                try
                {
                    // Check if contact exists with this phone number
                    var contact = await _contactRepository.FirstOrDefaultAsync(c =>
                        c.PhoneNumber == phoneNumber && c.UserId == keyword.UserId && !c.IsDeleted);

                    if (contact != null)
                    {
                        // Check if contact is already in the group
                        var existingMember = await _groupMemberRepository.FirstOrDefaultAsync(gm =>
                            gm.ContactId == contact.Id && gm.ContactGroupId == keyword.OptInGroupId.Value && !gm.IsDeleted);

                        if (existingMember == null)
                        {
                            // Add contact to group
                            var newMember = new ContactGroupMember
                            {
                                ContactGroupId = keyword.OptInGroupId.Value,
                                ContactId = contact.Id
                            };
                            await _groupMemberRepository.AddAsync(newMember);
                            await _unitOfWork.SaveChangesAsync();

                            _logger.LogInformation("Contact {ContactId} added to group {GroupId} via keyword {KeywordId}",
                                contact.Id, keyword.OptInGroupId.Value, keyword.Id);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Contact with phone number {PhoneNumber} not found for opt-in group addition", phoneNumber);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to add contact to opt-in group for keyword {KeywordId}", keyword.Id);
                }
            }

            var activityDto = _mapper.Map<KeywordActivityDto>(activity);
            activityDto.KeywordText = keyword.KeywordText;
            return activityDto;
        }

        public async Task<int> GetKeywordActivityCountAsync(int keywordId)
        {
            return await _keywordActivityRepository.CountAsync(ka => ka.KeywordId == keywordId);
        }

        public async Task<KeywordAnalyticsDto?> GetKeywordAnalyticsAsync(int keywordId, string userId)
        {
            // First verify the keyword belongs to the user
            var keyword = await _keywordRepository.FirstOrDefaultAsync(k =>
                k.Id == keywordId && k.UserId == userId && !k.IsDeleted);

            if (keyword == null)
            {
                _logger.LogWarning("Keyword {KeywordId} not found for user {UserId}", keywordId, userId);
                return null;
            }

            // Get all activities for this keyword
            var activities = await _keywordActivityRepository.FindAsync(ka => ka.KeywordId == keywordId);
            var activitiesList = activities.ToList();

            // Calculate time-based metrics
            var now = DateTime.UtcNow;
            var last24Hours = now.AddHours(-24);
            var last7Days = now.AddDays(-7);
            var last30Days = now.AddDays(-30);

            // Basic usage statistics
            var totalResponses = activitiesList.Count;
            var uniquePhoneNumbers = activitiesList.Select(a => a.PhoneNumber).Distinct().ToList();
            var uniqueContacts = uniquePhoneNumbers.Count;
            var repeatUsageCount = totalResponses - uniqueContacts;

            // Response statistics
            var responsesSent = activitiesList.Count(a => !string.IsNullOrWhiteSpace(a.ResponseSent));
            var responsesFailed = totalResponses - responsesSent;
            var responseSuccessRate = totalResponses > 0 
                ? Math.Round((decimal)responsesSent / totalResponses * 100, 2) 
                : 0;

            // Opt-in statistics
            int totalOptIns = 0;
            int successfulOptIns = 0;
            int failedOptIns = 0;

            if (keyword.OptInGroupId.HasValue)
            {
                // Count how many unique contacts from activities are in the opt-in group
                var contactsWithPhones = await _contactRepository.FindAsync(c => 
                    uniquePhoneNumbers.Contains(c.PhoneNumber) && 
                    c.UserId == userId && 
                    !c.IsDeleted);

                var contactIds = contactsWithPhones.Select(c => c.Id).ToList();
                totalOptIns = contactIds.Count;

                if (totalOptIns > 0)
                {
                    var groupMembers = await _groupMemberRepository.FindAsync(gm =>
                        gm.ContactGroupId == keyword.OptInGroupId.Value &&
                        contactIds.Contains(gm.ContactId) &&
                        !gm.IsDeleted);

                    successfulOptIns = groupMembers.Count();
                    failedOptIns = totalOptIns - successfulOptIns;
                }
            }

            var optInConversionRate = totalResponses > 0 
                ? Math.Round((decimal)successfulOptIns / totalResponses * 100, 2) 
                : 0;

            // Time-based analytics
            var firstUsedAt = activitiesList.Any() 
                ? activitiesList.Min(a => a.ReceivedAt) 
                : (DateTime?)null;
            var lastUsedAt = activitiesList.Any() 
                ? activitiesList.Max(a => a.ReceivedAt) 
                : (DateTime?)null;
            var activitiesLast24Hours = activitiesList.Count(a => a.ReceivedAt >= last24Hours);
            var activitiesLast7Days = activitiesList.Count(a => a.ReceivedAt >= last7Days);
            var activitiesLast30Days = activitiesList.Count(a => a.ReceivedAt >= last30Days);

            // Campaign statistics
            var campaignRelatedActivities = keyword.LinkedCampaignId.HasValue ? totalResponses : 0;
            string? linkedCampaignName = null;

            if (keyword.LinkedCampaignId.HasValue)
            {
                var campaign = await _campaignRepository.FirstOrDefaultAsync(c =>
                    c.Id == keyword.LinkedCampaignId.Value && !c.IsDeleted);
                linkedCampaignName = campaign?.Name;
            }

            return new KeywordAnalyticsDto
            {
                KeywordId = keyword.Id,
                KeywordText = keyword.KeywordText,
                TotalResponses = totalResponses,
                UniqueContacts = uniqueContacts,
                RepeatUsageCount = repeatUsageCount,
                TotalOptIns = totalOptIns,
                SuccessfulOptIns = successfulOptIns,
                FailedOptIns = failedOptIns,
                OptInConversionRate = optInConversionRate,
                ResponsesSent = responsesSent,
                ResponsesFailed = responsesFailed,
                ResponseSuccessRate = responseSuccessRate,
                LinkedCampaignId = keyword.LinkedCampaignId,
                LinkedCampaignName = linkedCampaignName,
                CampaignRelatedActivities = campaignRelatedActivities,
                FirstUsedAt = firstUsedAt,
                LastUsedAt = lastUsedAt,
                ActivitiesLast24Hours = activitiesLast24Hours,
                ActivitiesLast7Days = activitiesLast7Days,
                ActivitiesLast30Days = activitiesLast30Days
            };
        }

        private async Task<KeywordDto> MapToKeywordDtoAsync(Keyword keyword)
        {
            var dto = _mapper.Map<KeywordDto>(keyword);

            // Load linked campaign name
            if (keyword.LinkedCampaignId.HasValue)
            {
                var campaign = await _campaignRepository.FirstOrDefaultAsync(c =>
                    c.Id == keyword.LinkedCampaignId.Value && !c.IsDeleted);
                dto.LinkedCampaignName = campaign?.Name;
            }

            // Load opt-in group name
            if (keyword.OptInGroupId.HasValue)
            {
                var group = await _contactGroupRepository.FirstOrDefaultAsync(g =>
                    g.Id == keyword.OptInGroupId.Value && !g.IsDeleted);
                dto.OptInGroupName = group?.Name;
            }

            // Load activity count
            dto.ActivityCount = await GetKeywordActivityCountAsync(keyword.Id);

            return dto;
        }
    }
}
