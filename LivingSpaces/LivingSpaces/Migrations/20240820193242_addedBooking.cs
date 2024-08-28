using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LivingSpaces.Migrations
{
    /// <inheritdoc />
    public partial class addedBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyBookings",
                columns: table => new
                {
                    DailyBookingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyBookings", x => x.DailyBookingID);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyBookings",
                columns: table => new
                {
                    WeeklyBookingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MondayStartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    MondayEndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    TuesdayStartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    TuesdayEndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    WednesdayStartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    WednesdayEndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ThursdayStartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ThursdayEndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    FridayStartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    FridayEndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    SaturdayStartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    SaturdayEndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    SundayStartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    SundayEndTime = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyBookings", x => x.WeeklyBookingID);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    BookingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingType = table.Column<int>(type: "int", nullable: false),
                    DailyBookingID = table.Column<int>(type: "int", nullable: true),
                    WeeklyBookingID = table.Column<int>(type: "int", nullable: true),
                    PropertyListingID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.BookingID);
                    table.ForeignKey(
                        name: "FK_Bookings_DailyBookings_DailyBookingID",
                        column: x => x.DailyBookingID,
                        principalTable: "DailyBookings",
                        principalColumn: "DailyBookingID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_Properties_PropertyListingID",
                        column: x => x.PropertyListingID,
                        principalTable: "Properties",
                        principalColumn: "PropertyListingID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_WeeklyBookings_WeeklyBookingID",
                        column: x => x.WeeklyBookingID,
                        principalTable: "WeeklyBookings",
                        principalColumn: "WeeklyBookingID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_DailyBookingID",
                table: "Bookings",
                column: "DailyBookingID",
                unique: true,
                filter: "[DailyBookingID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PropertyListingID",
                table: "Bookings",
                column: "PropertyListingID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_WeeklyBookingID",
                table: "Bookings",
                column: "WeeklyBookingID",
                unique: true,
                filter: "[WeeklyBookingID] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "DailyBookings");

            migrationBuilder.DropTable(
                name: "WeeklyBookings");
        }
    }
}
