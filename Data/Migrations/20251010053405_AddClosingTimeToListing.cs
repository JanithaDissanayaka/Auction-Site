using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_1.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddClosingTimeToListing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Listings_Listingid",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Listings",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Listingid",
                table: "Comments",
                newName: "ListingId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_Listingid",
                table: "Comments",
                newName: "IX_Comments_ListingId");

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosingTime",
                table: "Listings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Listings_ListingId",
                table: "Comments",
                column: "ListingId",
                principalTable: "Listings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Listings_ListingId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ClosingTime",
                table: "Listings");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Listings",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ListingId",
                table: "Comments",
                newName: "Listingid");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_ListingId",
                table: "Comments",
                newName: "IX_Comments_Listingid");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Listings_Listingid",
                table: "Comments",
                column: "Listingid",
                principalTable: "Listings",
                principalColumn: "id");
        }
    }
}
