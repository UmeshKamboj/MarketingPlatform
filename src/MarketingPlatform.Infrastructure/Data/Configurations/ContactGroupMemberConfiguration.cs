using MarketingPlatform.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketingPlatform.Infrastructure.Data.Configurations
{
    public class ContactGroupMemberConfiguration : IEntityTypeConfiguration<ContactGroupMember>
    {
        public void Configure(EntityTypeBuilder<ContactGroupMember> builder)
        {
            builder.HasKey(cgm => cgm.Id);

            // Indexes
            builder.HasIndex(cgm => cgm.ContactId);
            builder.HasIndex(cgm => cgm.ContactGroupId);
            builder.HasIndex(cgm => new { cgm.ContactId, cgm.ContactGroupId }).IsUnique();

            // Relationships
            builder.HasOne(cgm => cgm.Contact)
                .WithMany(c => c.GroupMembers)
                .HasForeignKey(cgm => cgm.ContactId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cgm => cgm.ContactGroup)
                .WithMany(cg => cg.Members)
                .HasForeignKey(cgm => cgm.ContactGroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
