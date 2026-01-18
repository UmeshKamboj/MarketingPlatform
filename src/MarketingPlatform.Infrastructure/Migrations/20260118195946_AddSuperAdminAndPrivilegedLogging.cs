using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSuperAdminAndPrivilegedLogging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlatformConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsEncrypted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlatformConfigurations_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrivilegedActionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionType = table.Column<int>(type: "int", nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    PerformedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EntityId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    EntityName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ActionDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    BeforeState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AfterState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequestPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Success = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivilegedActionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrivilegedActionLogs_AspNetUsers_PerformedBy",
                        column: x => x.PerformedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SuperAdminRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    AssignmentReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevokedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RevocationReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuperAdminRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SuperAdminRoles_AspNetUsers_AssignedBy",
                        column: x => x.AssignedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SuperAdminRoles_AspNetUsers_RevokedBy",
                        column: x => x.RevokedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SuperAdminRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlatformConfigurations_Category",
                table: "PlatformConfigurations",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformConfigurations_IsActive",
                table: "PlatformConfigurations",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformConfigurations_Key",
                table: "PlatformConfigurations",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlatformConfigurations_LastModifiedBy",
                table: "PlatformConfigurations",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PrivilegedActionLogs_ActionType",
                table: "PrivilegedActionLogs",
                column: "ActionType");

            migrationBuilder.CreateIndex(
                name: "IX_PrivilegedActionLogs_EntityType",
                table: "PrivilegedActionLogs",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_PrivilegedActionLogs_EntityType_EntityId",
                table: "PrivilegedActionLogs",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_PrivilegedActionLogs_PerformedBy",
                table: "PrivilegedActionLogs",
                column: "PerformedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PrivilegedActionLogs_Severity",
                table: "PrivilegedActionLogs",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_PrivilegedActionLogs_Timestamp",
                table: "PrivilegedActionLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_SuperAdminRoles_AssignedAt",
                table: "SuperAdminRoles",
                column: "AssignedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SuperAdminRoles_AssignedBy",
                table: "SuperAdminRoles",
                column: "AssignedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SuperAdminRoles_IsActive",
                table: "SuperAdminRoles",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_SuperAdminRoles_RevokedBy",
                table: "SuperAdminRoles",
                column: "RevokedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SuperAdminRoles_UserId",
                table: "SuperAdminRoles",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlatformConfigurations");

            migrationBuilder.DropTable(
                name: "PrivilegedActionLogs");

            migrationBuilder.DropTable(
                name: "SuperAdminRoles");
        }
    }
}
