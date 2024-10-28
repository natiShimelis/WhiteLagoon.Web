using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WhiteLagoon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedVillaToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Villas",
                columns: new[] { "Id", "Created_Date", "Description", "ImageUrl", "Name", "Occupancy", "Price", "Sqft", "Updated_Date" },
                values: new object[,]
                {
                    { 1, null, "A beautiful villa", "https://example.com/villa.jpg", "Royal Villa", 4, 200.0, 1500, null },
                    { 2, null, "A beautiful villa with nice pool", "https://example.com/anothervilla.jpg", "Premium pool Villa", 6, 250.0, 2000, null },
                    { 3, null, "A luxurious villa with an amazing pool", "https://example.com/thirdvilla.jpg", "Luxury pool Villa", 8, 300.0, 2500, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
