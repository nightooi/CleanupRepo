using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace events.infra.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqunessWeirdnessAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_categories_id",
                table: "Events");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Events",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_CategoryId",
                table: "Events",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_categories_CategoryId",
                table: "Events",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_categories_CategoryId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_CategoryId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Events");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_categories_id",
                table: "Events",
                column: "id",
                principalTable: "categories",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
