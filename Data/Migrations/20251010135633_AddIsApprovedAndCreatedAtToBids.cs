using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_1.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsApprovedAndCreatedAtToBids : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Bids",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Bids",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Bids");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Bids");
        }
    }
}
