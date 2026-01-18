using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageRoutingAndDeliveryTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastMessageSentAt",
                table: "FrequencyControls",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateTable(
                name: "ChannelRoutingConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    PrimaryProvider = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FallbackProvider = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RoutingStrategy = table.Column<int>(type: "int", nullable: false),
                    EnableFallback = table.Column<bool>(type: "bit", nullable: false),
                    MaxRetries = table.Column<int>(type: "int", nullable: false),
                    RetryStrategy = table.Column<int>(type: "int", nullable: false),
                    InitialRetryDelaySeconds = table.Column<int>(type: "int", nullable: false),
                    MaxRetryDelaySeconds = table.Column<int>(type: "int", nullable: false),
                    CostThreshold = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    AdditionalSettings = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelRoutingConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageDeliveryAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampaignMessageId = table.Column<int>(type: "int", nullable: false),
                    AttemptNumber = table.Column<int>(type: "int", nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    ProviderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AttemptedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Success = table.Column<bool>(type: "bit", nullable: false),
                    ExternalMessageId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ErrorCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CostAmount = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    ResponseTimeMs = table.Column<int>(type: "int", nullable: false),
                    FallbackReason = table.Column<int>(type: "int", nullable: true),
                    AdditionalMetadata = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageDeliveryAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageDeliveryAttempts_CampaignMessages_CampaignMessageId",
                        column: x => x.CampaignMessageId,
                        principalTable: "CampaignMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChannelRoutingConfigs_Channel",
                table: "ChannelRoutingConfigs",
                column: "Channel");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelRoutingConfigs_Channel_IsActive_Priority",
                table: "ChannelRoutingConfigs",
                columns: new[] { "Channel", "IsActive", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_MessageDeliveryAttempts_AttemptedAt",
                table: "MessageDeliveryAttempts",
                column: "AttemptedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MessageDeliveryAttempts_CampaignMessageId",
                table: "MessageDeliveryAttempts",
                column: "CampaignMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageDeliveryAttempts_CampaignMessageId_AttemptNumber",
                table: "MessageDeliveryAttempts",
                columns: new[] { "CampaignMessageId", "AttemptNumber" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChannelRoutingConfigs");

            migrationBuilder.DropTable(
                name: "MessageDeliveryAttempts");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastMessageSentAt",
                table: "FrequencyControls",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
