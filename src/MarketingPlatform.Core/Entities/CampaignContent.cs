namespace MarketingPlatform.Core.Entities
{
    public class CampaignContent : BaseEntity
    {
        public int CampaignId { get; set; }
        public string? Subject { get; set; }
        public string? MessageBody { get; set; }
        public string? MediaUrls { get; set; } // JSON array
        public int? MessageTemplateId { get; set; }

        // Navigation properties
        public virtual Campaign Campaign { get; set; } = null!;
        public virtual MessageTemplate? MessageTemplate { get; set; }
    }
}
