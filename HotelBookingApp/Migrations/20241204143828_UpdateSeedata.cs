using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBookingApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 1,
                columns: new[] { "CheckInDate", "CheckOutDate" },
                values: new object[] { new DateTime(2024, 11, 29, 15, 38, 28, 594, DateTimeKind.Local).AddTicks(2563), new DateTime(2024, 12, 3, 15, 38, 28, 595, DateTimeKind.Local).AddTicks(5171) });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 2,
                column: "CheckInDate",
                value: new DateTime(2024, 12, 2, 15, 38, 28, 595, DateTimeKind.Local).AddTicks(5379));

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "InvoiceId",
                keyValue: 1,
                column: "PaymentDeadline",
                value: new DateTime(2024, 12, 1, 15, 38, 28, 595, DateTimeKind.Local).AddTicks(6270));

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "InvoiceId",
                keyValue: 2,
                column: "PaymentDeadline",
                value: new DateTime(2024, 12, 9, 15, 38, 28, 595, DateTimeKind.Local).AddTicks(6408));

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentId",
                keyValue: 1,
                column: "PaymentDate",
                value: new DateTime(2024, 11, 30, 15, 38, 28, 595, DateTimeKind.Local).AddTicks(6921));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 1,
                columns: new[] { "CheckInDate", "CheckOutDate" },
                values: new object[] { new DateTime(2024, 11, 29, 15, 31, 55, 18, DateTimeKind.Local).AddTicks(7191), new DateTime(2024, 12, 3, 15, 31, 55, 20, DateTimeKind.Local).AddTicks(22) });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 2,
                column: "CheckInDate",
                value: new DateTime(2024, 12, 2, 15, 31, 55, 20, DateTimeKind.Local).AddTicks(226));

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "InvoiceId",
                keyValue: 1,
                column: "PaymentDeadline",
                value: new DateTime(2024, 12, 1, 15, 31, 55, 20, DateTimeKind.Local).AddTicks(1088));

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "InvoiceId",
                keyValue: 2,
                column: "PaymentDeadline",
                value: new DateTime(2024, 12, 9, 15, 31, 55, 20, DateTimeKind.Local).AddTicks(1219));

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentId",
                keyValue: 1,
                column: "PaymentDate",
                value: new DateTime(2024, 11, 30, 15, 31, 55, 20, DateTimeKind.Local).AddTicks(1717));
        }
    }
}
