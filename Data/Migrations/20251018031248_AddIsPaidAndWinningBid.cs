using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_1.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPaidAndWinningBid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WinningBidId",
                table: "Listings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "Bids",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WinningBidId",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "Bids");
        }
    }
}
