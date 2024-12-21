using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HotelBookingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalpeopleColum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<decimal>(
            //    name: "TotalPeople",
            //    table: "Rooms",
            //    type: "decimal(18,2)",
            //    nullable: false,
            //    defaultValue: 0m);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CheckOutDate",
                table: "Bookings",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CheckInDate",
                table: "Bookings",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "BookingId", "BookingStatus", "CheckInDate", "CheckOutDate", "GuestId", "IsCheckedIn", "IsCheckedOut", "RoomId" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2024, 11, 29, 15, 31, 55, 18, DateTimeKind.Local).AddTicks(7191), new DateTime(2024, 12, 3, 15, 31, 55, 20, DateTimeKind.Local).AddTicks(22), 1, true, true, 1 },
                    { 2, false, new DateTime(2024, 12, 2, 15, 31, 55, 20, DateTimeKind.Local).AddTicks(226), null, 2, true, false, 2 }
                });

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
                columns: new[] { "IsPaid", "PaymentDeadline" },
                values: new object[] { true, new DateTime(2024, 12, 1, 15, 31, 55, 20, DateTimeKind.Local).AddTicks(1088) });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "InvoiceId",
                keyValue: 2,
                columns: new[] { "IsPaid", "PaymentDeadline" },
                values: new object[] { false, new DateTime(2024, 12, 9, 15, 31, 55, 20, DateTimeKind.Local).AddTicks(1219) });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentId",
                keyValue: 1,
                columns: new[] { "AmountPaid", "InvoiceId", "PaymentDate" },
                values: new object[] { 500.00m, 1, new DateTime(2024, 11, 30, 15, 31, 55, 20, DateTimeKind.Local).AddTicks(1717) });

            //migrationBuilder.UpdateData(
            //    table: "Rooms",
            //    keyColumn: "RoomId",
            //    keyValue: 1,
            //    columns: new[] { "PricePerNight", "SizeInSquareMeters", "TotalPeople" },
            //    values: new object[] { 1500m, 20, 0m });

            //migrationBuilder.UpdateData(
            //    table: "Rooms",
            //    keyColumn: "RoomId",
            //    keyValue: 2,
            //    columns: new[] { "PricePerNight", "SizeInSquareMeters", "TotalPeople" },
            //    values: new object[] { 3500m, 80, 0m });

            //migrationBuilder.UpdateData(
            //    table: "Rooms",
            //    keyColumn: "RoomId",
            //    keyValue: 3,
            //    columns: new[] { "IsAvailable", "PricePerNight", "SizeInSquareMeters", "TotalPeople" },
            //    values: new object[] { true, 3500m, 70, 0m });

            //migrationBuilder.UpdateData(
            //    table: "Rooms",
            //    keyColumn: "RoomId",
            //    keyValue: 4,
            //    columns: new[] { "IsAvailable", "PricePerNight", "SizeInSquareMeters", "TotalPeople" },
            //    values: new object[] { true, 1500m, 215, 0m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "TotalPeople",
                table: "Rooms");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CheckOutDate",
                table: "Bookings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CheckInDate",
                table: "Bookings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            //migrationBuilder.UpdateData(
            //    table: "Guests",
            //    keyColumn: "GuestId",
            //    keyValue: 1,
            //    columns: new[] { "Email", "FirstName", "LastName", "PhoneNumber" },
            //    values: new object[] { "alice@example.com", "Alice", "Smith", "1234567890" });

            //migrationBuilder.UpdateData(
            //    table: "Guests",
            //    keyColumn: "GuestId",
            //    keyValue: 2,
            //    columns: new[] { "Email", "FirstName", "LastName", "PhoneNumber" },
            //    values: new object[] { "bob@example.com", "Bob", "Johnson", "2345678901" });

            //migrationBuilder.UpdateData(
            //    table: "Guests",
            //    keyColumn: "GuestId",
            //    keyValue: 3,
            //    columns: new[] { "Email", "FirstName", "LastName", "PhoneNumber" },
            //    values: new object[] { "charlie@example.com", "Charlie", "Brown", "3456789012" });

            //migrationBuilder.UpdateData(
            //    table: "Guests",
            //    keyColumn: "GuestId",
            //    keyValue: 4,
            //    columns: new[] { "Email", "FirstName", "LastName", "PhoneNumber" },
            //    values: new object[] { "diana@example.com", "Diana", "Green", "4567890123" });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "InvoiceId",
                keyValue: 1,
                columns: new[] { "IsPaid", "PaymentDeadline" },
                values: new object[] { false, new DateTime(2024, 12, 9, 13, 9, 34, 360, DateTimeKind.Local).AddTicks(7603) });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "InvoiceId",
                keyValue: 2,
                columns: new[] { "IsPaid", "PaymentDeadline" },
                values: new object[] { true, new DateTime(2024, 12, 12, 13, 9, 34, 362, DateTimeKind.Local).AddTicks(1462) });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentId",
                keyValue: 1,
                columns: new[] { "AmountPaid", "InvoiceId", "PaymentDate" },
                values: new object[] { 300.00m, 2, new DateTime(2024, 11, 30, 13, 9, 34, 362, DateTimeKind.Local).AddTicks(2113) });

            //migrationBuilder.UpdateData(
            //    table: "Rooms",
            //    keyColumn: "RoomId",
            //    keyValue: 1,
            //    columns: new[] { "PricePerNight", "SizeInSquareMeters" },
            //    values: new object[] { 0m, 0 });

            //migrationBuilder.UpdateData(
            //    table: "Rooms",
            //    keyColumn: "RoomId",
            //    keyValue: 2,
            //    columns: new[] { "PricePerNight", "SizeInSquareMeters" },
            //    values: new object[] { 0m, 0 });

            //migrationBuilder.UpdateData(
            //    table: "Rooms",
            //    keyColumn: "RoomId",
            //    keyValue: 3,
            //    columns: new[] { "IsAvailable", "PricePerNight", "SizeInSquareMeters" },
            //    values: new object[] { false, 0m, 0 });

            //migrationBuilder.UpdateData(
            //    table: "Rooms",
            //    keyColumn: "RoomId",
            //    keyValue: 4,
            //    columns: new[] { "IsAvailable", "PricePerNight", "SizeInSquareMeters" },
            //    values: new object[] { false, 0m, 0 });
        }
    }
}
