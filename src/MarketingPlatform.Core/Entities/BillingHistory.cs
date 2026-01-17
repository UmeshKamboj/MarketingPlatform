using MarketingPlatform.Core.Enums;

namespace MarketingPlatform.Core.Entities
{
    public class BillingHistory : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public int? InvoiceId { get; set; }
        public TransactionType Type { get; set; }
        public TransactionStatus Status { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public string? StripeChargeId { get; set; }
        public DateTime TransactionDate { get; set; }

        // Navigation properties
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual Invoice? Invoice { get; set; }
    }
}
