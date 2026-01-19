using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadeConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactGroupMembers_ContactGroups_ContactGroupId",
                table: "ContactGroupMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactTagAssignments_ContactTags_ContactTagId",
                table: "ContactTagAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_KeywordAssignments_Campaigns_CampaignId",
                table: "KeywordAssignments");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactGroupMembers_ContactGroups_ContactGroupId",
                table: "ContactGroupMembers",
                column: "ContactGroupId",
                principalTable: "ContactGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContactTagAssignments_ContactTags_ContactTagId",
                table: "ContactTagAssignments",
                column: "ContactTagId",
                principalTable: "ContactTags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_KeywordAssignments_Campaigns_CampaignId",
                table: "KeywordAssignments",
                column: "CampaignId",
                principalTable: "Campaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactGroupMembers_ContactGroups_ContactGroupId",
                table: "ContactGroupMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactTagAssignments_ContactTags_ContactTagId",
                table: "ContactTagAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_KeywordAssignments_Campaigns_CampaignId",
                table: "KeywordAssignments");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactGroupMembers_ContactGroups_ContactGroupId",
                table: "ContactGroupMembers",
                column: "ContactGroupId",
                principalTable: "ContactGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContactTagAssignments_ContactTags_ContactTagId",
                table: "ContactTagAssignments",
                column: "ContactTagId",
                principalTable: "ContactTags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KeywordAssignments_Campaigns_CampaignId",
                table: "KeywordAssignments",
                column: "CampaignId",
                principalTable: "Campaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
