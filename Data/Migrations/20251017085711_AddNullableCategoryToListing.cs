using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_1.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNullableCategoryToListing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bids_Listings_ListingId",
                table: "Bids");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Listings_ListingId",
                table: "Comments");


            // --- THIS BLOCK WAS REMOVED ---
            // migrationBuilder.AddColumn<string>(
            //     name: "UserId",
            //     table: "Payments",
            //     type: "nvarchar(max)",
            //     nullable: false,
            //     defaultValue: "");
            // --- END OF REMOVED BLOCK ---


            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Listings",
                type: "nvarchar(max)",
                nullable: true); // <-- This is your good code

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ListingId",
                table: "Payments",
                column: "ListingId");

            // --- CORRECTIONS ARE HERE ---
            migrationBuilder.AddForeignKey(
                name: "FK_Bids_Listings_ListingId",
                table: "Bids",
                column: "ListingId",
                principalTable: "Listings",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction); // <-- CHANGED

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Listings_ListingId",
                table: "Comments",
                column: "ListingId",
                principalTable: "Listings",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction); // <-- CHANGED

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Listings_ListingId",
                table: "Payments",
                column: "ListingId",
                principalTable: "Listings",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction); // <-- CHANGED
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bids_Listings_ListingId",
                table: "Bids");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Listings_ListingId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Listings_ListingId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_ListingId",
                table: "Payments");

            // --- THIS BLOCK WAS REMOVED ---
            // migrationBuilder.DropColumn(
            //     name: "UserId",
            //     table: "Payments");
            // --- END OF REMOVED BLOCK ---


            migrationBuilder.DropColumn(
                name: "Category",
                table: "Listings");

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_Listings_ListingId",
                table: "Bids",
                column: "ListingId",
                principalTable: "Listings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Listings_ListingId",
                table: "Comments",
                column: "ListingId",
                principalTable: "Listings",
                principalColumn: "Id");
        }
    }
}