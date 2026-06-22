using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WineShop.Repository.Migrations
{
    /// <inheritdoc />
    public partial class SeedTastingPackages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TastingPackages",
                columns: new[] { "Id", "BlocksWholeDay", "Description", "DurationHours", "MaxGuests", "Name", "Price" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), false, "Прошетка низ винарија, дегустација на селектирани вина и храна на даска.", 3, 8, "Класична дегустација", 1500 },
                    { new Guid("22222222-2222-2222-2222-222222222222"), true, "Ексклузивна дегустација со водена тура и богато гастрономско искуство.", 3, 8, "Премиум искуство", 2800 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TastingPackages",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "TastingPackages",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));
        }
    }
}
