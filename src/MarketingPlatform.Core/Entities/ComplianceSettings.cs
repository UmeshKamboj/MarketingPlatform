namespace MarketingPlatform.Core.Entities
{
    public class ComplianceSettings : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public bool RequireDoubleOptIn { get; set; } = false;
        public bool EnableQuietHours { get; set; } = false;
        public TimeSpan? QuietHoursStart { get; set; }
        public TimeSpan? QuietHoursEnd { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyAddress { get; set; }
        public string? PrivacyPolicyUrl { get; set; }

        // Navigation properties
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
