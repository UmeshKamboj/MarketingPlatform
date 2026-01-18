using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCampaignMessageEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContactCount",
                table: "ContactGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDynamic",
                table: "ContactGroups",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsStatic",
                table: "ContactGroups",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RuleCriteria",
                table: "ContactGroups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TimeZoneAware",
                table: "CampaignSchedules",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Channel",
                table: "CampaignMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "CostAmount",
                table: "CampaignMessages",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ExternalMessageId",
                table: "CampaignMessages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FailedAt",
                table: "CampaignMessages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HTMLContent",
                table: "CampaignMessages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxRetries",
                table: "CampaignMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Recipient",
                table: "CampaignMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                table: "CampaignMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledAt",
                table: "CampaignMessages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "CampaignMessages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Channel",
                table: "CampaignContents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "HTMLContent",
                table: "CampaignContents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PersonalizationTokens",
                table: "CampaignContents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExclusionListIds",
                table: "CampaignAudiences",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactCount",
                table: "ContactGroups");

            migrationBuilder.DropColumn(
                name: "IsDynamic",
                table: "ContactGroups");

            migrationBuilder.DropColumn(
                name: "IsStatic",
                table: "ContactGroups");

            migrationBuilder.DropColumn(
                name: "RuleCriteria",
                table: "ContactGroups");

            migrationBuilder.DropColumn(
                name: "TimeZoneAware",
                table: "CampaignSchedules");

            migrationBuilder.DropColumn(
                name: "Channel",
                table: "CampaignMessages");

            migrationBuilder.DropColumn(
                name: "CostAmount",
                table: "CampaignMessages");

            migrationBuilder.DropColumn(
                name: "ExternalMessageId",
                table: "CampaignMessages");

            migrationBuilder.DropColumn(
                name: "FailedAt",
                table: "CampaignMessages");

            migrationBuilder.DropColumn(
                name: "HTMLContent",
                table: "CampaignMessages");

            migrationBuilder.DropColumn(
                name: "MaxRetries",
                table: "CampaignMessages");

            migrationBuilder.DropColumn(
                name: "Recipient",
                table: "CampaignMessages");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                table: "CampaignMessages");

            migrationBuilder.DropColumn(
                name: "ScheduledAt",
                table: "CampaignMessages");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "CampaignMessages");

            migrationBuilder.DropColumn(
                name: "Channel",
                table: "CampaignContents");

            migrationBuilder.DropColumn(
                name: "HTMLContent",
                table: "CampaignContents");

            migrationBuilder.DropColumn(
                name: "PersonalizationTokens",
                table: "CampaignContents");

            migrationBuilder.DropColumn(
                name: "ExclusionListIds",
                table: "CampaignAudiences");
        }
    }
}
