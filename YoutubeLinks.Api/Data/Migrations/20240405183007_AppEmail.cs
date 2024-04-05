using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YoutubeLinks.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AppEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Email", "Modified" },
                values: new object[] { new DateTime(2024, 4, 5, 18, 30, 7, 34, DateTimeKind.Utc).AddTicks(2726), "ytlinksapp@gmail.com", new DateTime(2024, 4, 5, 18, 30, 7, 34, DateTimeKind.Utc).AddTicks(2726) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Email", "Modified" },
                values: new object[] { new DateTime(2024, 4, 5, 18, 30, 7, 34, DateTimeKind.Utc).AddTicks(2726), "ytlinksapp1@gmail.com", new DateTime(2024, 4, 5, 18, 30, 7, 34, DateTimeKind.Utc).AddTicks(2726) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Email", "Modified" },
                values: new object[] { new DateTime(2024, 4, 4, 16, 42, 44, 179, DateTimeKind.Utc).AddTicks(7086), "freakfightsfan@gmail.com", new DateTime(2024, 4, 4, 16, 42, 44, 179, DateTimeKind.Utc).AddTicks(7086) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Email", "Modified" },
                values: new object[] { new DateTime(2024, 4, 4, 16, 42, 44, 179, DateTimeKind.Utc).AddTicks(7086), "freakfightsfan1@gmail.com", new DateTime(2024, 4, 4, 16, 42, 44, 179, DateTimeKind.Utc).AddTicks(7086) });
        }
    }
}
