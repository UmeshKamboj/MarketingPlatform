using MarketingPlatform.Core.Enums;

namespace MarketingPlatform.Core.Entities
{
    public class CampaignMessage : BaseEntity
    {
        public int CampaignId { get; set; }
        public int ContactId { get; set; }
        public MessageStatus Status { get; set; } = MessageStatus.Queued;
        public string? MessageBody { get; set; }
        public string? MediaUrls { get; set; } // JSON array
        public DateTime? SentAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string? ProviderMessageId { get; set; }
        public string? ErrorMessage { get; set; }
        public decimal CostAmount { get; set; } = 0.00m;

        // Navigation properties
        public virtual Campaign Campaign { get; set; } = null!;
        public virtual Contact Contact { get; set; } = null!;
    }
}
