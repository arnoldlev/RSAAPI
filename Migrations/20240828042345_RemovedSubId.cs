using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RSAAPI.Migrations
{
    public partial class RemovedSubId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubId",
                table: "Users");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 8, 27, 21, 23, 45, 571, DateTimeKind.Local).AddTicks(5960),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 8, 26, 11, 13, 9, 846, DateTimeKind.Local).AddTicks(4037));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 8, 27, 21, 23, 45, 571, DateTimeKind.Local).AddTicks(5818),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 8, 26, 11, 13, 9, 846, DateTimeKind.Local).AddTicks(3889));

            migrationBuilder.AddColumn<string>(
                name: "ApiToken",
                table: "Users",
                type: "nvarchar(1025)",
                maxLength: 1025,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SandBoxToken",
                table: "Users",
                type: "nvarchar(1025)",
                maxLength: 1025,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SandBoxToken",
                table: "Users");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 8, 26, 11, 13, 9, 846, DateTimeKind.Local).AddTicks(4037),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 8, 27, 21, 23, 45, 571, DateTimeKind.Local).AddTicks(5960));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 8, 26, 11, 13, 9, 846, DateTimeKind.Local).AddTicks(3889),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 8, 27, 21, 23, 45, 571, DateTimeKind.Local).AddTicks(5818));

            migrationBuilder.AddColumn<string>(
                name: "SubId",
                table: "Users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
