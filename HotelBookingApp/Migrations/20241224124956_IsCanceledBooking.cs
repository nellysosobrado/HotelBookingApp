using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBookingApp.Migrations
{
    /// <inheritdoc />
    public partial class IsCanceledBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 1,
                columns: new[] { "CheckInDate", "CheckOutDate", "IsCanceled" },
                values: new object[] { new DateTime(2024, 12, 26, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2024, 12, 28, 0, 0, 0, 0, DateTimeKind.Local), false });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 2,
                columns: new[] { "CheckInDate", "CheckOutDate", "IsCanceled" },
                values: new object[] { new DateTime(2024, 12, 28, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2024, 12, 31, 0, 0, 0, 0, DateTimeKind.Local), false });

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
                columns: new[] { "CheckInDate", "CheckOutDate", "IsCanceled" },
                values: new object[] { new DateTime(2024, 12, 26, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2024, 12, 31, 0, 0, 0, 0, DateTimeKind.Local), false });

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
                column: "AmountPaid",
                value: 2580.5m);

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
                columns: new[] { "ExtraBeds", "PricePerNight", "SizeInSquareMeters", "Type" },
                values: new object[] { 0, 4096m, 51, "Double" });

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
                columns: new[] { "ExtraBeds", "PricePerNight", "SizeInSquareMeters" },
                values: new object[] { 0, 1607m, 48 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: 4,
                columns: new[] { "ExtraBeds", "PricePerNight", "SizeInSquareMeters" },
                values: new object[] { 2, 1634m, 36 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 1,
                columns: new[] { "CheckInDate", "CheckOutDate", "IsCanceled" },
                values: new object[] { new DateTime(2024, 12, 28, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2024, 12, 29, 0, 0, 0, 0, DateTimeKind.Local), true });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 2,
                columns: new[] { "CheckInDate", "CheckOutDate", "IsCanceled" },
                values: new object[] { new DateTime(2024, 12, 27, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2024, 12, 29, 0, 0, 0, 0, DateTimeKind.Local), true });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 3,
                columns: new[] { "CheckInDate", "CheckOutDate" },
                values: new object[] { new DateTime(2024, 12, 28, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2024, 12, 29, 0, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 4,
                columns: new[] { "CheckInDate", "CheckOutDate", "IsCanceled" },
                values: new object[] { new DateTime(2024, 12, 27, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2024, 12, 28, 0, 0, 0, 0, DateTimeKind.Local), true });

            migrationBuilder.UpdateData(
                table: "Guests",
                keyColumn: "GuestId",
                keyValue: 1,
                columns: new[] { "Email", "FirstName", "LastName", "PhoneNumber" },
                values: new object[] { "ryan.rodriguez0@example.com", "Ryan", "Rodriguez", "1-429-497-9296" });

            migrationBuilder.UpdateData(
                table: "Guests",
                keyColumn: "GuestId",
                keyValue: 2,
                columns: new[] { "Email", "FirstName", "LastName", "PhoneNumber" },
                values: new object[] { "christina.dietrich1@example.com", "Christina", "Dietrich", "1-244-353-1104 x21370" });

            migrationBuilder.UpdateData(
                table: "Guests",
                keyColumn: "GuestId",
                keyValue: 3,
                columns: new[] { "Email", "FirstName", "LastName", "PhoneNumber" },
                values: new object[] { "amparo.schulist2@example.com", "Amparo", "Schulist", "(396) 279-8814" });

            migrationBuilder.UpdateData(
                table: "Guests",
                keyColumn: "GuestId",
                keyValue: 4,
                columns: new[] { "Email", "FirstName", "LastName", "PhoneNumber" },
                values: new object[] { "austin.yost3@example.com", "Austin", "Yost", "(516) 724-7554 x06613" });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "InvoiceId",
                keyValue: 1,
                columns: new[] { "PaymentDeadline", "TotalAmount" },
                values: new object[] { new DateTime(2025, 1, 5, 0, 0, 0, 0, DateTimeKind.Local), 13996m });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "InvoiceId",
                keyValue: 2,
                columns: new[] { "PaymentDeadline", "TotalAmount" },
                values: new object[] { new DateTime(2025, 1, 5, 0, 0, 0, 0, DateTimeKind.Local), 11759m });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "InvoiceId",
                keyValue: 3,
                columns: new[] { "PaymentDeadline", "TotalAmount" },
                values: new object[] { new DateTime(2025, 1, 5, 0, 0, 0, 0, DateTimeKind.Local), 19496m });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "InvoiceId",
                keyValue: 4,
                columns: new[] { "PaymentDeadline", "TotalAmount" },
                values: new object[] { new DateTime(2025, 1, 4, 0, 0, 0, 0, DateTimeKind.Local), 17602m });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentId",
                keyValue: 1,
                column: "AmountPaid",
                value: 6998m);

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentId",
                keyValue: 2,
                columns: new[] { "AmountPaid", "PaymentDate" },
                values: new object[] { 5879.5m, new DateTime(2024, 12, 21, 0, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentId",
                keyValue: 3,
                columns: new[] { "AmountPaid", "PaymentDate" },
                values: new object[] { 9748m, new DateTime(2024, 12, 20, 0, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentId",
                keyValue: 4,
                columns: new[] { "AmountPaid", "PaymentDate" },
                values: new object[] { 8801m, new DateTime(2024, 12, 22, 0, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: 1,
                columns: new[] { "ExtraBeds", "PricePerNight", "SizeInSquareMeters", "Type" },
                values: new object[] { 1, 3438m, 96, "Single" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: 2,
                columns: new[] { "ExtraBeds", "PricePerNight", "SizeInSquareMeters", "Type" },
                values: new object[] { 1, 1575m, 93, "Single" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: 3,
                columns: new[] { "ExtraBeds", "PricePerNight", "SizeInSquareMeters" },
                values: new object[] { 1, 1393m, 67 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: 4,
                columns: new[] { "ExtraBeds", "PricePerNight", "SizeInSquareMeters" },
                values: new object[] { 1, 1872m, 63 });
        }
    }
}
