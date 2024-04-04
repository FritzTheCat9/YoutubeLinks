using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YoutubeLinks.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserThemeColor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ThemeColor",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified", "ThemeColor" },
                values: new object[] { new DateTime(2024, 4, 4, 16, 42, 44, 179, DateTimeKind.Utc).AddTicks(7086), new DateTime(2024, 4, 4, 16, 42, 44, 179, DateTimeKind.Utc).AddTicks(7086), 0 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified", "ThemeColor" },
                values: new object[] { new DateTime(2024, 4, 4, 16, 42, 44, 179, DateTimeKind.Utc).AddTicks(7086), new DateTime(2024, 4, 4, 16, 42, 44, 179, DateTimeKind.Utc).AddTicks(7086), 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThemeColor",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 9, 16, 48, 6, 921, DateTimeKind.Utc).AddTicks(1924), new DateTime(2024, 3, 9, 16, 48, 6, 921, DateTimeKind.Utc).AddTicks(1924) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 9, 16, 48, 6, 921, DateTimeKind.Utc).AddTicks(1924), new DateTime(2024, 3, 9, 16, 48, 6, 921, DateTimeKind.Utc).AddTicks(1924) });
        }
    }
}
