namespace MarketingPlatform.Core.Entities
{
    public class ConsentHistory : BaseEntity
    {
        public int ContactId { get; set; }
        public bool ConsentGiven { get; set; }
        public string? ConsentType { get; set; }
        public string? IpAddress { get; set; }
        public DateTime ConsentDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Contact Contact { get; set; } = null!;
    }
}
