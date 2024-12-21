using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HotelBookingApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "Guests",
            //    columns: table => new
            //    {
            //        GuestId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Guests", x => x.GuestId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Rooms",
            //    columns: table => new
            //    {
            //        RoomId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        ExtraBeds = table.Column<int>(type: "int", nullable: false),
            //        IsAvailable = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Rooms", x => x.RoomId);
            //    });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    BookingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheckInDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckOutDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    GuestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.BookingId);
                    table.ForeignKey(
                        name: "FK_Bookings_Guests_GuestId",
                        column: x => x.GuestId,
                        principalTable: "Guests",
                        principalColumn: "GuestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                });

            //migrationBuilder.InsertData(
            //    table: "Guests",
            //    columns: new[] { "GuestId", "Email", "FirstName", "LastName", "PhoneNumber" },
            //    values: new object[,]
            //    {
            //        { 1, "alice@example.com", "Alice", "Smith", "1234567890" },
            //        { 2, "bob@example.com", "Bob", "Johnson", "2345678901" },
            //        { 3, "charlie@example.com", "Charlie", "Brown", "3456789012" },
            //        { 4, "diana@example.com", "Diana", "Green", "4567890123" }
            //    });

            //migrationBuilder.InsertData(
            //    table: "Rooms",
            //    columns: new[] { "RoomId", "ExtraBeds", "IsAvailable", "Type" },
            //    values: new object[,]
            //    {
            //        { 1, 0, false, "Single" },
            //        { 2, 1, false, "Double" },
            //        { 3, 2, false, "Double" },
            //        { 4, 0, false, "Single" }
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Bookings_GuestId",
            //    table: "Bookings",
            //    column: "GuestId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Bookings_RoomId",
            //    table: "Bookings",
            //    column: "RoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            //migrationBuilder.DropTable(
            //    name: "Guests");

            //migrationBuilder.DropTable(
            //    name: "Rooms");
        }
    }
}
