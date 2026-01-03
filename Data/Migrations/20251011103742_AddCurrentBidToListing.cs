using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_1.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrentBidToListing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CurrentBid",
                table: "Listings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentBid",
                table: "Listings");
        }
    }
}
