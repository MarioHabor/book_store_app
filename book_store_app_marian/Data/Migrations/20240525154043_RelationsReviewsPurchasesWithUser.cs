using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace book_store_app_marian.Data.Migrations
{
    /// <inheritdoc />
    public partial class RelationsReviewsPurchasesWithUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Likes",
                table: "Reviews",
                newName: "Rating");

            migrationBuilder.AddColumn<Guid>(
                name: "PurchaseId",
                table: "Reviews",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_PurchaseId",
                table: "Reviews",
                column: "PurchaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Purchases_PurchaseId",
                table: "Reviews",
                column: "PurchaseId",
                principalTable: "Purchases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Purchases_PurchaseId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_PurchaseId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "PurchaseId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Rating",
                table: "Reviews",
                newName: "Likes");
        }
    }
}
