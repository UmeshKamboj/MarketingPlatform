using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddComplianceAndConsentManagement : Migration
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

            migrationBuilder.AddColumn<bool>(
                name: "EmailOptIn",
                table: "Contacts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailOptInDate",
                table: "Contacts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MmsOptIn",
                table: "Contacts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "MmsOptInDate",
                table: "Contacts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SmsOptIn",
                table: "Contacts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SmsOptInDate",
                table: "Contacts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Channel",
                table: "ConsentHistories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "ConsentHistories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                table: "ConsentHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ConsentRetentionDays",
                table: "ComplianceSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "EnableAuditLogging",
                table: "ComplianceSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EnableConsentTracking",
                table: "ComplianceSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EnforceSuppressionList",
                table: "ComplianceSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OptInConfirmationMessage",
                table: "ComplianceSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OptInKeywords",
                table: "ComplianceSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OptOutConfirmationMessage",
                table: "ComplianceSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OptOutKeywords",
                table: "ComplianceSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuietHoursTimeZone",
                table: "ComplianceSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequireDoubleOptInEmail",
                table: "ComplianceSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequireDoubleOptInSms",
                table: "ComplianceSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TermsOfServiceUrl",
                table: "ComplianceSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ComplianceAuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContactId = table.Column<int>(type: "int", nullable: true),
                    CampaignId = table.Column<int>(type: "int", nullable: true),
                    ActionType = table.Column<int>(type: "int", nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplianceAuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComplianceAuditLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComplianceAuditLogs_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ComplianceAuditLogs_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ContactConsents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContactId = table.Column<int>(type: "int", nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Source = table.Column<int>(type: "int", nullable: false),
                    ConsentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactConsents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContactConsents_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceAuditLogs_CampaignId",
                table: "ComplianceAuditLogs",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceAuditLogs_ContactId",
                table: "ComplianceAuditLogs",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceAuditLogs_UserId",
                table: "ComplianceAuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactConsents_ContactId",
                table: "ContactConsents",
                column: "ContactId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComplianceAuditLogs");

            migrationBuilder.DropTable(
                name: "ContactConsents");

            migrationBuilder.DropColumn(
                name: "EmailOptIn",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "EmailOptInDate",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "MmsOptIn",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "MmsOptInDate",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "SmsOptIn",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "SmsOptInDate",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Channel",
                table: "ConsentHistories");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "ConsentHistories");

            migrationBuilder.DropColumn(
                name: "UserAgent",
                table: "ConsentHistories");

            migrationBuilder.DropColumn(
                name: "ConsentRetentionDays",
                table: "ComplianceSettings");

            migrationBuilder.DropColumn(
                name: "EnableAuditLogging",
                table: "ComplianceSettings");

            migrationBuilder.DropColumn(
                name: "EnableConsentTracking",
                table: "ComplianceSettings");

            migrationBuilder.DropColumn(
                name: "EnforceSuppressionList",
                table: "ComplianceSettings");

            migrationBuilder.DropColumn(
                name: "OptInConfirmationMessage",
                table: "ComplianceSettings");

            migrationBuilder.DropColumn(
                name: "OptInKeywords",
                table: "ComplianceSettings");

            migrationBuilder.DropColumn(
                name: "OptOutConfirmationMessage",
                table: "ComplianceSettings");

            migrationBuilder.DropColumn(
                name: "OptOutKeywords",
                table: "ComplianceSettings");

            migrationBuilder.DropColumn(
                name: "QuietHoursTimeZone",
                table: "ComplianceSettings");

            migrationBuilder.DropColumn(
                name: "RequireDoubleOptInEmail",
                table: "ComplianceSettings");

            migrationBuilder.DropColumn(
                name: "RequireDoubleOptInSms",
                table: "ComplianceSettings");

            migrationBuilder.DropColumn(
                name: "TermsOfServiceUrl",
                table: "ComplianceSettings");

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
