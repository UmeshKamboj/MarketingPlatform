using AutoMapper;
using MarketingPlatform.Application.DTOs.Common;
using MarketingPlatform.Application.DTOs.Message;
using MarketingPlatform.Application.Interfaces;
using MarketingPlatform.Core.Entities;
using MarketingPlatform.Core.Enums;
using MarketingPlatform.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MarketingPlatform.Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly IRepository<CampaignMessage> _messageRepository;
        private readonly IRepository<Contact> _contactRepository;
        private readonly IRepository<Campaign> _campaignRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISMSProvider _smsProvider;
        private readonly IMMSProvider _mmsProvider;
        private readonly IEmailProvider _emailProvider;
        private readonly ILogger<MessageService> _logger;

        public MessageService(
            IRepository<CampaignMessage> messageRepository,
            IRepository<Contact> contactRepository,
            IRepository<Campaign> campaignRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ISMSProvider smsProvider,
            IMMSProvider mmsProvider,
            IEmailProvider emailProvider,
            ILogger<MessageService> logger)
        {
            _messageRepository = messageRepository;
            _contactRepository = contactRepository;
            _campaignRepository = campaignRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _smsProvider = smsProvider;
            _mmsProvider = mmsProvider;
            _emailProvider = emailProvider;
            _logger = logger;
        }

        public async Task<MessageDto?> GetMessageByIdAsync(string userId, int messageId)
        {
            var message = await _messageRepository.GetQueryable()
                .Include(m => m.Campaign)
                .Include(m => m.Contact)
                .Where(m => m.Id == messageId && m.Campaign.UserId == userId)
                .FirstOrDefaultAsync();

            if (message == null)
                return null;

            var dto = _mapper.Map<MessageDto>(message);
            
            // Deserialize MediaUrls if present
            if (!string.IsNullOrEmpty(message.MediaUrls))
            {
                dto.MediaUrls = JsonConvert.DeserializeObject<List<string>>(message.MediaUrls);
            }

            return dto;
        }

        public async Task<PaginatedResult<MessageDto>> GetMessagesAsync(string userId, PagedRequest request)
        {
            var query = _messageRepository.GetQueryable()
                .Include(m => m.Campaign)
                .Include(m => m.Contact)
                .Where(m => m.Campaign.UserId == userId);

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(m => 
                    m.Recipient.Contains(request.SearchTerm) ||
                    m.Campaign.Name.Contains(request.SearchTerm) ||
                    m.Contact.FirstName!.Contains(request.SearchTerm) ||
                    m.Contact.LastName!.Contains(request.SearchTerm));
            }

            var totalCount = await query.CountAsync();

            query = request.SortDescending
                ? query.OrderByDescending(m => EF.Property<object>(m, request.SortBy ?? "CreatedAt"))
                : query.OrderBy(m => EF.Property<object>(m, request.SortBy ?? "CreatedAt"));

            var messages = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var dtos = messages.Select(m => {
                var dto = _mapper.Map<MessageDto>(m);
                if (!string.IsNullOrEmpty(m.MediaUrls))
                {
                    dto.MediaUrls = JsonConvert.DeserializeObject<List<string>>(m.MediaUrls);
                }
                return dto;
            }).ToList();

            return new PaginatedResult<MessageDto>
            {
                Items = dtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }

        public async Task<PaginatedResult<MessageDto>> GetMessagesByCampaignAsync(string userId, int campaignId, PagedRequest request)
        {
            var campaign = await _campaignRepository.GetByIdAsync(campaignId);
            if (campaign == null || campaign.UserId != userId)
            {
                throw new UnauthorizedAccessException("Campaign not found or access denied");
            }

            var query = _messageRepository.GetQueryable()
                .Include(m => m.Campaign)
                .Include(m => m.Contact)
                .Where(m => m.CampaignId == campaignId);

            var totalCount = await query.CountAsync();

            query = request.SortDescending
                ? query.OrderByDescending(m => EF.Property<object>(m, request.SortBy ?? "CreatedAt"))
                : query.OrderBy(m => EF.Property<object>(m, request.SortBy ?? "CreatedAt"));

            var messages = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var dtos = messages.Select(m => {
                var dto = _mapper.Map<MessageDto>(m);
                if (!string.IsNullOrEmpty(m.MediaUrls))
                {
                    dto.MediaUrls = JsonConvert.DeserializeObject<List<string>>(m.MediaUrls);
                }
                return dto;
            }).ToList();

            return new PaginatedResult<MessageDto>
            {
                Items = dtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }

        public async Task<List<MessageDto>> GetMessagesByStatusAsync(string userId, MessageStatus status)
        {
            var messages = await _messageRepository.GetQueryable()
                .Include(m => m.Campaign)
                .Include(m => m.Contact)
                .Where(m => m.Campaign.UserId == userId && m.Status == status)
                .ToListAsync();

            return messages.Select(m => {
                var dto = _mapper.Map<MessageDto>(m);
                if (!string.IsNullOrEmpty(m.MediaUrls))
                {
                    dto.MediaUrls = JsonConvert.DeserializeObject<List<string>>(m.MediaUrls);
                }
                return dto;
            }).ToList();
        }

        public async Task<MessageDto> CreateMessageAsync(string userId, CreateMessageDto dto)
        {
            var campaign = await _campaignRepository.GetByIdAsync(dto.CampaignId);
            if (campaign == null || campaign.UserId != userId)
            {
                throw new UnauthorizedAccessException("Campaign not found or access denied");
            }

            var contact = await _contactRepository.GetByIdAsync(dto.ContactId);
            if (contact == null || contact.UserId != userId)
            {
                throw new UnauthorizedAccessException("Contact not found or access denied");
            }

            var message = _mapper.Map<CampaignMessage>(dto);
            message.Status = MessageStatus.Queued;
            
            if (dto.MediaUrls != null && dto.MediaUrls.Any())
            {
                message.MediaUrls = JsonConvert.SerializeObject(dto.MediaUrls);
            }

            await _messageRepository.AddAsync(message);
            await _unitOfWork.SaveChangesAsync();

            // Reload with navigation properties
            var createdMessage = await _messageRepository.GetQueryable()
                .Include(m => m.Campaign)
                .Include(m => m.Contact)
                .FirstAsync(m => m.Id == message.Id);

            var result = _mapper.Map<MessageDto>(createdMessage);
            if (!string.IsNullOrEmpty(createdMessage.MediaUrls))
            {
                result.MediaUrls = JsonConvert.DeserializeObject<List<string>>(createdMessage.MediaUrls);
            }

            return result;
        }

        public async Task<List<MessageDto>> CreateBulkMessagesAsync(string userId, BulkMessageRequestDto dto)
        {
            var campaign = await _campaignRepository.GetByIdAsync(dto.CampaignId);
            if (campaign == null || campaign.UserId != userId)
            {
                throw new UnauthorizedAccessException("Campaign not found or access denied");
            }

            var contacts = await _contactRepository.GetQueryable()
                .Where(c => dto.ContactIds.Contains(c.Id) && c.UserId == userId)
                .ToListAsync();

            if (contacts.Count != dto.ContactIds.Count)
            {
                throw new ArgumentException("Some contacts were not found or access denied");
            }

            var messages = new List<CampaignMessage>();
            var mediaUrlsJson = dto.MediaUrls != null && dto.MediaUrls.Any() 
                ? JsonConvert.SerializeObject(dto.MediaUrls) 
                : null;

            foreach (var contact in contacts)
            {
                var recipient = dto.Channel == ChannelType.Email 
                    ? contact.Email ?? contact.PhoneNumber 
                    : contact.PhoneNumber;

                var message = new CampaignMessage
                {
                    CampaignId = dto.CampaignId,
                    ContactId = contact.Id,
                    Recipient = recipient,
                    Channel = dto.Channel,
                    Subject = dto.Subject,
                    MessageBody = dto.MessageBody,
                    HTMLContent = dto.HTMLContent,
                    MediaUrls = mediaUrlsJson,
                    ScheduledAt = dto.ScheduledAt,
                    Status = MessageStatus.Queued,
                    MaxRetries = 3
                };

                messages.Add(message);
            }

            foreach (var message in messages)
            {
                await _messageRepository.AddAsync(message);
            }
            await _unitOfWork.SaveChangesAsync();

            // Reload with navigation properties
            var messageIds = messages.Select(m => m.Id).ToList();
            var createdMessages = await _messageRepository.GetQueryable()
                .Include(m => m.Campaign)
                .Include(m => m.Contact)
                .Where(m => messageIds.Contains(m.Id))
                .ToListAsync();

            return createdMessages.Select(m => {
                var result = _mapper.Map<MessageDto>(m);
                if (!string.IsNullOrEmpty(m.MediaUrls))
                {
                    result.MediaUrls = JsonConvert.DeserializeObject<List<string>>(m.MediaUrls);
                }
                return result;
            }).ToList();
        }

        public async Task<bool> UpdateMessageStatusAsync(string userId, int messageId, MessageStatusUpdateDto dto)
        {
            var message = await _messageRepository.GetQueryable()
                .Include(m => m.Campaign)
                .FirstOrDefaultAsync(m => m.Id == messageId);

            if (message == null)
                return false;

            // For webhook updates, userId can be system (empty) or match the campaign owner
            if (!string.IsNullOrEmpty(userId) && message.Campaign.UserId != userId)
            {
                throw new UnauthorizedAccessException("Access denied");
            }

            message.Status = dto.Status;
            message.ErrorMessage = dto.ErrorMessage;

            if (dto.CostAmount.HasValue)
            {
                message.CostAmount = dto.CostAmount.Value;
            }

            if (dto.Status == MessageStatus.Delivered)
            {
                message.DeliveredAt = DateTime.UtcNow;
            }
            else if (dto.Status == MessageStatus.Failed || dto.Status == MessageStatus.Bounced)
            {
                message.FailedAt = DateTime.UtcNow;
            }

            _messageRepository.Update(message);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RetryFailedMessageAsync(string userId, int messageId)
        {
            var message = await _messageRepository.GetQueryable()
                .Include(m => m.Campaign)
                .FirstOrDefaultAsync(m => m.Id == messageId && m.Campaign.UserId == userId);

            if (message == null)
                return false;

            if (message.Status != MessageStatus.Failed && message.Status != MessageStatus.Bounced)
            {
                throw new InvalidOperationException("Only failed or bounced messages can be retried");
            }

            if (message.RetryCount >= message.MaxRetries)
            {
                throw new InvalidOperationException("Maximum retry attempts reached");
            }

            message.Status = MessageStatus.Queued;
            message.ErrorMessage = null;
            message.ScheduledAt = DateTime.UtcNow;
            message.RetryCount++;

            _messageRepository.Update(message);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<int> RetryFailedMessagesForCampaignAsync(string userId, int campaignId)
        {
            var campaign = await _campaignRepository.GetByIdAsync(campaignId);
            if (campaign == null || campaign.UserId != userId)
            {
                throw new UnauthorizedAccessException("Campaign not found or access denied");
            }

            var failedMessages = await _messageRepository.GetQueryable()
                .Where(m => m.CampaignId == campaignId && 
                           (m.Status == MessageStatus.Failed || m.Status == MessageStatus.Bounced) &&
                           m.RetryCount < m.MaxRetries)
                .ToListAsync();

            foreach (var message in failedMessages)
            {
                message.Status = MessageStatus.Queued;
                message.ErrorMessage = null;
                message.ScheduledAt = DateTime.UtcNow;
                message.RetryCount++;
                _messageRepository.Update(message);
            }

            await _unitOfWork.SaveChangesAsync();

            return failedMessages.Count;
        }

        public async Task<bool> CancelScheduledMessageAsync(string userId, int messageId)
        {
            var message = await _messageRepository.GetQueryable()
                .Include(m => m.Campaign)
                .FirstOrDefaultAsync(m => m.Id == messageId && m.Campaign.UserId == userId);

            if (message == null)
                return false;

            if (message.Status != MessageStatus.Queued)
            {
                throw new InvalidOperationException("Only queued messages can be cancelled");
            }

            // Instead of deleting, mark as failed with cancellation message
            message.Status = MessageStatus.Failed;
            message.ErrorMessage = "Cancelled by user";
            message.FailedAt = DateTime.UtcNow;

            _messageRepository.Update(message);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<MessageDeliveryReportDto> GetDeliveryReportAsync(string userId, int campaignId)
        {
            var campaign = await _campaignRepository.GetByIdAsync(campaignId);
            if (campaign == null || campaign.UserId != userId)
            {
                throw new UnauthorizedAccessException("Campaign not found or access denied");
            }

            var messages = await _messageRepository.GetQueryable()
                .Where(m => m.CampaignId == campaignId)
                .ToListAsync();

            var totalMessages = messages.Count;
            var queued = messages.Count(m => m.Status == MessageStatus.Queued);
            var sending = messages.Count(m => m.Status == MessageStatus.Sending);
            var sent = messages.Count(m => m.Status == MessageStatus.Sent);
            var delivered = messages.Count(m => m.Status == MessageStatus.Delivered);
            var failed = messages.Count(m => m.Status == MessageStatus.Failed);
            var bounced = messages.Count(m => m.Status == MessageStatus.Bounced);
            var totalCost = messages.Sum(m => m.CostAmount);

            var deliveryRate = totalMessages > 0 
                ? (decimal)delivered / totalMessages * 100 
                : 0;

            var failureRate = totalMessages > 0 
                ? (decimal)(failed + bounced) / totalMessages * 100 
                : 0;

            var averageCost = totalMessages > 0 
                ? totalCost / totalMessages 
                : 0;

            return new MessageDeliveryReportDto
            {
                CampaignId = campaignId,
                TotalMessages = totalMessages,
                Queued = queued,
                Sending = sending,
                Sent = sent,
                Delivered = delivered,
                Failed = failed,
                Bounced = bounced,
                DeliveryRate = Math.Round(deliveryRate, 2),
                FailureRate = Math.Round(failureRate, 2),
                TotalCost = totalCost,
                AverageCost = Math.Round(averageCost, 4)
            };
        }

        public async Task ProcessMessageQueueAsync()
        {
            // Use AsNoTracking for read query to avoid locks and improve performance
            // Take only a batch of messages at a time to prevent long-running transactions
            var messagesToProcess = await _messageRepository.GetQueryable()
                .AsNoTracking()
                .Where(m => m.Status == MessageStatus.Queued && 
                           (m.ScheduledAt == null || m.ScheduledAt <= DateTime.UtcNow))
                .Take(50) // Process max 50 messages per batch
                .Select(m => new { m.Id, m.Channel, m.Recipient, m.Subject, m.MessageBody, m.HTMLContent, m.MediaUrls })
                .ToListAsync();

            if (!messagesToProcess.Any())
                return;

            _logger.LogInformation("Processing {Count} messages from queue", messagesToProcess.Count);

            foreach (var msg in messagesToProcess)
            {
                try
                {
                    // Update status to Sending in a separate transaction to release locks quickly
                    await UpdateMessageStatusToSendingAsync(msg.Id);

                    // Send message (this happens outside the database transaction to avoid blocking)
                    bool success;
                    string? externalId;
                    string? error;
                    decimal? cost;

                    switch (msg.Channel)
                    {
                        case ChannelType.SMS:
                            (success, externalId, error, cost) = await _smsProvider.SendSMSAsync(
                                msg.Recipient, msg.MessageBody ?? string.Empty);
                            break;

                        case ChannelType.MMS:
                            var mediaUrls = !string.IsNullOrEmpty(msg.MediaUrls)
                                ? JsonConvert.DeserializeObject<List<string>>(msg.MediaUrls) ?? new List<string>()
                                : new List<string>();
                            (success, externalId, error, cost) = await _mmsProvider.SendMMSAsync(
                                msg.Recipient, msg.MessageBody ?? string.Empty, mediaUrls);
                            break;

                        case ChannelType.Email:
                            (success, externalId, error, cost) = await _emailProvider.SendEmailAsync(
                                msg.Recipient, msg.Subject ?? string.Empty, msg.MessageBody ?? string.Empty, msg.HTMLContent);
                            break;

                        default:
                            throw new InvalidOperationException($"Unsupported channel type: {msg.Channel}");
                    }

                    // Update final status in another separate transaction
                    await UpdateMessageAfterSendAsync(msg.Id, success, externalId, error, cost);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message {MessageId}", msg.Id);
                    await UpdateMessageAfterSendAsync(msg.Id, false, null, ex.Message, null);
                }
            }
        }

        private async Task UpdateMessageStatusToSendingAsync(int messageId)
        {
            // Quick update in a short transaction - no locks held during external API calls
            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message != null && message.Status == MessageStatus.Queued)
            {
                message.Status = MessageStatus.Sending;
                _messageRepository.Update(message);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        private async Task UpdateMessageAfterSendAsync(int messageId, bool success, string? externalId, string? error, decimal? cost)
        {
            // Quick update in a short transaction
            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message != null)
            {
                if (success)
                {
                    message.Status = MessageStatus.Sent;
                    message.SentAt = DateTime.UtcNow;
                    message.ExternalMessageId = externalId;
                    message.ProviderMessageId = externalId; // Backward compatibility
                    if (cost.HasValue)
                    {
                        message.CostAmount = cost.Value;
                    }
                }
                else
                {
                    message.Status = MessageStatus.Failed;
                    message.FailedAt = DateTime.UtcNow;
                    message.ErrorMessage = error;
                }

                _messageRepository.Update(message);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> SendMessageNowAsync(string userId, int messageId)
        {
            var message = await _messageRepository.GetQueryable()
                .Include(m => m.Campaign)
                .FirstOrDefaultAsync(m => m.Id == messageId && m.Campaign.UserId == userId);

            if (message == null)
                return false;

            if (message.Status != MessageStatus.Queued)
            {
                throw new InvalidOperationException("Only queued messages can be sent immediately");
            }

            // Update scheduled time to now to be picked up by queue processor
            message.ScheduledAt = DateTime.UtcNow;
            _messageRepository.Update(message);
            await _unitOfWork.SaveChangesAsync();

            // Trigger immediate processing
            await ProcessSingleMessageAsync(messageId);

            return true;
        }

        private async Task ProcessSingleMessageAsync(int messageId)
        {
            var message = await _messageRepository.GetQueryable()
                .AsNoTracking()
                .Where(m => m.Id == messageId)
                .Select(m => new { m.Id, m.Channel, m.Recipient, m.Subject, m.MessageBody, m.HTMLContent, m.MediaUrls, m.Status })
                .FirstOrDefaultAsync();

            if (message == null || message.Status != MessageStatus.Queued)
                return;

            try
            {
                await UpdateMessageStatusToSendingAsync(message.Id);

                bool success;
                string? externalId;
                string? error;
                decimal? cost;

                switch (message.Channel)
                {
                    case ChannelType.SMS:
                        (success, externalId, error, cost) = await _smsProvider.SendSMSAsync(
                            message.Recipient, message.MessageBody ?? string.Empty);
                        break;

                    case ChannelType.MMS:
                        var mediaUrls = !string.IsNullOrEmpty(message.MediaUrls)
                            ? JsonConvert.DeserializeObject<List<string>>(message.MediaUrls) ?? new List<string>()
                            : new List<string>();
                        (success, externalId, error, cost) = await _mmsProvider.SendMMSAsync(
                            message.Recipient, message.MessageBody ?? string.Empty, mediaUrls);
                        break;

                    case ChannelType.Email:
                        (success, externalId, error, cost) = await _emailProvider.SendEmailAsync(
                            message.Recipient, message.Subject ?? string.Empty, message.MessageBody ?? string.Empty, message.HTMLContent);
                        break;

                    default:
                        throw new InvalidOperationException($"Unsupported channel type: {message.Channel}");
                }

                await UpdateMessageAfterSendAsync(message.Id, success, externalId, error, cost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message {MessageId}", message.Id);
                await UpdateMessageAfterSendAsync(message.Id, false, null, ex.Message, null);
            }
        }
    }
}
