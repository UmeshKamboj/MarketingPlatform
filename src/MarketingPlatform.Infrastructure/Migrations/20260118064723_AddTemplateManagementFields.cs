using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTemplateManagementFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MediaUrls",
                table: "MessageTemplates",
                newName: "TemplateVariables");

            migrationBuilder.AlterColumn<int>(
                name: "Category",
                table: "MessageTemplates",
                type: "int",
                maxLength: 100,
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefaultMediaUrls",
                table: "MessageTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "MessageTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HTMLContent",
                table: "MessageTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "MessageTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUsedAt",
                table: "MessageTemplates",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsageCount",
                table: "MessageTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddColumn<decimal>(
                name: "CostAmount",
                table: "CampaignMessages",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

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
                name: "DefaultMediaUrls",
                table: "MessageTemplates");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "MessageTemplates");

            migrationBuilder.DropColumn(
                name: "HTMLContent",
                table: "MessageTemplates");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "MessageTemplates");

            migrationBuilder.DropColumn(
                name: "LastUsedAt",
                table: "MessageTemplates");

            migrationBuilder.DropColumn(
                name: "UsageCount",
                table: "MessageTemplates");

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
                name: "CostAmount",
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

            migrationBuilder.RenameColumn(
                name: "TemplateVariables",
                table: "MessageTemplates",
                newName: "MediaUrls");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "MessageTemplates",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 100);
        }
    }
}
