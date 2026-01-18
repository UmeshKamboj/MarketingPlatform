using MarketingPlatform.Core.Entities;
using MarketingPlatform.Core.Enums;

namespace MarketingPlatform.Application.Interfaces
{
    public interface IWorkflowService
    {
        /// <summary>
        /// Execute a workflow for a specific contact
        /// </summary>
        Task ExecuteWorkflowAsync(int workflowId, int contactId);

        /// <summary>
        /// Process the next step in a workflow execution
        /// </summary>
        Task ProcessNextStepAsync(int executionId);

        /// <summary>
        /// Pause a running workflow execution
        /// </summary>
        Task PauseWorkflowExecutionAsync(int executionId);

        /// <summary>
        /// Resume a paused workflow execution
        /// </summary>
        Task ResumeWorkflowExecutionAsync(int executionId);

        /// <summary>
        /// Cancel a workflow execution
        /// </summary>
        Task CancelWorkflowExecutionAsync(int executionId);

        /// <summary>
        /// Get all active workflow executions for a contact
        /// </summary>
        Task<List<WorkflowExecution>> GetActiveExecutionsForContactAsync(int contactId);
    }
}
