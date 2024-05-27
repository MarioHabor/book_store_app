using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace book_store_app_marian.Data.Migrations
{
    /// <inheritdoc />
    public partial class SecondCreateNewColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTimestamp",
                table: "Purchases",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "Purchases",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "Products",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedTimestamp",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Products");
        }
    }
}
