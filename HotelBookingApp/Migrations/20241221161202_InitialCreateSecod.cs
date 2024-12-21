using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HotelBookingApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateSecod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Guests",
                columns: table => new
                {
                    GuestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guests", x => x.GuestId);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    RoomId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExtraBeds = table.Column<int>(type: "int", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    TotalPeople = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExtraBedPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PricePerNight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SizeInSquareMeters = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.RoomId);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    BookingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    GuestId = table.Column<int>(type: "int", nullable: false),
                    CheckInDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CheckOutDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCheckedIn = table.Column<bool>(type: "bit", nullable: false),
                    IsCheckedOut = table.Column<bool>(type: "bit", nullable: false),
                    BookingStatus = table.Column<bool>(type: "bit", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    InvoiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    PaymentDeadline = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.InvoiceId);
                    table.ForeignKey(
                        name: "FK_Invoices_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<int>(type: "int", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payments_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "InvoiceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Guests",
                columns: new[] { "GuestId", "Email", "FirstName", "LastName", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, "gmail.com1", "p1", "l1", "11111" },
                    { 2, "gmail.com2", "p2", "l2", "22222" },
                    { 3, "gmail.com3", "p3", "l3", "33333" },
                    { 4, "gmail.com4", "p4", "l4", "44444" }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "RoomId", "ExtraBedPrice", "ExtraBeds", "IsAvailable", "PricePerNight", "SizeInSquareMeters", "TotalPeople", "Type" },
                values: new object[,]
                {
                    { 1, 0m, 0, false, 1500m, 20, 0m, "Single" },
                    { 2, 0m, 1, false, 3500m, 80, 0m, "Double" },
                    { 3, 0m, 0, true, 2000m, 25, 0m, "Single" },
                    { 4, 0m, 2, true, 4000m, 90, 0m, "Double" }
                });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "BookingId", "BookingStatus", "CheckInDate", "CheckOutDate", "GuestId", "IsCheckedIn", "IsCheckedOut", "RoomId" },
                values: new object[,]
                {
                    { 1, false, new DateTime(2024, 12, 22, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2024, 12, 25, 0, 0, 0, 0, DateTimeKind.Local), 1, false, false, 1 },
                    { 2, false, new DateTime(2024, 12, 22, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2024, 12, 25, 0, 0, 0, 0, DateTimeKind.Local), 2, false, false, 2 },
                    { 3, false, new DateTime(2024, 12, 19, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2024, 12, 22, 0, 0, 0, 0, DateTimeKind.Local), 3, true, false, 3 },
                    { 4, true, new DateTime(2024, 12, 16, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2024, 12, 20, 0, 0, 0, 0, DateTimeKind.Local), 4, false, true, 4 }
                });

            migrationBuilder.InsertData(
                table: "Invoices",
                columns: new[] { "InvoiceId", "BookingId", "IsPaid", "PaymentDeadline", "TotalAmount" },
                values: new object[,]
                {
                    { 1, 1, false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), 4500m },
                    { 2, 2, false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), 7000m },
                    { 3, 3, false, new DateTime(2024, 12, 28, 0, 0, 0, 0, DateTimeKind.Local), 6000m },
                    { 4, 4, true, new DateTime(2024, 12, 19, 0, 0, 0, 0, DateTimeKind.Local), 8000m }
                });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "PaymentId", "AmountPaid", "InvoiceId", "PaymentDate" },
                values: new object[] { 1, 8000m, 4, new DateTime(2024, 12, 19, 0, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_GuestId",
                table: "Bookings",
                column: "GuestId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_RoomId",
                table: "Bookings",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_BookingId",
                table: "Invoices",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_InvoiceId",
                table: "Payments",
                column: "InvoiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Guests");

            migrationBuilder.DropTable(
                name: "Rooms");
        }
    }
}
