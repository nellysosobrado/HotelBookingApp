using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBookingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToRooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Rooms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 1,
                columns: new[] { "CheckInDate", "CheckOutDate" },
                values: new object[] { new DateTime(2024, 12, 30, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2025, 1, 4, 0, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 2,
                columns: new[] { "CheckInDate", "CheckOutDate" },
                values: new object[] { new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2025, 1, 4, 0, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 3,
                columns: new[] { "CheckInDate", "CheckOutDate" },
                values: new object[] { new DateTime(2024, 12, 31, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 4,
                columns: new[] { "CheckInDate", "CheckOutDate" },
                values: new object[] { new DateTime(2024, 12, 31, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.UpdateData(
                table: "Guests",
                keyColumn: "GuestId",
                keyValue: 1,
                columns: new[] { "Email", "FirstName", "LastName", "PhoneNumber" },
                values: new object[] { "noemy.rowe0@example.com", "Noemy", "Rowe", "670-973-4269" });

            migrationBuilder.UpdateData(
                table: "Guests",
                keyColumn: "GuestId",
                keyValue: 2,
                columns: new[] { "Email", "FirstName", "LastName", "PhoneNumber" },
                values: new object[] { "maya.daniel1@example.com", "Maya", "Daniel", "708.926.4820" });

            migrationBuilder.UpdateData(
                table: "Guests",
                keyColumn: "GuestId",
                keyValue: 3,
                columns: new[] { "Email", "FirstName", "LastName", "PhoneNumber" },
                values: new object[] { "dalton.armstrong2@example.com", "Dalton", "Armstrong", "908.339.2334" });

            migrationBuilder.UpdateData(
                table: "Guests",
                keyColumn: "GuestId",
                keyValue: 4,
                columns: new[] { "Email", "FirstName", "LastName", "PhoneNumber" },
                values: new object[] { "jermain.becker3@example.com", "Jermain", "Becker", "350-395-8028" });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "InvoiceId",
                keyValue: 1,
                columns: new[] { "PaymentDeadline", "TotalAmount" },
                values: new object[] { new DateTime(2025, 1, 11, 0, 0, 0, 0, DateTimeKind.Local), 9888m });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "InvoiceId",
                keyValue: 2,
                columns: new[] { "PaymentDeadline", "TotalAmount" },
                values: new object[] { new DateTime(2025, 1, 11, 0, 0, 0, 0, DateTimeKind.Local), 19690m });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "InvoiceId",
                keyValue: 3,
                columns: new[] { "PaymentDeadline", "TotalAmount" },
                values: new object[] { new DateTime(2025, 1, 9, 0, 0, 0, 0, DateTimeKind.Local), 19926m });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "InvoiceId",
                keyValue: 4,
                columns: new[] { "PaymentDeadline", "TotalAmount" },
                values: new object[] { new DateTime(2025, 1, 9, 0, 0, 0, 0, DateTimeKind.Local), 7103m });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentId",
                keyValue: 1,
                columns: new[] { "AmountPaid", "PaymentDate" },
                values: new object[] { 4944m, new DateTime(2024, 12, 27, 0, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentId",
                keyValue: 2,
                columns: new[] { "AmountPaid", "PaymentDate" },
                values: new object[] { 9845m, new DateTime(2024, 12, 24, 0, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentId",
                keyValue: 3,
                columns: new[] { "AmountPaid", "PaymentDate" },
                values: new object[] { 9963m, new DateTime(2024, 12, 26, 0, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentId",
                keyValue: 4,
                columns: new[] { "AmountPaid", "PaymentDate" },
                values: new object[] { 3551.5m, new DateTime(2024, 12, 26, 0, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: 1,
                columns: new[] { "IsDeleted", "PricePerNight", "SizeInSquareMeters" },
                values: new object[] { false, 1463m, 76 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: 2,
                columns: new[] { "ExtraBeds", "IsDeleted", "PricePerNight", "SizeInSquareMeters", "Type" },
                values: new object[] { 1, false, 2705m, 53, "Single" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: 3,
                columns: new[] { "ExtraBeds", "IsDeleted", "PricePerNight", "SizeInSquareMeters", "Type" },
                values: new object[] { 2, false, 1735m, 62, "Double" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: 4,
                columns: new[] { "ExtraBeds", "IsDeleted", "PricePerNight", "SizeInSquareMeters", "Type" },
                values: new object[] { 1, false, 2028m, 39, "Double" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Rooms");

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 1,
                columns: new[] { "CheckInDate", "CheckOutDate" },
                values: new object[] { new DateTime(2024, 12, 26, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2024, 12, 28, 0, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 2,
                columns: new[] { "CheckInDate", "CheckOutDate" },
                values: new object[] { new DateTime(2024, 12, 28, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2024, 12, 31, 0, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 3,
                columns: new[] { "CheckInDate", "CheckOutDate" },
                values: new object[] { new DateTime(2024, 12, 26, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2024, 12, 30, 0, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 4,
                columns: new[] { "CheckInDate", "CheckOutDate" },
                values: new object[] { new DateTime(2024, 12, 26, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2024, 12, 31, 0, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.UpdateData(
                table: "Guests",
                keyColumn: "GuestId",
                keyValue: 1,
                columns: new[] { "Email", "FirstName", "LastName", "PhoneNumber" },
                values: new object[] { "mafalda.gleichner0@example.com", "Mafalda", "Gleichner", "679-759-8552 x99711" });

            migrationBuilder.UpdateData(
                table: "Guests",
                keyColumn: "GuestId",
                keyValue: 2,
                columns: new[] { "Email", "FirstName", "LastName", "PhoneNumber" },
                values: new object[] { "katelyn.balistreri1@example.com", "Katelyn", "Balistreri", "559-837-9563" });

            migrationBuilder.UpdateData(
                table: "Guests",
                keyColumn: "GuestId",
                keyValue: 3,
                columns: new[] { "Email", "FirstName", "LastName", "PhoneNumber" },
                values: new object[] { "dedric.borer2@example.com", "Dedric", "Borer", "853-445-1967" });

            migrationBuilder.UpdateData(
                table: "Guests",
                keyColumn: "GuestId",
                keyValue: 4,
                columns: new[] { "Email", "FirstName", "LastName", "PhoneNumber" },
                values: new object[] { "davon.rodriguez3@example.com", "Davon", "Rodriguez", "780-854-4991 x346" });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "InvoiceId",
                keyValue: 1,
                columns: new[] { "PaymentDeadline", "TotalAmount" },
                values: new object[] { new DateTime(2025, 1, 4, 0, 0, 0, 0, DateTimeKind.Local), 5161m });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "InvoiceId",
                keyValue: 2,
                columns: new[] { "PaymentDeadline", "TotalAmount" },
                values: new object[] { new DateTime(2025, 1, 7, 0, 0, 0, 0, DateTimeKind.Local), 16124m });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "InvoiceId",
                keyValue: 3,
                columns: new[] { "PaymentDeadline", "TotalAmount" },
                values: new object[] { new DateTime(2025, 1, 6, 0, 0, 0, 0, DateTimeKind.Local), 11573m });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "InvoiceId",
                keyValue: 4,
                columns: new[] { "PaymentDeadline", "TotalAmount" },
                values: new object[] { new DateTime(2025, 1, 7, 0, 0, 0, 0, DateTimeKind.Local), 13557m });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentId",
                keyValue: 1,
                columns: new[] { "AmountPaid", "PaymentDate" },
                values: new object[] { 2580.5m, new DateTime(2024, 12, 22, 0, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentId",
                keyValue: 2,
                columns: new[] { "AmountPaid", "PaymentDate" },
                values: new object[] { 8062m, new DateTime(2024, 12, 20, 0, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentId",
                keyValue: 3,
                columns: new[] { "AmountPaid", "PaymentDate" },
                values: new object[] { 5786.5m, new DateTime(2024, 12, 22, 0, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentId",
                keyValue: 4,
                columns: new[] { "AmountPaid", "PaymentDate" },
                values: new object[] { 6778.5m, new DateTime(2024, 12, 20, 0, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: 1,
                columns: new[] { "PricePerNight", "SizeInSquareMeters" },
                values: new object[] { 4096m, 51 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: 2,
                columns: new[] { "ExtraBeds", "PricePerNight", "SizeInSquareMeters", "Type" },
                values: new object[] { 0, 1487m, 84, "Double" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: 3,
                columns: new[] { "ExtraBeds", "PricePerNight", "SizeInSquareMeters", "Type" },
                values: new object[] { 0, 1607m, 48, "Single" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: 4,
                columns: new[] { "ExtraBeds", "PricePerNight", "SizeInSquareMeters", "Type" },
                values: new object[] { 2, 1634m, 36, "Single" });
        }
    }
}
