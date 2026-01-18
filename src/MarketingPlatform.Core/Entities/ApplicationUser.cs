using Microsoft.AspNetCore.Identity;

namespace MarketingPlatform.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>();
        public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
        public virtual ICollection<ContactGroup> ContactGroups { get; set; } = new List<ContactGroup>();
        public virtual ICollection<UserActivityLog> ActivityLogs { get; set; } = new List<UserActivityLog>();
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
