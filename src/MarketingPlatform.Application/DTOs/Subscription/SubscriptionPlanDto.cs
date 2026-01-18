namespace MarketingPlatform.Application.DTOs.Subscription
{
    public class SubscriptionPlanDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal PriceMonthly { get; set; }
        public decimal PriceYearly { get; set; }
        public int SMSLimit { get; set; }
        public int MMSLimit { get; set; }
        public int EmailLimit { get; set; }
        public int ContactLimit { get; set; }
        public Dictionary<string, object>? Features { get; set; }
        public string? StripeProductId { get; set; }
        public string? StripePriceIdMonthly { get; set; }
        public string? StripePriceIdYearly { get; set; }
        public string? PayPalProductId { get; set; }
        public string? PayPalPlanIdMonthly { get; set; }
        public string? PayPalPlanIdYearly { get; set; }
        public bool IsActive { get; set; }
        public bool IsVisible { get; set; }
        public bool ShowOnLanding { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
