using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJourneyDesignerFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BranchCondition",
                table: "WorkflowSteps",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NextNodeOnFalse",
                table: "WorkflowSteps",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NextNodeOnTrue",
                table: "WorkflowSteps",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NodeLabel",
                table: "WorkflowSteps",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PositionX",
                table: "WorkflowSteps",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PositionY",
                table: "WorkflowSteps",
                type: "float",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSteps_WorkflowId_StepOrder",
                table: "WorkflowSteps",
                columns: new[] { "WorkflowId", "StepOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkflowSteps_WorkflowId_StepOrder",
                table: "WorkflowSteps");

            migrationBuilder.DropColumn(
                name: "BranchCondition",
                table: "WorkflowSteps");

            migrationBuilder.DropColumn(
                name: "NextNodeOnFalse",
                table: "WorkflowSteps");

            migrationBuilder.DropColumn(
                name: "NextNodeOnTrue",
                table: "WorkflowSteps");

            migrationBuilder.DropColumn(
                name: "NodeLabel",
                table: "WorkflowSteps");

            migrationBuilder.DropColumn(
                name: "PositionX",
                table: "WorkflowSteps");

            migrationBuilder.DropColumn(
                name: "PositionY",
                table: "WorkflowSteps");
        }
    }
}
