using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSchedulingAndAutomation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FrequencyControls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContactId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    MaxMessagesPerDay = table.Column<int>(type: "int", nullable: false, defaultValue: 5),
                    MaxMessagesPerWeek = table.Column<int>(type: "int", nullable: false, defaultValue: 20),
                    MaxMessagesPerMonth = table.Column<int>(type: "int", nullable: false, defaultValue: 50),
                    LastMessageSentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MessagesSentToday = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    MessagesSentThisWeek = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    MessagesSentThisMonth = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrequencyControls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FrequencyControls_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FrequencyControls_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FrequencyControls_ContactId",
                table: "FrequencyControls",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_FrequencyControls_ContactId_UserId",
                table: "FrequencyControls",
                columns: new[] { "ContactId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FrequencyControls_UserId",
                table: "FrequencyControls",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FrequencyControls");
        }
    }
}
