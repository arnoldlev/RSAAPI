using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RSAAPI.Migrations
{
    public partial class LicenseKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 8, 26, 11, 13, 9, 846, DateTimeKind.Local).AddTicks(4037),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 8, 26, 10, 46, 50, 745, DateTimeKind.Local).AddTicks(2624));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 8, 26, 11, 13, 9, 846, DateTimeKind.Local).AddTicks(3889),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 8, 26, 10, 46, 50, 745, DateTimeKind.Local).AddTicks(2311));

            migrationBuilder.AddColumn<string>(
                name: "LicenseKey",
                table: "Users",
                type: "nvarchar(1025)",
                maxLength: 1025,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LicenseKey",
                table: "Users");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 8, 26, 10, 46, 50, 745, DateTimeKind.Local).AddTicks(2624),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 8, 26, 11, 13, 9, 846, DateTimeKind.Local).AddTicks(4037));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 8, 26, 10, 46, 50, 745, DateTimeKind.Local).AddTicks(2311),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 8, 26, 11, 13, 9, 846, DateTimeKind.Local).AddTicks(3889));
        }
    }
}
