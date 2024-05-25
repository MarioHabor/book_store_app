using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace book_store_app_marian.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateColumnReviewsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Likes",
                table: "Reviews",
                newName: "Rating");

            migrationBuilder.AddColumn<Guid>(
                name: "PurchasesId",
                table: "Reviews",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_PurchasesId",
                table: "Reviews",
                column: "PurchasesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Purchases_PurchasesId",
                table: "Reviews",
                column: "PurchasesId",
                principalTable: "Purchases",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Purchases_PurchasesId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_PurchasesId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "PurchasesId",
                table: "Reviews");

            migrationBuilder.RenameColumn(
                name: "Rating",
                table: "Reviews",
                newName: "Likes");
        }
    }
}
