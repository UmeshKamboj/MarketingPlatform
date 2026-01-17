using MarketingPlatform.Core.Enums;

namespace MarketingPlatform.Core.Entities
{
    public class WorkflowStep : BaseEntity
    {
        public int WorkflowId { get; set; }
        public int StepOrder { get; set; }
        public WorkflowActionType ActionType { get; set; }
        public string? ActionConfiguration { get; set; } // JSON
        public int? DelayMinutes { get; set; }

        // Navigation properties
        public virtual Workflow Workflow { get; set; } = null!;
    }
}
