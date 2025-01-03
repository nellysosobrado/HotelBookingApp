using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HotelBookingApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RemovalReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    SizeInSquareMeters = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
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
                    BookingCompleted = table.Column<bool>(type: "bit", nullable: false),
                    ExtraBeds = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CanceledDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCanceled = table.Column<bool>(type: "bit", nullable: false)
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
                name: "CanceledBookingsHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    GuestId = table.Column<int>(type: "int", nullable: false),
                    GuestName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CanceledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCanceled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanceledBookingsHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanceledBookingsHistory_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CanceledBookingsHistory_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
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
                    PaymentDeadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                columns: new[] { "GuestId", "DeletedDate", "Email", "FirstName", "IsDeleted", "LastName", "PhoneNumber", "RemovalReason" },
                values: new object[,]
                {
                    { 1, null, "margarita.d'amore0@example.com", "Margarita", false, "D'Amore", "658.247.8219", "Not applicable" },
                    { 2, null, "jaylin.beahan1@example.com", "Jaylin", false, "Beahan", "204-791-5675 x468", "Not applicable" },
                    { 3, null, "kiana.schulist2@example.com", "Kiana", false, "Schulist", "834-645-7666 x367", "Not applicable" },
                    { 4, null, "oral.gleason3@example.com", "Oral", false, "Gleason", "519.765.7953", "Not applicable" }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "RoomId", "ExtraBedPrice", "ExtraBeds", "IsAvailable", "IsDeleted", "PricePerNight", "SizeInSquareMeters", "TotalPeople", "Type" },
                values: new object[,]
                {
                    { 1, 0m, 1, true, false, 3463m, 65, 1m, "Single" },
                    { 2, 0m, 1, true, false, 2369m, 21, 3m, "Double" },
                    { 3, 0m, 2, true, false, 3922m, 22, 4m, "Double" },
                    { 4, 0m, 0, true, false, 4857m, 32, 2m, "Double" }
                });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "BookingId", "BookingCompleted", "CanceledDate", "CheckInDate", "CheckOutDate", "CreatedDate", "ExtraBeds", "GuestId", "IsCanceled", "IsCheckedIn", "IsCheckedOut", "RegistrationDate", "RoomId" },
                values: new object[,]
                {
                    { 1, false, null, new DateTime(2025, 1, 6, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 1, false, false, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, false, null, new DateTime(2025, 1, 6, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2025, 1, 11, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 2, false, false, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 3, false, null, new DateTime(2025, 1, 6, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 3, false, false, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 4, false, null, new DateTime(2025, 1, 4, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2025, 1, 8, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 4, false, false, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4 }
                });

            migrationBuilder.InsertData(
                table: "Invoices",
                columns: new[] { "InvoiceId", "BookingId", "CreatedDate", "IsPaid", "PaymentDeadline", "TotalAmount" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Local), false, new DateTime(2025, 1, 13, 0, 0, 0, 0, DateTimeKind.Local), 17241m },
                    { 2, 2, new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Local), false, new DateTime(2025, 1, 13, 0, 0, 0, 0, DateTimeKind.Local), 6144m },
                    { 3, 3, new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Local), true, new DateTime(2025, 1, 13, 0, 0, 0, 0, DateTimeKind.Local), 5214m },
                    { 4, 4, new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Local), false, new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Local), 10806m }
                });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "PaymentId", "AmountPaid", "InvoiceId", "PaymentDate" },
                values: new object[] { 1, 5214m, 3, new DateTime(2024, 12, 30, 0, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_GuestId",
                table: "Bookings",
                column: "GuestId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_RoomId",
                table: "Bookings",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_CanceledBookingsHistory_BookingId",
                table: "CanceledBookingsHistory",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_CanceledBookingsHistory_RoomId",
                table: "CanceledBookingsHistory",
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
                name: "CanceledBookingsHistory");

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
