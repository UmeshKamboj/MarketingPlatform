namespace MarketingPlatform.Core.Entities
{
    /// <summary>
    /// Landing page feature card entity
    /// </summary>
    public class LandingFeature : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string DetailedDescription { get; set; } = string.Empty;
        public string IconClass { get; set; } = string.Empty; // Bootstrap icon class (e.g., "bi-broadcast")
        public string ColorClass { get; set; } = string.Empty; // Color class (e.g., "primary", "success")
        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public bool ShowOnLanding { get; set; } = true;

        // Additional details for flip card back
        public string? StatTitle1 { get; set; }
        public string? StatValue1 { get; set; }
        public string? StatTitle2 { get; set; }
        public string? StatValue2 { get; set; }
        public string? StatTitle3 { get; set; }
        public string? StatValue3 { get; set; }
        public string? CallToActionText { get; set; }
        public string? CallToActionUrl { get; set; }
    }
}
