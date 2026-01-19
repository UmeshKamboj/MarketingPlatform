using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPageContentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PageContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MetaDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrls = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageContents_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PageContents_PageKey",
                table: "PageContents",
                column: "PageKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PageContents_LastModifiedBy",
                table: "PageContents",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PageContents_IsPublished",
                table: "PageContents",
                column: "IsPublished");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageContents");
        }
    }
}
