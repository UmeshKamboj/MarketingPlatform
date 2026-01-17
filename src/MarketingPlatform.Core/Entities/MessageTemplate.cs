using MarketingPlatform.Core.Enums;

namespace MarketingPlatform.Core.Entities
{
    public class MessageTemplate : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public ChannelType Channel { get; set; }
        public string? Category { get; set; }
        public string? Subject { get; set; }
        public string MessageBody { get; set; } = string.Empty;
        public string? MediaUrls { get; set; } // JSON array
        public bool IsDefault { get; set; } = false;

        // Navigation properties
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual ICollection<CampaignContent> CampaignContents { get; set; } = new List<CampaignContent>();
    }
}
