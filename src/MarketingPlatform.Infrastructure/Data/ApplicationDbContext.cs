using MarketingPlatform.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MarketingPlatform.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<UserActivityLog> UserActivityLogs => Set<UserActivityLog>();
        public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
        public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();
        public DbSet<UsageTracking> UsageTrackings => Set<UsageTracking>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<BillingHistory> BillingHistories => Set<BillingHistory>();
        public DbSet<Contact> Contacts => Set<Contact>();
        public DbSet<ContactGroup> ContactGroups => Set<ContactGroup>();
        public DbSet<ContactGroupMember> ContactGroupMembers => Set<ContactGroupMember>();
        public DbSet<ContactTag> ContactTags => Set<ContactTag>();
        public DbSet<ContactTagAssignment> ContactTagAssignments => Set<ContactTagAssignment>();
        public DbSet<SuppressionList> SuppressionLists => Set<SuppressionList>();
        public DbSet<ConsentHistory> ConsentHistories => Set<ConsentHistory>();
        public DbSet<Campaign> Campaigns => Set<Campaign>();
        public DbSet<CampaignContent> CampaignContents => Set<CampaignContent>();
        public DbSet<CampaignAudience> CampaignAudiences => Set<CampaignAudience>();
        public DbSet<CampaignSchedule> CampaignSchedules => Set<CampaignSchedule>();
        public DbSet<CampaignMessage> CampaignMessages => Set<CampaignMessage>();
        public DbSet<MessageTemplate> MessageTemplates => Set<MessageTemplate>();
        public DbSet<Keyword> Keywords => Set<Keyword>();
        public DbSet<KeywordActivity> KeywordActivities => Set<KeywordActivity>();
        public DbSet<Workflow> Workflows => Set<Workflow>();
        public DbSet<WorkflowStep> WorkflowSteps => Set<WorkflowStep>();
        public DbSet<WorkflowExecution> WorkflowExecutions => Set<WorkflowExecution>();
        public DbSet<URLShortener> URLShorteners => Set<URLShortener>();
        public DbSet<URLClick> URLClicks => Set<URLClick>();
        public DbSet<CampaignAnalytics> CampaignAnalytics => Set<CampaignAnalytics>();
        public DbSet<ContactEngagement> ContactEngagements => Set<ContactEngagement>();
        public DbSet<MessageProvider> MessageProviders => Set<MessageProvider>();
        public DbSet<ProviderLog> ProviderLogs => Set<ProviderLog>();
        public DbSet<ComplianceSettings> ComplianceSettings => Set<ComplianceSettings>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all entity configurations from assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // Global query filters for soft delete
            modelBuilder.Entity<Contact>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Campaign>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<ContactGroup>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<MessageTemplate>().HasQueryFilter(m => !m.IsDeleted);
            modelBuilder.Entity<Workflow>().HasQueryFilter(w => !w.IsDeleted);
        }
    }
}
