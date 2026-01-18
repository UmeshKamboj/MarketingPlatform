using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddKeywordManagementAndPricingFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KeywordAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeywordId = table.Column<int>(type: "int", nullable: false),
                    CampaignId = table.Column<int>(type: "int", nullable: false),
                    AssignedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UnassignedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeywordAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeywordAssignments_AspNetUsers_AssignedByUserId",
                        column: x => x.AssignedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KeywordAssignments_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KeywordAssignments_Keywords_KeywordId",
                        column: x => x.KeywordId,
                        principalTable: "Keywords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KeywordConflicts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeywordText = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequestingUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ExistingUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ResolvedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Resolution = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeywordConflicts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeywordConflicts_AspNetUsers_ExistingUserId",
                        column: x => x.ExistingUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KeywordConflicts_AspNetUsers_RequestingUserId",
                        column: x => x.RequestingUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KeywordConflicts_AspNetUsers_ResolvedByUserId",
                        column: x => x.ResolvedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KeywordReservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeywordText = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequestedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ApprovedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeywordReservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeywordReservations_AspNetUsers_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KeywordReservations_AspNetUsers_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PricingModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BillingPeriod = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Configuration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaxConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    RegionCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Rate = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    FlatAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsPercentage = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Configuration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChannelPricings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PricingModelId = table.Column<int>(type: "int", nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    PricePerUnit = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    MinimumCharge = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FreeUnitsIncluded = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Configuration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelPricings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChannelPricings_PricingModels_PricingModelId",
                        column: x => x.PricingModelId,
                        principalTable: "PricingModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegionPricings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PricingModelId = table.Column<int>(type: "int", nullable: false),
                    RegionCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RegionName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PriceMultiplier = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    FlatAdjustment = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Configuration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionPricings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegionPricings_PricingModels_PricingModelId",
                        column: x => x.PricingModelId,
                        principalTable: "PricingModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsagePricings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PricingModelId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    TierStart = table.Column<int>(type: "int", nullable: false),
                    TierEnd = table.Column<int>(type: "int", nullable: true),
                    PricePerUnit = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Configuration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsagePricings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsagePricings_PricingModels_PricingModelId",
                        column: x => x.PricingModelId,
                        principalTable: "PricingModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChannelPricings_IsActive",
                table: "ChannelPricings",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelPricings_PricingModelId_Channel",
                table: "ChannelPricings",
                columns: new[] { "PricingModelId", "Channel" });

            migrationBuilder.CreateIndex(
                name: "IX_KeywordAssignments_AssignedByUserId",
                table: "KeywordAssignments",
                column: "AssignedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_KeywordAssignments_CampaignId",
                table: "KeywordAssignments",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_KeywordAssignments_IsActive",
                table: "KeywordAssignments",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_KeywordAssignments_KeywordId",
                table: "KeywordAssignments",
                column: "KeywordId");

            migrationBuilder.CreateIndex(
                name: "IX_KeywordConflicts_ExistingUserId",
                table: "KeywordConflicts",
                column: "ExistingUserId");

            migrationBuilder.CreateIndex(
                name: "IX_KeywordConflicts_KeywordText",
                table: "KeywordConflicts",
                column: "KeywordText");

            migrationBuilder.CreateIndex(
                name: "IX_KeywordConflicts_RequestingUserId",
                table: "KeywordConflicts",
                column: "RequestingUserId");

            migrationBuilder.CreateIndex(
                name: "IX_KeywordConflicts_ResolvedByUserId",
                table: "KeywordConflicts",
                column: "ResolvedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_KeywordConflicts_Status",
                table: "KeywordConflicts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_KeywordReservations_ApprovedByUserId",
                table: "KeywordReservations",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_KeywordReservations_KeywordText",
                table: "KeywordReservations",
                column: "KeywordText");

            migrationBuilder.CreateIndex(
                name: "IX_KeywordReservations_RequestedByUserId",
                table: "KeywordReservations",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_KeywordReservations_Status",
                table: "KeywordReservations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PricingModels_IsActive",
                table: "PricingModels",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PricingModels_Name",
                table: "PricingModels",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_PricingModels_Priority",
                table: "PricingModels",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_RegionPricings_IsActive",
                table: "RegionPricings",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_RegionPricings_PricingModelId_RegionCode",
                table: "RegionPricings",
                columns: new[] { "PricingModelId", "RegionCode" });

            migrationBuilder.CreateIndex(
                name: "IX_TaxConfigurations_IsActive",
                table: "TaxConfigurations",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TaxConfigurations_Name",
                table: "TaxConfigurations",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_TaxConfigurations_Priority",
                table: "TaxConfigurations",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_TaxConfigurations_RegionCode",
                table: "TaxConfigurations",
                column: "RegionCode");

            migrationBuilder.CreateIndex(
                name: "IX_TaxConfigurations_Type",
                table: "TaxConfigurations",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_UsagePricings_IsActive",
                table: "UsagePricings",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_UsagePricings_PricingModelId_Type_TierStart",
                table: "UsagePricings",
                columns: new[] { "PricingModelId", "Type", "TierStart" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChannelPricings");

            migrationBuilder.DropTable(
                name: "KeywordAssignments");

            migrationBuilder.DropTable(
                name: "KeywordConflicts");

            migrationBuilder.DropTable(
                name: "KeywordReservations");

            migrationBuilder.DropTable(
                name: "RegionPricings");

            migrationBuilder.DropTable(
                name: "TaxConfigurations");

            migrationBuilder.DropTable(
                name: "UsagePricings");

            migrationBuilder.DropTable(
                name: "PricingModels");
        }
    }
}
