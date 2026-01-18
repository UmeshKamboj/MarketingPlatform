using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignABTesting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalSettings",
                table: "ComplianceAuditLogs");

            migrationBuilder.DropColumn(
                name: "CostThreshold",
                table: "ComplianceAuditLogs");

            migrationBuilder.AddColumn<DateTime>(
                name: "ABTestEndDate",
                table: "Campaigns",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsABTest",
                table: "Campaigns",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "WinningVariantId",
                table: "Campaigns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VariantId",
                table: "CampaignMessages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CampaignVariants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampaignId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TrafficPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    IsControl = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MessageBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HTMLContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MediaUrls = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageTemplateId = table.Column<int>(type: "int", nullable: true),
                    PersonalizationTokens = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecipientCount = table.Column<int>(type: "int", nullable: false),
                    SentCount = table.Column<int>(type: "int", nullable: false),
                    DeliveredCount = table.Column<int>(type: "int", nullable: false),
                    FailedCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignVariants_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CampaignVariants_MessageTemplates_MessageTemplateId",
                        column: x => x.MessageTemplateId,
                        principalTable: "MessageTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CampaignVariantAnalytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampaignVariantId = table.Column<int>(type: "int", nullable: false),
                    TotalSent = table.Column<int>(type: "int", nullable: false),
                    TotalDelivered = table.Column<int>(type: "int", nullable: false),
                    TotalFailed = table.Column<int>(type: "int", nullable: false),
                    TotalClicks = table.Column<int>(type: "int", nullable: false),
                    TotalOptOuts = table.Column<int>(type: "int", nullable: false),
                    TotalBounces = table.Column<int>(type: "int", nullable: false),
                    TotalOpens = table.Column<int>(type: "int", nullable: false),
                    TotalReplies = table.Column<int>(type: "int", nullable: false),
                    TotalConversions = table.Column<int>(type: "int", nullable: false),
                    DeliveryRate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    ClickRate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    OptOutRate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    OpenRate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    BounceRate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    ConversionRate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    ConfidenceLevel = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    IsStatisticallySignificant = table.Column<bool>(type: "bit", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignVariantAnalytics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignVariantAnalytics_CampaignVariants_CampaignVariantId",
                        column: x => x.CampaignVariantId,
                        principalTable: "CampaignVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignMessages_VariantId",
                table: "CampaignMessages",
                column: "VariantId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignVariantAnalytics_CampaignVariantId",
                table: "CampaignVariantAnalytics",
                column: "CampaignVariantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CampaignVariants_CampaignId",
                table: "CampaignVariants",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignVariants_CampaignId_IsControl",
                table: "CampaignVariants",
                columns: new[] { "CampaignId", "IsControl" });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignVariants_MessageTemplateId",
                table: "CampaignVariants",
                column: "MessageTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_CampaignMessages_CampaignVariants_VariantId",
                table: "CampaignMessages",
                column: "VariantId",
                principalTable: "CampaignVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CampaignMessages_CampaignVariants_VariantId",
                table: "CampaignMessages");

            migrationBuilder.DropTable(
                name: "CampaignVariantAnalytics");

            migrationBuilder.DropTable(
                name: "CampaignVariants");

            migrationBuilder.DropIndex(
                name: "IX_CampaignMessages_VariantId",
                table: "CampaignMessages");

            migrationBuilder.DropColumn(
                name: "ABTestEndDate",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "IsABTest",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "WinningVariantId",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "VariantId",
                table: "CampaignMessages");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalSettings",
                table: "ComplianceAuditLogs",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CostThreshold",
                table: "ComplianceAuditLogs",
                type: "decimal(18,6)",
                nullable: true);
        }
    }
}
