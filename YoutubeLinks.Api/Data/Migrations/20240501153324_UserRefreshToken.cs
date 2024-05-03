using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YoutubeLinks.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified", "RefreshToken" },
                values: new object[] { new DateTime(2024, 5, 1, 15, 33, 23, 935, DateTimeKind.Utc).AddTicks(5956), new DateTime(2024, 5, 1, 15, 33, 23, 935, DateTimeKind.Utc).AddTicks(5956), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified", "RefreshToken" },
                values: new object[] { new DateTime(2024, 5, 1, 15, 33, 23, 935, DateTimeKind.Utc).AddTicks(5956), new DateTime(2024, 5, 1, 15, 33, 23, 935, DateTimeKind.Utc).AddTicks(5956), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 4, 5, 18, 30, 7, 34, DateTimeKind.Utc).AddTicks(2726), new DateTime(2024, 4, 5, 18, 30, 7, 34, DateTimeKind.Utc).AddTicks(2726) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 4, 5, 18, 30, 7, 34, DateTimeKind.Utc).AddTicks(2726), new DateTime(2024, 4, 5, 18, 30, 7, 34, DateTimeKind.Utc).AddTicks(2726) });
        }
    }
}
