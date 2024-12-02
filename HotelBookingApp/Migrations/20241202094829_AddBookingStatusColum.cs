using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBookingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingStatusColum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BookingStatus",
                table: "Bookings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookingStatus",
                table: "Bookings");
        }
    }
}
