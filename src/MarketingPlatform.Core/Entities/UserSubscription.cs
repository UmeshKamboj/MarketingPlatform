using MarketingPlatform.Core.Enums;

namespace MarketingPlatform.Core.Entities
{
    public class UserSubscription : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public int SubscriptionPlanId { get; set; }
        public SubscriptionStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? TrialEndDate { get; set; }
        public bool IsYearly { get; set; } = false;
        public string? StripeSubscriptionId { get; set; }
        public string? StripeCustomerId { get; set; }
        public DateTime? CanceledAt { get; set; }
        public string? CancellationReason { get; set; }

        // Navigation properties
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual SubscriptionPlan SubscriptionPlan { get; set; } = null!;
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
