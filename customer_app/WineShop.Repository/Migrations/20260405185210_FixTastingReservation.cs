using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WineShop.Repository.Migrations
{
    /// <inheritdoc />
    public partial class FixTastingReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TastingPackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DurationHours = table.Column<int>(type: "int", nullable: false),
                    MaxGuests = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    BlocksWholeDay = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TastingPackages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TastingReservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReservationStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReservationEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumberOfGuests = table.Column<int>(type: "int", nullable: false),
                    TastingPackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TastingReservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TastingReservations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TastingReservations_TastingPackages_TastingPackageId",
                        column: x => x.TastingPackageId,
                        principalTable: "TastingPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TastingReservations_TastingPackageId",
                table: "TastingReservations",
                column: "TastingPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_TastingReservations_UserId",
                table: "TastingReservations",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TastingReservations");

            migrationBuilder.DropTable(
                name: "TastingPackages");
        }
    }
}
