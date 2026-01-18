using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEncryptionAuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EncryptionAuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Operation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: true),
                    FieldName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    KeyVersion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Success = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OperationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncryptionAuditLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EncryptionAuditLogs_EntityId",
                table: "EncryptionAuditLogs",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EncryptionAuditLogs_EntityType",
                table: "EncryptionAuditLogs",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_EncryptionAuditLogs_Operation",
                table: "EncryptionAuditLogs",
                column: "Operation");

            migrationBuilder.CreateIndex(
                name: "IX_EncryptionAuditLogs_OperationTimestamp",
                table: "EncryptionAuditLogs",
                column: "OperationTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_EncryptionAuditLogs_UserId",
                table: "EncryptionAuditLogs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EncryptionAuditLogs");
        }
    }
}
