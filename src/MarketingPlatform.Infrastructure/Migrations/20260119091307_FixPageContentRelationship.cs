using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPageContentRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PageContents_AspNetUsers_LastModifiedByUserId",
                table: "PageContents");

            migrationBuilder.DropIndex(
                name: "IX_PageContents_LastModifiedByUserId",
                table: "PageContents");

            migrationBuilder.DropColumn(
                name: "LastModifiedByUserId",
                table: "PageContents");

            migrationBuilder.AlterColumn<string>(
                name: "PageKey",
                table: "PageContents",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                table: "PageContents",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PageContents_LastModifiedBy",
                table: "PageContents",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PageContents_PageKey",
                table: "PageContents",
                column: "PageKey",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PageContents_AspNetUsers_LastModifiedBy",
                table: "PageContents",
                column: "LastModifiedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PageContents_AspNetUsers_LastModifiedBy",
                table: "PageContents");

            migrationBuilder.DropIndex(
                name: "IX_PageContents_LastModifiedBy",
                table: "PageContents");

            migrationBuilder.DropIndex(
                name: "IX_PageContents_PageKey",
                table: "PageContents");

            migrationBuilder.AlterColumn<string>(
                name: "PageKey",
                table: "PageContents",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                table: "PageContents",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedByUserId",
                table: "PageContents",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PageContents_LastModifiedByUserId",
                table: "PageContents",
                column: "LastModifiedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PageContents_AspNetUsers_LastModifiedByUserId",
                table: "PageContents",
                column: "LastModifiedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
