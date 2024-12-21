using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HotelBookingApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookingwithinvoicesandpayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 1,
                columns: new[] { "BookingStatus", "CheckInDate", "CheckOutDate", "IsCheckedIn", "IsCheckedOut" },
                values: new object[] { false, new DateTime(2024, 12, 18, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2024, 12, 21, 0, 0, 0, 0, DateTimeKind.Local), false, false });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 2,
                columns: new[] { "CheckInDate", "CheckOutDate", "IsCheckedIn" },
                values: new object[] { new DateTime(2024, 12, 18, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2024, 12, 21, 0, 0, 0, 0, DateTimeKind.Local), false });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "BookingId", "BookingStatus", "CheckInDate", "CheckOutDate", "GuestId", "IsCheckedIn", "IsCheckedOut", "RoomId" },
                values: new object[,]
                {
                    { 3, false, new DateTime(2024, 12, 15, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2024, 12, 18, 0, 0, 0, 0, DateTimeKind.Local), 3, true, false, 3 },
                    { 4, true, new DateTime(2024, 12, 12, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2024, 12, 16, 0, 0, 0, 0, DateTimeKind.Local), 4, false, true, 4 }
                });

            //migrationBuilder.UpdateData(
            //    table: "Guests",
            //    keyColumn: "GuestId",
            //    keyValue: 1,
            //    columns: new[] { "Email", "FirstName", "LastName", "PhoneNumber" },
            //    values: new object[] { "gmail.com1", "p1", "l1", "11111" });

            //migrationBuilder.UpdateData(
            //    table: "Guests",
            //    keyColumn: "GuestId",
            //    keyValue: 2,
            //    columns: new[] { "Email", "FirstName", "LastName", "PhoneNumber" },
            //    values: new object[] { "gmail.com2", "p2", "l2", "22222" });

            //migrationBuilder.UpdateData(
            //    table: "Guests",
            //    keyColumn: "GuestId",
            //    keyValue: 3,
            //    columns: new[] { "Email", "FirstName", "LastName", "PhoneNumber" },
            //    values: new object[] { "gmail.com3", "p3", "l3", "33333" });

            //migrationBuilder.UpdateData(
            //    table: "Guests",
            //    keyColumn: "GuestId",
            //    keyValue: 4,
            //    columns: new[] { "Email", "FirstName", "LastName", "PhoneNumber" },
            //    values: new object[] { "gmail.com4", "p4", "l4", "44444" });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "InvoiceId",
                keyValue: 1,
                columns: new[] { "IsPaid", "PaymentDeadline", "TotalAmount" },
                values: new object[] { false, new DateTime(2024, 12, 28, 0, 0, 0, 0, DateTimeKind.Local), 4500m });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "InvoiceId",
                keyValue: 2,
                columns: new[] { "PaymentDeadline", "TotalAmount" },
                values: new object[] { new DateTime(2024, 12, 28, 0, 0, 0, 0, DateTimeKind.Local), 7000m });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentId",
                keyValue: 1,
                columns: new[] { "AmountPaid", "InvoiceId", "PaymentDate" },
                values: new object[] { 8000m, 4, new DateTime(2024, 12, 15, 0, 0, 0, 0, DateTimeKind.Local) });

            //migrationBuilder.UpdateData(
            //    table: "Rooms",
            //    keyColumn: "RoomId",
            //    keyValue: 3,
            //    columns: new[] { "ExtraBeds", "PricePerNight", "SizeInSquareMeters", "Type" },
            //    values: new object[] { 0, 2000m, 25, "Single" });

            //migrationBuilder.UpdateData(
            //    table: "Rooms",
            //    keyColumn: "RoomId",
            //    keyValue: 4,
            //    columns: new[] { "ExtraBeds", "PricePerNight", "SizeInSquareMeters", "Type" },
            //    values: new object[] { 2, 4000m, 90, "Double" });

            migrationBuilder.InsertData(
                table: "Invoices",
                columns: new[] { "InvoiceId", "BookingId", "IsPaid", "PaymentDeadline", "TotalAmount" },
                values: new object[,]
                {
                    { 3, 3, false, new DateTime(2024, 12, 24, 0, 0, 0, 0, DateTimeKind.Local), 6000m },
                    { 4, 4, true, new DateTime(2024, 12, 15, 0, 0, 0, 0, DateTimeKind.Local), 8000m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Invoices",
                keyColumn: "InvoiceId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Invoices",
                keyColumn: "InvoiceId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 4);

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 1,
                columns: new[] { "BookingStatus", "CheckInDate", "CheckOutDate", "IsCheckedIn", "IsCheckedOut" },
                values: new object[] { true, new DateTime(2024, 11, 29, 15, 38, 28, 594, DateTimeKind.Local).AddTicks(2563), new DateTime(2024, 12, 3, 15, 38, 28, 595, DateTimeKind.Local).AddTicks(5171), true, true });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 2,
                columns: new[] { "CheckInDate", "CheckOutDate", "IsCheckedIn" },
                values: new object[] { new DateTime(2024, 12, 2, 15, 38, 28, 595, DateTimeKind.Local).AddTicks(5379), null, true });

            //migrationBuilder.UpdateData(
            //    table: "Guests",
            //    keyColumn: "GuestId",
            //    keyValue: 1,
            //    columns: new[] { "Email", "FirstName", "LastName", "PhoneNumber" },
            //    values: new object[] { "234p@example.com", "Person2", "lastname1", "234" });

            //migrationBuilder.UpdateData(
            //    table: "Guests",
            //    keyColumn: "GuestId",
            //    keyValue: 2,
            //    columns: new[] { "Email", "FirstName", "LastName", "PhoneNumber" },
            //    values: new object[] { "342p@example.com", "Person2", "lastname2", "3453" });

            //migrationBuilder.UpdateData(
            //    table: "Guests",
            //    keyColumn: "GuestId",
            //    keyValue: 3,
            //    columns: new[] { "Email", "FirstName", "LastName", "PhoneNumber" },
            //    values: new object[] { "p234@example.com", "Person3", "lastname3", "3453" });

            //migrationBuilder.UpdateData(
            //    table: "Guests",
            //    keyColumn: "GuestId",
            //    keyValue: 4,
            //    columns: new[] { "Email", "FirstName", "LastName", "PhoneNumber" },
            //    values: new object[] { "243p@example.com", "Person4", "lastname4", "43" });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "InvoiceId",
                keyValue: 1,
                columns: new[] { "IsPaid", "PaymentDeadline", "TotalAmount" },
                values: new object[] { true, new DateTime(2024, 12, 1, 15, 38, 28, 595, DateTimeKind.Local).AddTicks(6270), 500.00m });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "InvoiceId",
                keyValue: 2,
                columns: new[] { "PaymentDeadline", "TotalAmount" },
                values: new object[] { new DateTime(2024, 12, 9, 15, 38, 28, 595, DateTimeKind.Local).AddTicks(6408), 300.00m });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentId",
                keyValue: 1,
                columns: new[] { "AmountPaid", "InvoiceId", "PaymentDate" },
                values: new object[] { 500.00m, 1, new DateTime(2024, 11, 30, 15, 38, 28, 595, DateTimeKind.Local).AddTicks(6921) });

            //migrationBuilder.UpdateData(
            //    table: "Rooms",
            //    keyColumn: "RoomId",
            //    keyValue: 3,
            //    columns: new[] { "ExtraBeds", "PricePerNight", "SizeInSquareMeters", "Type" },
            //    values: new object[] { 2, 3500m, 70, "Double" });

            //migrationBuilder.UpdateData(
            //    table: "Rooms",
            //    keyColumn: "RoomId",
            //    keyValue: 4,
            //    columns: new[] { "ExtraBeds", "PricePerNight", "SizeInSquareMeters", "Type" },
            //    values: new object[] { 0, 1500m, 215, "Single" });
        }
    }
}
